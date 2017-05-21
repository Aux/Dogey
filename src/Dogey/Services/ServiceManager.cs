using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.DependencyInjection;
using NTwitch.Rest;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class ServiceManager
    {
        private readonly DiscordSocketClient _client;

        public ServiceManager(DiscordSocketClient client)
        {
            _client = client;
            _client.Log += OnLogAsync;
        }

        public async Task StartAsync()
        {
            var provider = ConfigureServices();

            var twitch = provider.GetService<TwitchRestClient>();
            await twitch.LoginAsync(Configuration.Load().Token.Twitch);

            var handler = provider.GetService<CommandHandler>();
            await handler.StartAsync();
        }

        private IServiceProvider ConfigureServices()
        {
            var config = Configuration.Load();

            var services = new ServiceCollection()
                .AddDbContext<TagDatabase>()
                .AddDbContext<ConfigDatabase>()
                .AddDbContext<PatsDatabase>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<Random>()
                .AddSingleton(_client)
                .AddSingleton(config)
                .AddSingleton(new TwitchRestClient())
                .AddSingleton(new CommandService(new CommandServiceConfig()
                {
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Info,
                    ThrowOnError = false
                }))
                .AddSingleton(new CustomsearchService(new BaseClientService.Initializer()
                {
                    ApiKey = config.CustomSearch.Token,
                    MaxUrlLength = 256
                }))
                .AddSingleton(new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = config.Token.Google
                }));
            
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            provider.GetService<CommandHandler>();
            
            return provider;
        }

        private Task OnLogAsync(LogMessage msg)
            => PrettyConsole.LogAsync(msg.Severity, msg.Source, msg.Exception?.ToString() ?? msg.Message);
    }
}
