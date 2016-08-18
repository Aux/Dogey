using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Utilities;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class CommandHandler
    {
        private CommandService commands;
        private DiscordSocketClient client;
        private ISelfUser self;

        public async Task Install(DiscordSocketClient c)
        {
            client = c;
            commands = new CommandService();
            self = await client.GetCurrentUserAsync();

            // Set up the dependency map
            var map = new DependencyMap();
            map.Add(client);
            map.Add(self);

            // Load modules
            await commands.LoadAssembly(Assembly.GetEntryAssembly(), map);
        }

        public async Task HandleCommand(IMessage msg)
        {
            if (msg.Author.IsBot)
                return;
            
            int argPos = 0;
            if (msg.HasCharPrefix(Globals.Config.Prefix, ref argPos))
            {
                var result = await commands.Execute(msg, argPos);
                if (!result.IsSuccess && !result.ErrorReason.Contains("Unknown"))
                {
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
    }
}