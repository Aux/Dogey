using Discord;
using Discord.Commands;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Dogey.Modules.Eval
{
    [Group("eval"), Name("Eval")]
    [Summary("Evaluate csharp scripts.")]
    public class EvalModule : ModuleBase<SocketCommandContext>
    {
        private readonly RoslynManager _roslyn;
        private Stopwatch _timer = new Stopwatch();

        public EvalModule(IServiceProvider provider)
        {
            _roslyn = provider.GetService<RoslynManager>();
            _timer.Start();
        }
        
        [Command(RunMode = RunMode.Async)]
        public async Task EvalAsync([Remainder]string code)
        {
            var options = _roslyn.GetOptions();
            string cleancode = _roslyn.GetFormattedCode("cs", code);
            string reply, type;

            try
            {
                var result = await CSharpScript.EvaluateAsync(cleancode, options, Context);
                type = result.GetType().Name;
                reply = result.ToString();
            }
            catch (Exception ex)
            {
                type = ex.GetType().Name;
                reply = ex.Message;
            }
            _timer.Stop();

            var embed = GetEmbed(cleancode, reply, type);
            await ReplyAsync("", embed: embed);
        }

        private EmbedBuilder GetEmbed(string code, string result, string resultType)
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
                x.Name = $"Result<{resultType}>";
                x.Value = result;
            });
            builder.WithFooter(x =>
            {
                x.Text = $"In {_timer.ElapsedMilliseconds}ms";
            });

            return builder;
        }
    }
}
