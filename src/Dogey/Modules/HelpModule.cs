using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        public Task HelpCommandAsync()
            => HelpAsync(Context);
        public Task HelpAsync(SocketCommandContext context)
        {
            return Task.CompletedTask;
        }

        [Command("help")]
        public Task HelpCommandAsync(string command)
            => HelpAsync(Context, command);
        public Task HelpAsync(SocketCommandContext context, string command)
        {
            return Task.CompletedTask;
        }
    }
}
