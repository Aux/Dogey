using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        public CommandService _service;
        
        public HelpModule(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        public Task HelpCommandAsync()
            => HelpAsync(Context);
        public Task HelpAsync(SocketCommandContext context)
        {
            string list = null;
            foreach (var m in _service.Modules)
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
