using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey.Services
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _cmds;

        public async Task Install(DiscordSocketClient c)
        {
            _client = c;                                            
            _cmds = new CommandService();                         

            await _cmds.AddModulesAsync(Assembly.GetEntryAssembly()); 

            _client.MessageReceived += HandleCommand;         
        }

        private async Task HandleCommand(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg != null)                                      
            {
                var context = new CommandContext(_client, msg);    

                int argPos = 0;                                 
                if (msg.HasStringPrefix("!", ref argPos) ||
                    msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
                {                                          
                    var result = await _cmds.ExecuteAsync(context, argPos);

                    if (!result.IsSuccess)                     
                        await context.Channel.SendMessageAsync(result.ToString());
                }
            }
        }
    }
}
