using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Help
{
    [Module]
    public class HelpModule
    {
        private CommandService _commands;
        private DiscordSocketClient _client;

        public HelpModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("help")]
        public async Task Help(IMessage msg, string cmd = null)
        {
            _commands = new CommandService();
            var user = msg.Author as IGuildUser;

            var map = new DependencyMap();
            map.Add(_client);
            map.Add(user);

            var modules = await _commands.LoadAssembly(System.Reflection.Assembly.GetEntryAssembly(), map);

            if (string.IsNullOrWhiteSpace(cmd))
            {
                var commands = new Dictionary<string, List<string>>();

                foreach (var module in modules.Where(x => !string.IsNullOrWhiteSpace(x.Description)))
                {
                    if (!string.IsNullOrWhiteSpace(module.Name))
                    {
                        //if (commands.Keys.Contains(module.Description))
                        //    commands[module.Description].Add(module.Name + "*");
                        //else
                        //    commands.Add(module.Description, new List<string> { module.Name + "*" });
                    } else
                    {
                        foreach (var cond in module.Preconditions)
                        {
                            //cond.CheckPermissions(msg, )
                        }

                        if (commands.Keys.Contains(module.Description))
                            commands[module.Description].AddRange(module.Commands.Select(x => x.Name));
                        else
                            commands.Add(module.Description, new List<string>(module.Commands.Select(x => x.Name)));
                    }
                }

                var helpmsg = new List<string>();
                foreach (var item in commands)
                {
                    helpmsg.Add($"**{item.Key}**: {string.Join(", ", item.Value.Select(x => $"`{x.ToLower()}`"))}");
                }

                await msg.Channel.SendMessageAsync("These are the commands you can use.\n" + string.Join("\n", helpmsg));
            }
            else
            {
                var command = FindCommand(modules, cmd);

                if (command != null)
                    await msg.Channel.SendMessageAsync(FormatCommand(command));
            }
        }

        public Command FindCommand(IEnumerable<Module> modules, string cmd)
        {
            Command command = null;
            foreach (var module in modules)
            {
                command = module.Commands.FirstOrDefault(x => x.Name.ToLower() == cmd);

                if (command != null)
                    break;
            }

            return command;
        }

        public string FormatCommand(Command cmd)
        {
            string helpmsg = $"`{cmd.Name.ToLower()}` ";
            
            foreach (var parameter in cmd.Parameters)
            {
                string pname = parameter.Name.ToLower();

                if (parameter.IsOptional)
                {
                    if (parameter.IsMultiple)
                        helpmsg += $"`[{pname}...]`";
                    else
                        helpmsg += $"`[{pname}]`";
                }
                else
                {
                    if (parameter.IsMultiple)
                        helpmsg += $"`<{pname}...>`";
                    else
                        helpmsg += $"`<{pname}>`";
                }
            }

            return $"{helpmsg}\n{cmd.Description}";
        }
    }
}
