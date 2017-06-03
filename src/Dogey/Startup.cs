using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class Startup
    {
        public async Task<IServiceProvider> ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddDbContext<TagDatabase>(ServiceLifetime.Transient)
                .AddDbContext<ConfigDatabase>(ServiceLifetime.Transient)
                .AddDbContext<PatsDatabase>(ServiceLifetime.Transient)
                .AddDbContext<ScriptDatabase>(ServiceLifetime.Transient)
                .AddTransient<TagManager>()
                .AddSingleton<CommandManager>()
                .AddSingleton<RoslynManager>()
                .AddSingleton<Random>()
                .AddSingleton(Configuration.Load());
            
            // Discord
            await LoadDiscordAsync(services);

            // Google
            await LoadGoogleAsync(services);
            
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            return provider;
        }

        private async Task LoadDiscordAsync(IServiceCollection services)
        {
            var discord = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
            });

            var commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async,
                ThrowOnError = false
            });

            discord.Log += OnLogAsync;
            commands.Log += OnLogAsync;

            await discord.LoginAsync(TokenType.Bot, Configuration.Load().Token.Discord);
            await discord.StartAsync();

            services.AddSingleton(discord);
            services.AddSingleton(commands);
        }
        
        private Task LoadGoogleAsync(IServiceCollection services)
        {
            var config = Configuration.Load();

            var search = new CustomsearchService(new BaseClientService.Initializer()
            {
                ApiKey = config.CustomSearch.Token,
                MaxUrlLength = 256
            });

            var youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = config.Token.Google,
                MaxUrlLength = 256
            });

            services.AddSingleton(search);
            services.AddSingleton(youtube);
            return Task.CompletedTask;
        }

        private Task OnLogAsync(LogMessage msg)
            => PrettyConsole.LogAsync(msg.Severity, msg.Source, msg.Exception?.ToString() ?? msg.Message);
    }
}
