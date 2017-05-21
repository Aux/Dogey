using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.Eval
{
    [Group("scripts"), Name("Scripts")]
    [Summary("Search and view available scripts.")]
    public class ScriptsModule : ModuleBase<SocketCommandContext>
    {
        private readonly ScriptDatabase _db;

        public ScriptsModule(IServiceProvider provider)
        {
            _db = provider.GetService<ScriptDatabase>();
        }
        
        [Command]
        [Summary("")]
        public async Task ScriptsAsync()
        {
            await ReplyAsync(":thumbsup:");
        }

        [Command]
        [Summary("")]
        public async Task ScriptsAsync([Remainder]SocketUser user)
        {
            await ReplyAsync(":thumbsup:");
        }
    }
}
