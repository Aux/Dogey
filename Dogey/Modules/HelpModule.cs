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

            var channel = (msg.Channel as IGuildChannel) ?? null;
            var user = msg.Author as IGuildUser;

            var map = new DependencyMap();
            map.Add(_client);
            map.Add(user);

            var helpmsg = new List<string>();
            var modules = await _commands.LoadAssembly(System.Reflection.Assembly.GetEntryAssembly(), map);

            if (string.IsNullOrWhiteSpace(cmd))
            {
                var commands = new Dictionary<string, List<string>>();

                foreach (var module in modules.Where(x => !string.IsNullOrWhiteSpace(x.Description)))
                {
                    if (!string.IsNullOrWhiteSpace(module.Prefix))
                    {
                        if (commands.Keys.Contains(module.Description))
                            commands[module.Description].Add(module.Prefix + "*");
                        else
                            commands.Add(module.Description, new List<string> { module.Name + "*" });
                    } else
                    {
                        if (commands.Keys.Contains(module.Description))
                            commands[module.Description].AddRange(module.Commands.Select(x => x.Name));
                        else
                            commands.Add(module.Description, new List<string>(module.Commands.Select(x => x.Name)));
                    }
                }

                using (var c = new CommandContext())
                {
                    commands.Add("Custom", c.Commands.Where(x => x.GuildId == channel.Guild.Id).Select(x => x.Name).ToList());
                }

                foreach (var item in commands)
                {
                    helpmsg.Add($"{item.Key}: {string.Join(", ", item.Value.Select(x => x.ToLower()))}");
                }

                await msg.Channel.SendMessageAsync($"These are the commands you can use.\n```xl\n{string.Join("\n", helpmsg)}```");
            }
            else
            {
                foreach(var module in modules.Where(x => x.Prefix == cmd))
                    foreach (var subcmd in module.Commands)
                    {
                        helpmsg.Add(subcmd.Name.ToLower());
                    }

                if (helpmsg.Count() > 1)
                {
                    await msg.Channel.SendMessageAsync($"These are the sub-commands for **{cmd}**.\n```xl\n{string.Join(", ", helpmsg)}```");
                } else
                {
                    var command = FindCommand(modules, cmd);

                    if (command != null)
                        await msg.Channel.SendMessageAsync(FormatCommand(command));
                }
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
