using Discord;
using Discord.WebSocket;
using NTwitch;
using NTwitch.Rest;
using System.Threading.Tasks;

namespace Dogey
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _discord;
        private TwitchRestClient _twitch;
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
            _twitch = new TwitchRestClient(new TwitchRestConfig()
            {
                LogLevel = LogLevel.Info
            });

            _discord.Log += (l)
                => Task.Run(()
                => PrettyConsole.Log(l.Severity, l.Source, l.Exception?.ToString() ?? l.Message));
            _twitch.Log += (l)
                => Task.Run(()
                => PrettyConsole.Log(l.Level, l.Source, l.Exception?.ToString() ?? l.Message));

            await _twitch.LoginAsync(AuthMode.Oauth, Configuration.Load().Token.Twitch);
            await _discord.LoginAsync(TokenType.Bot, Configuration.Load().Token.Discord);
            await _discord.StartAsync();

            _manager = new ServiceManager(_discord, _twitch);
            await _manager.InitializeAsync();

            await Task.Delay(-1);
        }
    }
}