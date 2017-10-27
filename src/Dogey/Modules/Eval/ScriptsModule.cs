using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Eval
{
    [RequireOwner]
    [Group("scripts"), Name("Scripts")]
    public class ScriptsModule : DogeyModuleBase
    {
        private readonly ScriptManager _manager;

        public ScriptsModule(ScriptManager manager)
        {
            _manager = manager;
        }
        
        [Command]
        [Summary("")]
        public Task ScriptsAsync()
            => ScriptsAsync(Context.User);
        
        [Command]
        [Summary("")]
        public async Task ScriptsAsync([Remainder]SocketUser user)
        {
            var scripts = await _manager.GetScriptsAsync(user.Id);

            if (scripts.Count() == 0)
            {
                await ReplyAsync("This user has no scripts yet!");
                return;
            }

            var builder = new EmbedBuilder();

            builder.ThumbnailUrl = user.GetAvatarUrl();
            builder.Title = $"Scripts for {user}";
            builder.Description = string.Join(", ", scripts.SelectMany(x => x.Aliases));

            await ReplyAsync(builder);
        }
    }
}
