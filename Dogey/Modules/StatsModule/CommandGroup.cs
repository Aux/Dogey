using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Dogey.Enums;
using Dogey.Models;
using Dogey.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module, Name("Stats")]
    [RequireContext(ContextType.Guild)]
    public class CommandGroup
    {
        private DiscordSocketClient _client;

        public CommandGroup(DiscordSocketClient client)
        {
            _client = client;
        }
        
        [Command("stats")]
        public async Task Stats(IUserMessage msg)
        {
            var leaderboard = new List<string>();
            using (var db = new DataContext())
            {
                leaderboard = db.CommandLogs.Where(x => x.Action == CommandAction.Executed)
                                .GroupBy(x => x.Command)
                                .OrderByDescending(x => x.Count()).Take(5)
                                .Select(x => $"{x.Key}: {x.Count()}").ToList();
            }

            string reply = $"```xl\n{string.Join("\n", leaderboard)}```";
            await Utility.SendMessage(msg, reply);
        }

        [Module("stats"), Name("Stats")]
        public class SubCommands
        {
            [Command("executions")]
            [Alias("runs", "execs")]
            [Description("Get the total number of executions for this command.")]
            public async Task Executions(IUserMessage msg, string cmd)
            {
                await Task.Delay(1);
            }
        }
    }
}
