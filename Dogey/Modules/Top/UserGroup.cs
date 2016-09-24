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
    public class UserGroup
    {
        private DiscordSocketClient _client;

        public UserGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("usertop")]
        [Description("List the available stats.")]
        [Example("usertop")]
        public async Task Usertop(IUserMessage msg)
        {
            await Task.Delay(1);
        }

        [Module("usertop"), Name("Top")]
        [RequireContext(ContextType.Guild)]
        public class SubCommands
        {
            private DiscordSocketClient _client;

            public SubCommands(DiscordSocketClient client)
            {
                _client = client;
            }

            [Command("words"), Alias("messages", "msgs")]
            [Description("Get this user's favorite words.")]
            [Example("usertop words @Billy")]
            public async Task Messages(IUserMessage msg, [Remainder]IUser user = null)
            {
                var u = user as IGuildUser ?? msg.Author as IGuildUser;
                var guild = (msg.Channel as IGuildChannel)?.Guild;

                using (var db = new DataContext())
                {
                    var top = db.MessageLogs.Where(x => x.GuildId == guild.Id && x.AuthorId == u.Id)
                                  .SelectMany(x => x.Content.ToLower().Split(' '))
                                  .Where(x => !string.IsNullOrWhiteSpace(x))
                                  .GroupBy(x => x).OrderByDescending(x => x.Count())
                                  .Take(10).Select(x => $"{x.Key}: {x.Count()}");

                    var message = new List<string>
                    {
                        $"Favorite Words for **{user}**",
                        "```xl",
                        string.Join("\n", top),
                        "```"
                    };

                    await msg.Channel.SendMessageAsync(string.Join("\n", message));
                }
            }

            [Command("commands"), Alias("cmds")]
            [Description("Get this user's favorite commands.")]
            [Example("usertop commands @John")]
            public async Task Commands(IUserMessage msg, [Remainder]IUser user = null)
            {
                var u = user as IGuildUser ?? msg.Author as IGuildUser;
                var guild = (msg.Channel as IGuildChannel)?.Guild;

                using (var db = new DataContext())
                {
                    var top = db.CommandLogs.Where(x => !x.Command.Contains(".") && x.GuildId == guild.Id && x.UserId == u.Id)
                                         .GroupBy(x => x.Command)
                                         .OrderByDescending(g => g.Count())
                                         .Take(10).Select(x => $"{x.Key}: {x.Count()}");

                    var message = new List<string>
                    {
                        $"Favorite Commands for **{user}**",
                        "```xl",
                        string.Join("\n", top),
                        "```"
                    };

                    await msg.Channel.SendMessageAsync(string.Join("\n", message));
                }
            }

            [Command("emojis"), Alias("emotes")]
            [Description("Get this user's favorite emojis.")]
            [Example("usertop emojis @Vox Aura")]
            public async Task Emojis(IUserMessage msg, [Remainder]IUser user = null)
            {
                await Task.Delay(1);
            }
        }
    }
}
