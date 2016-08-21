using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Commands
{
    [Module]
    [Group("commands")]
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
            var channel = (msg.Channel as IGuildChannel) ?? null;

            if (channel != null)
            {
                var infomsg = new List<string>();
                using (var c = new CommandContext())
                {
                    infomsg.AddRange(new string[]
                    {
                        "```xl",
                        $"Server: {string.Join(", ", c.Commands.Where(x => x.GuildId == channel.Guild.Id).Select(x => x.Name))}",
                        "```"
                    });
                }
                await msg.Channel.SendMessageAsync(string.Join("\n", infomsg));
            }
        }

        [Command("create")]
        public async Task Create(IMessage msg, string name)
        {
            try
            {
                var channel = (msg.Channel as IGuildChannel) ?? null;

                if (channel != null)
                {
                    var cmd = new CustomCommand()
                    {
                        OwnerId = msg.Author.Id,
                        GuildId = channel.Guild.Id,
                        Name = name,
                        Messages = new List<string> { "testing creation " }
                    };

                    await cmd.CreateAsync(msg);
                    await msg.Channel.SendMessageAsync($"Created `{name}`.");
                }
            } catch (Exception ex)
            {
                DogeyConsole.Log(LogSeverity.Debug, "[]", ex.ToString());
            }
        }
    }
}
