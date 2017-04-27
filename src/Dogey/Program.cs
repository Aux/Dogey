using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Dogey
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private ServiceManager _manager;

        public async Task Start()
        {
            PrettyConsole.NewLine("===   Dogey   ===");
            PrettyConsole.NewLine();

            Configuration.EnsureExists();

            _client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 1000
            });

            _client.Log += OnLogAsync;
            
            await _client.LoginAsync(TokenType.Bot, Configuration.Load().Token.Discord);
            await _client.StartAsync();

            _manager = new ServiceManager(_client);
            await _manager.InitializeAsync();

            await Task.Delay(-1);
        }

        private Task OnLogAsync(LogMessage msg)
            => PrettyConsole.LogAsync(msg.Severity, msg.Source, msg.Exception?.ToString() ?? msg.Message);
    }
}