using Discord;
using Discord.WebSocket;
using Dogey.Types;
using Dogey.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dogey
{
    public class Program
    {
        public static void Main(string[] args) => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _cmds;

        public async Task Start()
        {
            DogeyConsole.TitleCard("Dogey");
            
            if (!Globals.ConfigExists())
            {
                Globals.CreateConfig();
                Console.ReadKey();
                return;
            }
            else
            {
                Globals.Config = new Configuration().FromFile(@"data\configuration.json");
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info
            });
            _cmds = new CommandHandler();

            _client.Log += (l) => Task.Run(() => DogeyConsole.Log(l.Severity, l.Source, l.Message));
            _client.MessageReceived += MessageReceived;

            await _client.LoginAsync(TokenType.Bot, Globals.Config.Token.Discord);
            await _client.ConnectAsync();
            await _cmds.Install(_client);

            await Task.Delay(-1);
        }

        private async Task MessageReceived(IMessage m)
        {
            var msg = (m as IUserMessage) ?? null;

            if (msg != null)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
                var channel = (msg.Channel as ITextChannel) ?? null;

                DogeyConsole.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.Gray);

                if (guild == null)
                    DogeyConsole.Append($"[PM] ", ConsoleColor.Magenta);
                else
                    DogeyConsole.Append($"[{guild.Name} #{channel.Name}] ", ConsoleColor.DarkGreen);

                DogeyConsole.Append($"{msg.Author}: ", ConsoleColor.Green);
                DogeyConsole.Append(msg.Content, ConsoleColor.White);
                await _cmds.HandleCommand(msg);
            }
        }
    }
}