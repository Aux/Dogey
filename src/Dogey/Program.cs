using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Dogey
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _discord;
        private ServiceManager _manager;

        public async Task Start()
        {
            PrettyConsole.NewLine("===   Dogey   ===");
            PrettyConsole.NewLine();

            Configuration.EnsureExists();

            _discord = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 1000
            });

            _discord.Log += (l)
                => Task.Run(()
                => PrettyConsole.Log(l.Severity, l.Source, l.Exception?.ToString() ?? l.Message));
            
            await _discord.LoginAsync(TokenType.Bot, Configuration.Load().Token.Discord);
            await _discord.StartAsync();

            _manager = new ServiceManager(_discord);
            await _manager.InitializeAsync();

            await Task.Delay(-1);
        }
    }
}