using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Eval
{
    [Group("scripts"), Name("Scripts")]
    [Summary("Search and view available scripts.")]
    [RequireOwner]
    public class ScriptsModule : ModuleBase<SocketCommandContext>
    {
        private readonly ScriptDatabase _db;

        public ScriptsModule(IServiceProvider provider)
        {
            _db = provider.GetService<ScriptDatabase>();
        }
        
        [Command]
        [Summary("")]
        public Task ScriptsAsync()
            => ScriptsAsync(Context.User);
        
        [Command]
        [Summary("")]
        public async Task ScriptsAsync([Remainder]SocketUser user)
        {
            var scripts = await _db.GetScriptsAsync(user.Id);

            if (scripts.Count() == 0)
            {
                await ReplyAsync("This user has no scripts yet!");
                return;
            }

            var builder = new EmbedBuilder();

            builder.ThumbnailUrl = user.GetAvatarUrl();
            builder.Title = $"Scripts for {user}";
            builder.Description = string.Join(", ", scripts.SelectMany(x => x.Aliases));

            await ReplyAsync("", embed: builder);
        }
    }
}
