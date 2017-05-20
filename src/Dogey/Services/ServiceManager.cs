using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
            var services = new ServiceCollection()
                .AddDbContext<TagDatabase>()
                .AddDbContext<ConfigDatabase>()
                .AddDbContext<PatsDatabase>()
                .AddSingleton<CommandHandler>()
                .AddSingleton(_client)
                .AddSingleton(new TwitchRestClient())
                .AddSingleton(new CommandService(new CommandServiceConfig()
                {
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Info,
                    ThrowOnError = false
                }));
            
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            provider.GetService<CommandHandler>();
            
            return provider;
        }

        private Task OnLogAsync(LogMessage msg)
            => PrettyConsole.LogAsync(msg.Severity, msg.Source, msg.Exception?.ToString() ?? msg.Message);
    }
}
