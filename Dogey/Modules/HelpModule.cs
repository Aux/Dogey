using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Modules.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Help
{
    [Module, Name("")]
    public class HelpModule
    {
        private CommandService _commands;
        private DiscordSocketClient _client;

        public HelpModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("help")]
        public async Task Help(IUserMessage msg, [Remainder]string cmd = null)
        {
            if (Globals.Config.IsSelfBot) return;
            
            var service = new CommandService();
            var map = new DependencyMap();
            map.Add(_client);
            
            var modules = await service.LoadAssembly(System.Reflection.Assembly.GetEntryAssembly(), map);
            var commands = modules.SelectMany(x => x.Commands);
            
            string helpmsg;
            if (string.IsNullOrWhiteSpace(cmd))
                helpmsg = GetAllHelp(msg, commands);
            else
                helpmsg = GetCmdHelp(msg, commands, cmd);
            
            await msg.Channel.SendMessageAsync($"These are the commands you can use.```xl\n{helpmsg}```");
        }

        private string GetAllHelp(IUserMessage msg, IEnumerable<Command> commands)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            var helpmsg = new Dictionary<string, string>();
            
            foreach(var c in commands)
            {
                if (!c.CheckPreconditions(msg).Result.IsSuccess)
                    continue;
                
                if (string.IsNullOrEmpty(c.Module.Name))
                    continue;
                
                if (!string.IsNullOrWhiteSpace(c.Module.Prefix))
                {
                    if (helpmsg.Values.Contains($"{c.Module.Prefix.ToLower()}*"))
                        continue;
                    
                    if (!helpmsg.Keys.Contains(c.Module.Name))
                        helpmsg.Add(c.Module.Name, c.Module.Prefix.ToLower() + "*");
                    else
                        helpmsg[c.Module.Name] += $", {c.Module.Prefix.ToLower()}*";
                } else
                {
                    if (!helpmsg.Keys.Contains(c.Module.Name))
                        helpmsg.Add(c.Module.Name, c.Name.ToLower());
                    else
                        helpmsg[c.Module.Name] += $", {c.Name.ToLower()}";
                }
            }
            
            if (guild != null)
            using (var c = new CommandContext())
                helpmsg.Add("Custom", string.Join(", ", c.Commands.Where(x => x.GuildId == guild.Id).Select(x => x.Name)));
            
            return string.Join("\n", helpmsg.Select(x => $"{x.Key}: {x.Value}"));
        }

        private string GetCmdHelp(IUserMessage msg, IEnumerable<Command> commands, string cmd)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            var helpmsg = new Dictionary<string, string>();

            var command = commands.FirstOrDefault();
            return "";
        }
    }
}
