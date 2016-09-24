using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Top
{
    [Module, Name("Top")]
    [RequireContext(ContextType.Guild)]
    public class ServerGroup
    {
        private DiscordSocketClient _client;

        public ServerGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("servertop")]
        [Description("List the available stats.")]
        [Example("servertop")]
        public async Task Servertop(IUserMessage msg)
        {
            await Task.Delay(1);
        }

        [Module("servertop"), Name("Top")]
        [RequireContext(ContextType.Guild)]
        public class SubCommands
        {
            private DiscordSocketClient _client;

            public SubCommands(DiscordSocketClient client)
            {
                _client = client;
            }

            [Command("words"), Alias("messages", "msgs")]
            [Description("Get this servers's favorite words.")]
            [Example("servertop words")]
            public async Task Messages(IUserMessage msg)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                using (var db = new DataContext())
                {
                    var top = db.MessageLogs.Where(x => x.GuildId == guild.Id)
                                  .SelectMany(x => x.Content.ToLower().Split(' '))
                                  .Where(x => !string.IsNullOrWhiteSpace(x))
                                  .GroupBy(x => x).OrderByDescending(x => x.Count())
                                  .Take(10).Select(x => $"{x.Key}: {x.Count()}");

                    var message = new List<string>
                    {
                        $"Favorite Words for **{guild.Name}**",
                        "```xl",
                        string.Join("\n", top),
                        "```"
                    };

                    await msg.Channel.SendMessageAsync(string.Join("\n", message));
                }
            }

            [Command("commands"), Alias("cmds")]
            [Description("Get this servers's favorite commands.")]
            [Example("servertop commands")]
            public async Task Commands(IUserMessage msg)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                using (var db = new DataContext())
                {
                    var top = db.CommandLogs.Where(x => !x.Command.Contains(".") && x.GuildId == guild.Id)
                                         .GroupBy(x => x.Command)
                                         .OrderByDescending(g => g.Count())
                                         .Take(10).Select(x => $"{x.Key}: {x.Count()}");

                    var message = new List<string>
                    {
                        $"Favorite Commands for **{guild.Name}**",
                        "```xl",
                        string.Join("\n", top),
                        "```"
                    };

                    await msg.Channel.SendMessageAsync(string.Join("\n", message));
                }
            }

            [Command("emojis"), Alias("emotes")]
            [Description("Get this servers's favorite emojis.")]
            [Example("servertop emojis")]
            public async Task Emojis(IUserMessage msg)
            {
                await Task.Delay(1);
            }
        }
    }
}
