using Discord;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class RoslynService
    {
        private readonly IServiceProvider _services;

        public RoslynService(IServiceProvider services)
        {
            _services = services;
        }

        public async Task<Embed> EvaluateAsync(DogeyCommandContext context, string script)
        {
            var timer = Stopwatch.StartNew();

            var options = GetOptions();
            string formatted = GetFormattedCode("cs", script);

            object result;
            try
            {
                result = await CSharpScript.EvaluateAsync(formatted, options, new RoslynGlobals(context, _services));
            } catch (Exception ex)
            {
                result = ex;
            }

            timer.Stop();
            return GetEmbed(formatted, result, timer.ElapsedMilliseconds);
        }

        private ScriptOptions GetOptions()
        {
            var options = ScriptOptions.Default
            .AddReferences(
                typeof(object).GetTypeInfo().Assembly,
                typeof(Assembly).GetTypeInfo().Assembly,
                typeof(Task).GetTypeInfo().Assembly,
                typeof(Enumerable).GetTypeInfo().Assembly,
                typeof(List<>).GetTypeInfo().Assembly,
                typeof(IGuild).GetTypeInfo().Assembly,
                typeof(SocketGuild).GetTypeInfo().Assembly,
                typeof(Startup).GetTypeInfo().Assembly
            )
            .AddImports(
                "System",
                "System.Reflection",
                "System.Threading.Tasks",
                "System.Linq",
                "System.Collections.Generic",
                "Discord",
                "Discord.WebSocket",
                "Dogey"
            );
            return options;
        }

        private string GetFormattedCode(string language, string rawmsg)
        {
            string code = rawmsg;

            if (code.StartsWith("```"))
                code = code.Substring(3, code.Length - 6);
            if (code.StartsWith(language))
                code = code.Substring(2, code.Length - 2);

            code = code.Trim();
            code = code.Replace(";\n", ";");
            code = code.Replace("; ", ";");
            code = code.Replace(";", ";\n");

            return code;
        }

        private Embed GetEmbed(string code, object result, long executeTime)
        {
            var builder = new EmbedBuilder()
                .WithColor(Color.DarkGreen)
                .AddField(x =>
                {
                    x.Name = "Script";
                    x.Value = $"```cs\n{code}```";
                })
                .AddField(x =>
                {
                    x.Name = $"Result<{result?.GetType().FullName ?? "null"}>";

                    if (result is Exception ex)
                        x.Value = ex.Message;
                    else
                        x.Value = result ?? "null";
                })
                .WithFooter(x =>
                {
                    x.Text = $"In {executeTime}ms";
                });
            return builder.Build();
        }
    }

    public class RoslynGlobals
    {
        public DogeyCommandContext Context { get; }
        public IServiceProvider Services { get; }

        public RoslynGlobals(DogeyCommandContext context, IServiceProvider services)
        {
            Context = context;
            Services = services;
        }
    }
}
