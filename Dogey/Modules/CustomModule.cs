using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Enums;
using Dogey.Extensions;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module, Name("Custom")]
    [RequireContext(ContextType.Guild)]
    public class CustomModule
    {
        private DiscordSocketClient _client;

        public CustomModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("create")]
        [Description("Create a new custom command.")]
        public async Task Create(IUserMessage msg, string name, [Remainder]string description = null)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            string prefix = await guild.GetCustomPrefixAsync();

            var regex = new Regex("^[a-zA-Z0-9]*$");
            if (!regex.IsMatch(name))
            {
                await msg.Channel.SendMessageAsync("Command names cannot be empty.");
                return;
            }

            using (var db = new DataContext())
            {
                int cmds = db.Commands.Where(x => x.GuildId == guild.Id && x.Name == name).Count();

                if (cmds == 0)
                {
                    var cmd = new CustomCommand(msg, name.ToLower(), description);
                    db.Commands.Add(cmd);

                    string[] parts = msg.Content.Replace(prefix, "").Split(new[] { ' ' }, 2);
                    db.CommandLogs.Add(new CommandLog()
                    {
                        Timestamp = DateTime.UtcNow,
                        GuildId = guild.Id,
                        ChannelId = msg.Channel.Id,
                        UserId = msg.Author.Id,
                        Command = parts[0],
                        Parameters = parts[1],
                        Action = CommandAction.Created
                    });
                    
                    await db.SaveChangesAsync();
                    await msg.Channel.SendMessageAsync($":thumbsup: You can now add tags with `{prefix}{cmd.Name}.add <tag> <message>`.");
                } else
                {
                    await msg.Channel.SendMessageAsync($"The command `{name}` already exists.");
                }
            }
        }

        [Command("delete")]
        public async Task Delete(IUserMessage msg, string name, string CONFIRM)
        {

        }

        [Command("rename")]
        public async Task Rename(IUserMessage msg, string oldname, string newname)
        {

        }

        [Command("commands")]
        public async Task Commands(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            string prefix = await guild.GetCustomPrefixAsync();
            string message;

            using (var db = new DataContext())
            {
                var cmds = db.Commands.Where(x => x.GuildId == guild.Id).Select(x => x.Name);

                if (cmds.Count() > 0)
                    message = $"```xl\n{string.Join(", ", cmds)}```";
                else
                    message = $"There are no commands for this server, add some with `{prefix}create <name> [desc]`.";
            }

            await msg.Channel.SendMessageAsync(message);
        }
        
        [Module("commands"), Name("Custom")]
        public class SubCommands
        {
            private DiscordSocketClient _client;

            public SubCommands(DiscordSocketClient client)
            {
                _client = client;
            }

            [Command("top")]
            public async Task Top(IUserMessage msg, int page = 1)
            {
                int p = page * 5 - 5;
                using (var db = new DataContext())
                {
                    var cmds = db.CommandLogs.Where(x => !x.Command.Contains("."))
                                             .GroupBy(x => x.Command)
                                             .OrderByDescending(g => g.Count())
                                             .Skip(p).Take(5)
                                             .Select(x => $"{x.Key}: {x.Count()}");

                    var message = new List<string>
                    {
                        $"Top Commands pg{page}",
                        "```xl",
                        string.Join("\n", cmds),
                        "```"
                    };

                    await msg.Channel.SendMessageAsync(string.Join("\n", message));
                }
            }

            [Command("recent")]
            public async Task Recent(IUserMessage msg, int page = 1)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                int p = page * 10 - 10;
                using (var db = new DataContext())
                {
                    var cmds = db.CommandLogs.Where(x => !x.Command.Contains(".") && x.GuildId == guild.Id)
                                             .OrderByDescending(x => x.Timestamp)
                                             .Skip(p).Take(10)
                                             .Select(x => $"{guild.GetUser(x.UserId)}: {x.Command} {x.Parameters}");

                    var message = new List<string>
                    {
                        $"Recent Commands pg{page}",
                        "```xl",
                        string.Join("\n", cmds),
                        "```"
                    };

                    await msg.Channel.SendMessageAsync(string.Join("\n", message));
                }
            }

            [Command("mine")]
            public async Task Mine(IUserMessage msg, int page = 1)
            {

            }
        }
    }
}
