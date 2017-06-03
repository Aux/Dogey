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
    public class RoslynManager
    {
        private readonly IServiceProvider _provider;

        public RoslynManager(IServiceProvider provider)
        {
            _provider = provider;
        }
        
        public ScriptOptions GetOptions()
        {
            var options = ScriptOptions.Default
            .AddReferences(
                typeof(string).GetTypeInfo().Assembly,
                typeof(Assembly).GetTypeInfo().Assembly,
                typeof(Task).GetTypeInfo().Assembly,
                typeof(Enumerable).GetTypeInfo().Assembly,
                typeof(List<>).GetTypeInfo().Assembly,
                typeof(IGuild).GetTypeInfo().Assembly,
                typeof(SocketGuild).GetTypeInfo().Assembly,
                typeof(Dogey.IEntity<>).GetTypeInfo().Assembly
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

        public Task<Embed> EvalAsync(DogeyCommandContext context, Script script)
            => EvalAsync(context, script.Content);

        public async Task<Embed> EvalAsync(DogeyCommandContext context, string content)
        {
            var _timer = new Stopwatch();
            _timer.Start();

            var cleancode = GetFormattedCode("cs", content);
            var options = GetOptions();
            object result;

            try
            {
                result = await CSharpScript.EvaluateAsync(cleancode, options, new RoslynGlobals(_provider, context));
            }
            catch (Exception ex)
            {
                result = ex;
            }
            _timer.Stop();

            return GetEmbed(cleancode, result, _timer.ElapsedMilliseconds);
        }
        
        public string GetFormattedCode(string language, string rawmsg)
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
        
        public Embed GetEmbed(string code, object result, long executeTime)
        {
            var builder = new EmbedBuilder();
            builder.Color = new Color(25, 128, 0);
            builder.AddField(x =>
            {
                x.Name = "Code";
                x.Value = $"```cs\n{code}```";
            });
            builder.AddField(x =>
            {
                x.Name = $"Result<{result?.GetType().FullName ?? "null"}>";

                if (result is Exception ex)
                    x.Value = ex.Message;
                else
                    x.Value = result ?? "null";
            });
            builder.WithFooter(x =>
            {
                x.Text = $"In {executeTime}ms";
            });

            return builder;
        }
    }

    public class RoslynGlobals
    {
        public readonly DogeyCommandContext Context;

        public RoslynGlobals(IServiceProvider provider, DogeyCommandContext context)
        {
            Context = context;
        }
    }
}
