using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
        public async Task Help(IUserMessage msg)
        {
            if (Globals.Config.IsSelfbot) return;

            var modules = Globals.CommandService.Modules;
            var commands = Globals.CommandService.Commands;

            var helpmsg = new Dictionary<string, List<string>>();
            foreach (var c in commands)
            {
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
        public async Task Help(IUserMessage msg, string cmd)
        {
            await Task.Delay(1);
        }
    }
}
