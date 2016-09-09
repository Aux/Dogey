using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Discord;
using Dogey.Models;
using System.Linq;
using Dogey.Tools;
using Dogey.Extensions;

namespace Dogey
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _cmds;

        public async Task Install(DiscordSocketClient c)
        {
            _client = c;
            _cmds = new CommandService();

            var map = new DependencyMap();
            map.Add(_client);
            
            await _cmds.LoadAssembly(Assembly.GetEntryAssembly(), map);
            Globals.CommandService = _cmds;
        }

        public async Task HandleCommand(IUserMessage msg)
        {
            if (msg.Author.IsBot)
                return;

            if (msg != null)
            {
                var channel = (msg.Channel as IGuildChannel);
                
                string prefix;
                if (channel?.Guild != null)
                    prefix = await channel.Guild.GetCustomPrefixAsync();
                else
                    prefix = Globals.Config.DefaultPrefix;
                
                int argPos = 0;
                if (msg.HasStringPrefix(prefix, ref argPos))
                {
                    var result = await _cmds.Execute(msg, argPos);

                    if (!result.IsSuccess)
                    {
                        if (result.Error != CommandError.UnknownCommand)
                            await msg.Channel.SendMessageAsync(result.ErrorReason);
                    } else {
                        DogeyConsole.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.Gray);

                        if (channel?.Guild == null)
                            DogeyConsole.Append($"[PM] ", ConsoleColor.Magenta);
                        else
                            DogeyConsole.Append($"[{channel.Guild.Name} #{channel.Name}] ", ConsoleColor.DarkGreen);

                        DogeyConsole.Append($"{msg.Author}: ", ConsoleColor.Green);
                        DogeyConsole.Append(msg.Content, ConsoleColor.White);
                    }
                }
            }
        }
    }
}