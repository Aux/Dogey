using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        public readonly CommandService _commands;
        
        public HelpModule(CommandService service)
        {
            _commands = service;
        }

        [Command("help")]
        public Task HelpCommandAsync()
            => HelpAsync(Context);
        public Task HelpAsync(SocketCommandContext context)
        {
            string list = null;
            foreach (var m in _commands.Modules)
            {
                list += $"\n**{m.Name}**: " + string.Join(", ", m.Commands.Select(x => x.Aliases.First()));
            }
            
            return context.Channel.SendMessageAsync(list);
        }

        [Command("help")]
        public Task HelpCommandAsync(string command)
            => HelpAsync(Context, command);
        public Task HelpAsync(SocketCommandContext context, string command)
        {
            return context.Channel.SendMessageAsync($"This would normally be the help command for `{command}`");
        }
    }
}
