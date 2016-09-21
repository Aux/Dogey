using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Enums;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Info
{
    [Module, Name("Info")]
    [RequireContext(ContextType.Guild)]
    public class CommandGroup
    {
        private DiscordSocketClient _client;

        public CommandGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("commandinfo")]
        [Description("")]
        public async Task CommandInfo(IUserMessage msg, string name)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            
            using (var db = new DataContext())
            {
                var cmd = db.Commands.Where(x => x.GuildId == guild.Id && x.Name == name).FirstOrDefault();
                var created = db.CommandLogs.Where(x => x.Command == name && x.Action == CommandAction.Created).FirstOrDefault();
                
                if (cmd == null)
                {
                    await msg.Channel.SendMessageAsync($"`{name}` is not a valid command.");
                }
                else
                {
                    var infomsg = new List<string>
                    {
                        "```xl",
                        $"   Name: {cmd.Name} ({cmd.Id})",
                        $"   Desc: {cmd.Description}",
                        $"  Owner: {cmd.OwnerId}",
                        $"   Mode: {Enum.GetName(typeof(CommandType), cmd.Type)}",
                        $"Created: {created?.Timestamp.ToString("ddddd, MMM dd yyyy, hh:mm:ss tt") ?? "N/A"}",
                        $"   Uses: {db.CommandLogs.Count(x => x.Command == name && x.Action == CommandAction.Executed)}",
                        $"Changes: {db.CommandLogs.Count(x => x.Command == name && x.Action == CommandAction.Modified)}",
                        "```"
                    };

                    await msg.Channel.SendMessageAsync(string.Join("\n", infomsg));
                }
            }
        }

        [Module("commandinfo"), Name("Info")]
        public class SubCommands
        {
            private DiscordSocketClient _client;

            public SubCommands(DiscordSocketClient client)
            {
                _client = client;
            }

        }
    }
}
