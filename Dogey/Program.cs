using Discord;
using Discord.WebSocket;
using Dogey.Tools;
using Dogey.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _cmds;

        public async Task Start()
        {
            DogeyConsole.TitleCard("Dogey");

            if (!Globals.ConfigExists())
                Globals.CreateConfig();
            else
                Globals.LoadConfig();

            _cmds = new CommandHandler();
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info
            });
            
            _client.MessageReceived += OnMessageReceived;
            _client.Log += (l)
                => Task.Run(()
                => DogeyConsole.Log(l.Severity, l.Source, l.Exception?.ToString() ?? l.Message));

            await _client.LoginAsync(TokenType.Bot, Globals.Config.Token.Discord);
            await _client.ConnectAsync();
            await _cmds.Install(_client);

            await Task.Delay(-1);
        }

        private async Task OnMessageReceived(IMessage arg)
        {
            var msg = arg as IUserMessage;

            if (msg != null)
            {
                await _cmds.HandleCommand(msg);

                var channel = msg.Channel as ITextChannel;
                if (msg.Author.Id == (await _client.GetCurrentUserAsync()).Id)
                {
                    DogeyConsole.Log(msg);
                }
            }
        }
    }
}
