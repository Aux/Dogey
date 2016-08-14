using Discord;
using Discord.WebSocket;
using Dogey.Utilities;
using System;
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

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose
            });
            _cmds = new CommandHandler();

            _client.Log += Log;
            _client.MessageReceived += MessageReceived;

            await _client.LoginAsync(TokenType.Bot, "MTgxODM4NTU0MDA2MTU5MzYw.Co4jrw.pmqTT50MzpxKigA1R1OpM7q1Jxw");
            await _client.ConnectAsync();
            await _cmds.Install(_client);

            await Task.Delay(-1);
        }

        private async Task Log(LogMessage l)
        {
            DogeyConsole.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            DogeyConsole.Append($"[{l.Severity}] ", ConsoleColor.Red);
            DogeyConsole.Append($"{l.Source}: ", ConsoleColor.DarkGreen);
            DogeyConsole.Append(l.Message, ConsoleColor.White);
            await Task.Delay(1);
        }

        private async Task MessageReceived(IMessage m)
        {
            var guild = (m.Channel as IGuildChannel)?.Guild ?? null;
            var channel = (m.Channel as ITextChannel) ?? null;

            DogeyConsole.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.Gray);

            if (guild == null)
                DogeyConsole.Append($"[PM] ", ConsoleColor.Magenta);
            else
                DogeyConsole.Append($"[{guild.Name} #{channel.Name}] ", ConsoleColor.DarkGreen);
            
            DogeyConsole.Append($"{m.Author}: ", ConsoleColor.Green);
            DogeyConsole.Append(m.Content, ConsoleColor.White);
            await _cmds.HandleCommand(m);
        }
    }
}