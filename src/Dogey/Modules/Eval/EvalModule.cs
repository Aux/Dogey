using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.Eval
{
    [Group("eval"), Name("Eval")]
    [Summary("Evaluate csharp scripts.")]
    [RequireOwner]
    public class EvalModule : ModuleBase<SocketCommandContext>
    {
        private readonly RoslynManager _roslyn;

        public EvalModule(IServiceProvider provider)
        {
            _roslyn = provider.GetService<RoslynManager>();
        }
        
        [Command(RunMode = RunMode.Async)]
        public async Task EvalAsync([Remainder]string code)
        {
            var result = await _roslyn.EvalAsync(Context, code);
            await ReplyAsync("", embed: result);
        }
    }
}
