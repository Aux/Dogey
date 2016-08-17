using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Commands
{
    [Module]
    [Description("Commands")]
    public class CommandBase
    {
        private DiscordSocketClient _client;

        public CommandBase(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("commands")]
        public async Task Commands(IMessage msg)
        {
            using (var c = new CommandContext())
            {
                await msg.Channel.SendMessageAsync(string.Join(", ", c.Commands.Select(x => x.Name)));
            }
        }
    }
}
