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
    public class ChannelGroup
    {
        private DiscordSocketClient _client;

        public ChannelGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("channeltop")]
        [Description("List the available stats.")]
        [Example("channeltop")]
        public async Task Channeltop(IUserMessage msg)
        {
            await Task.Delay(1);
        }

        [Module("channeltop"), Name("Top")]
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
            [Example("channeltop words")]
            public async Task Messages(IUserMessage msg, [Remainder]IChannel channel = null)
            {
                var c = channel as ITextChannel ?? msg.Channel as ITextChannel;
                var guild = c.Guild;

                using (var db = new DataContext())
                {
                    var top = db.MessageLogs.Where(x => x.GuildId == guild.Id && x.ChannelId == c.Id)
                                  .SelectMany(x => x.Content.ToLower().Split(' '))
                                  .Where(x => !string.IsNullOrWhiteSpace(x))
                                  .GroupBy(x => x).OrderByDescending(x => x.Count())
                                  .Take(10).Select(x => $"{x.Key}: {x.Count()}");

                    var message = new List<string>
                    {
                        $"Favorite Words for {c.Mention}",
                        "```xl",
                        string.Join("\n", top),
                        "```"
                    };

                    await msg.Channel.SendMessageAsync(string.Join("\n", message));
                }
            }

            [Command("commands"), Alias("cmds")]
            [Description("Get this servers's favorite commands.")]
            [Example("channeltop commands")]
            public async Task Commands(IUserMessage msg, [Remainder]IChannel channel = null)
            {
                var c = channel as ITextChannel ?? msg.Channel as ITextChannel;
                var guild = c.Guild;

                using (var db = new DataContext())
                {
                    var top = db.CommandLogs.Where(x => !x.Command.Contains(".") && x.GuildId == guild.Id && x.ChannelId == c.Id)
                                         .GroupBy(x => x.Command)
                                         .OrderByDescending(g => g.Count())
                                         .Take(10).Select(x => $"{x.Key}: {x.Count()}");

                    var message = new List<string>
                    {
                        $"Favorite Commands for {c.Mention}",
                        "```xl",
                        string.Join("\n", top),
                        "```"
                    };

                    await msg.Channel.SendMessageAsync(string.Join("\n", message));
                }
            }

            [Command("emojis"), Alias("emotes")]
            [Description("Get this servers's favorite emojis.")]
            [Example("channeltop emojis")]
            public async Task Emojis(IUserMessage msg, [Remainder]IChannel channel = null)
            {
                await Task.Delay(1);
            }
        }
    }
}
