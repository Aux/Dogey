using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module, Name("")]
    public class HelpModule
    {
        private DiscordSocketClient _client;

        public HelpModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("help")]
        [Description("What...")]
        [Example("help help")]
        public async Task Help(IUserMessage msg)
        {
            var modules = Globals.CommandService.Modules;
            var commands = Globals.CommandService.Commands;

            var helpmsg = new Dictionary<string, List<string>>();
            foreach (var c in commands)
            {
                var parameters = new List<string>();
                foreach (var p in c.Parameters)
                {
                    if (p.IsOptional)
                    {
                        if (p.IsRemainder || p.IsMultiple)
                            parameters.Add($"[{p.Name}...]");
                        else
                            parameters.Add($"[{p.Name}]");
                    }
                    else
                    {
                        if (p.IsRemainder || p.IsMultiple)
                            parameters.Add($"<{p.Name}...>");
                        else
                            parameters.Add($"<{p.Name}>");
                    }
                }
                c.Markdownify(string.Join(" ", parameters));

                if (!c.CheckPreconditions(msg).Result.IsSuccess)
                    continue;

                if (string.IsNullOrEmpty(c.Module.Name))
                    continue;

                if (string.IsNullOrEmpty(c.Module.Prefix))      // Root command
                {
                    if (helpmsg.Keys.Contains(c.Module.Name))
                        helpmsg[c.Module.Name].Add(c.Text);
                    else
                        helpmsg.Add(c.Module.Name, new List<string> { c.Text });
                }
                else                                            // Sub command
                {
                    if (helpmsg.Keys.Contains(c.Module.Name) || helpmsg.Keys.Contains(c.Module.Name + "*"))
                    {
                        int index = helpmsg[c.Module.Name].IndexOf(c.Module.Prefix);
                        if (index >= 0)
                        {
                            if (!helpmsg[c.Module.Name][index].EndsWith("*"))
                                helpmsg[c.Module.Name][index] += "*";
                        }
                    }
                    else
                    {
                        helpmsg.Add(c.Module.Name, new List<string> { c.Module.Prefix });
                    }
                }
            }

            string content = string.Join("\n", helpmsg.Select(x => $"{x.Key}: {string.Join(", ", x.Value)}"));

            if (string.IsNullOrWhiteSpace(content))
                await msg.Channel.SendMessageAsync("There are no commands available.");
            else
                await msg.Channel.SendMessageAsync($"```xl\n{content}```");
        }

        [Command("help")]
        public async Task Help(IUserMessage msg, [Remainder]string command)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            var modules = Globals.CommandService.Modules;
            var commands = Globals.CommandService.Commands;

            var keyword = command.ToLower().Split(' ');
            var cmd = commands.Where(x => x.Name == keyword[0] || x.Aliases.Contains(keyword[0]));

            if (cmd.Count() < 1)
            {
                await msg.Channel.SendMessageAsync($"I could not find a command like `{command}`.");
            } else
            if (cmd.Count() > 1)
            {
                await msg.Channel.SendMessageAsync($"The command `{command}` is ambiguous between ```xl\n{string.Join(", ", cmd.Select(x => x.Name))}```");
            } else
            {
                var c = cmd.FirstOrDefault();
                if (!c.CheckPreconditions(msg).Result.IsSuccess)
                    return;

                var infomsg = new List<string>();
                infomsg.Add("```erlang");
                infomsg.Add($"   Name: {c.Name} ({c.Module.Name})");
                infomsg.Add($"   Desc: {c.Remarks}");
                
                if (c.Aliases.Count > 1)
                    infomsg.Add($"Aliases: {string.Join(", ", c.Aliases)}");

                if (c.Parameters.Count > 0)
                {
                    var parameters = new List<string>();
                    foreach(var p in c.Parameters)
                    {
                        if (p.IsOptional)
                        {
                            if (p.IsRemainder || p.IsMultiple)
                                parameters.Add($"[{p.Name}...]");
                            else
                                parameters.Add($"[{p.Name}]");
                        }
                        else
                        {
                            if (p.IsRemainder || p.IsMultiple)
                                parameters.Add($"<{p.Name}...>");
                            else
                                parameters.Add($"<{p.Name}>");
                        }
                    }
                    infomsg.Add($"   Args: {string.Join(" ", parameters)}");
                    infomsg.Add($"Example:");
                    infomsg.Add($"       {await guild.GetCustomPrefixAsync()}{c.Summary}");
                }
                
                infomsg.Add("```");
                await msg.Channel.SendMessageAsync($"Help for the command **{c.Name}**{string.Join("\n", infomsg)}");
            }
        }
    }
}
