using Discord;
using Discord.WebSocket;
using Dogey.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dogey
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _handler;

        public async Task Start()
        {
            PrettyConsole.NewLine("===   Dogey   ===");
            PrettyConsole.NewLine();

            EnsureConfigExists();
            await EnsureDatabaseExists();
            
            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info
            });

            _client.Log += (l)
                => Task.Run(()
                => PrettyConsole.Log(l.Severity, l.Source, l.Exception?.ToString() ?? l.Message));

            await _client.LoginAsync(TokenType.Bot, Configuration.Load().Token.Discord);
            await _client.ConnectAsync();

            _handler = new CommandHandler();
            await _handler.Install(_client);

            await Task.Delay(-1);
        }

        public static void EnsureConfigExists()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "data")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "data"));

            string loc = Path.Combine(AppContext.BaseDirectory, "data/configuration.json");

            if (!File.Exists(loc))
            {
                var config = new Configuration();
                config.Save();

                PrettyConsole.Log(LogSeverity.Error,
                    "[Startup]",
                    "The configuration file has been created at 'data\\configuration.json', " +
                    "please enter your information and restart Dogey.");
                PrettyConsole.NewLine("Press any key to continue...");

                Console.ReadKey();
                Environment.Exit(0);
            }
            PrettyConsole.Log(LogSeverity.Info, "Dogey", "Configuration Loaded");
        }

        public async Task EnsureDatabaseExists()
        {
            using (var db = new LogContext())
                await db.Database.EnsureCreatedAsync();
            PrettyConsole.Log(LogSeverity.Info, "Dogey", "Log Database Loaded");
        }
    }
}
