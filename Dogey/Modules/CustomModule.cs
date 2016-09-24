using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
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
        [Example("create example This is an example.")]
        [Ratelimit(5, RateMeasure.Minutes)]
        public async Task Create(IUserMessage msg, string name, [Remainder]string description = null)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            string prefix = await guild.GetCustomPrefixAsync();

            var regex = new Regex("^[a-zA-Z0-9]*$");
            if (!regex.IsMatch(name))
            {
                await msg.Channel.SendMessageAsync("Invalid command name `{name}`.");
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
        [Description("Delete an existing custom command.")]
        [Example("delete example CONFIRM")]
        public async Task Delete(IUserMessage msg, string name, string CONFIRM)
        {

        }

        [Command("rename")]
        [Description("Rename an existing custom command.")]
        [Example("rename example test")]
        public async Task Rename(IUserMessage msg, string oldname, string newname)
        {

        }

        [Command("restore")]
        [Description("Restore a command that was deleted in the past 7 days.")]
        [Example("restore example")]
        public async Task Restore(IUserMessage msg, string name)
        {

        }

        [Command("commands"), Alias("cmds")]
        [Description("Get a list of all custom commands on this server.")]
        [Example("commands")]
        public async Task Commands(IUserMessage msg, int page = 1)
        {
            int p = page * 25 - 25;
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            string prefix = await guild.GetCustomPrefixAsync();
            string message;

            using (var db = new DataContext())
            {
                var cmds = db.Commands.Where(x => x.GuildId == guild.Id).Skip(p).Take(25).Select(x => x.Name);

                if (cmds.Count() > 0)
                    message = $"{guild.Name} Commands pg{p}```xl\n{string.Join(", ", cmds)}```";
                else
                    message = $"There are no commands for this server, add some with `{prefix}create <name> [desc]`.";
            }

            await msg.Channel.SendMessageAsync(message);
        }
        
        [Module("commands"), Name("Custom")]
        [RequireContext(ContextType.Guild)]
        public class SubCommands
        {
            private DiscordSocketClient _client;

            public SubCommands(DiscordSocketClient client)
            {
                _client = client;
            }
            
            [Command("recent")]
            [Description("Get a list of the most recently used commands in this server.")]
            [Example("commands recent 3 Vox Aura")]
            public async Task Recent(IUserMessage msg, int page = 1, [Remainder]IUser user = null)
            {
                int p = page * 10 - 10;
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                using (var db = new DataContext())
                {
                    IQueryable<string> cmds;
                    if (user != null)
                        cmds = db.CommandLogs.Where(x => !x.Command.Contains(".") && x.GuildId == guild.Id && x.UserId == user.Id)
                                             .OrderByDescending(x => x.Timestamp)
                                             .Skip(p).Take(10)
                                             .Select(x => $"[{x.Timestamp.ToUniversalTime().ToString("hh:mm:ss")}] {guild.GetUser(x.UserId)}: {x.Command} {x.Parameters}");
                    else
                        cmds = db.CommandLogs.Where(x => !x.Command.Contains(".") && x.GuildId == guild.Id)
                                             .OrderByDescending(x => x.Timestamp)
                                             .Skip(p).Take(10)
                                             .Select(x => $"[{x.Timestamp.ToUniversalTime().ToString("hh:mm:ss")}] {guild.GetUser(x.UserId)}: {x.Command} {x.Parameters}");

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
            [Description("Get a list of commands you own in this server.")]
            [Example("commands mine")]
            public async Task Mine(IUserMessage msg, int page = 1)
            {
                int p = page * 25 - 25;
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                string prefix = await guild.GetCustomPrefixAsync();
                string message;

                using (var db = new DataContext())
                {
                    var cmds = db.Commands.Where(x => x.GuildId == guild.Id && x.OwnerId == msg.Author.Id).Skip(p).Take(25).Select(x => x.Name);

                    if (cmds.Count() > 0)
                        message = $"{msg.Author} Commands pg{p}```xl\n{string.Join(", ", cmds)}```";
                    else
                        message = $"You have no commands on this server, add some with `{prefix}create <name> [desc]`.";
                }

                await msg.Channel.SendMessageAsync(message);
            }

            [Command("search"), Alias("find")]
            [Description("Search for a custom command.")]
            [Example("commands search example")]
            public async Task Search(IUserMessage msg, string query, int page = 1)
            {
                int p = page * 25 - 25;
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                string prefix = await guild.GetCustomPrefixAsync();
                string message;

                using (var db = new DataContext())
                {
                    var cmds = db.Commands.Where(x => x.GuildId == guild.Id && x.Name.Contains(query)).Skip(p).Take(25).Select(x => x.Name);

                    if (cmds.Count() > 0)
                        message = $"Commands like **{query}** pg{page}```xl\n{string.Join(", ", cmds)}```";
                    else
                        message = $"You have no commands on this server, add some with `{prefix}create <name> [desc]`.";
                }

                await msg.Channel.SendMessageAsync(message);
            }
        }
    }
}
