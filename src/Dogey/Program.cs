using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using Discord.Commands;
using Discord;
using Discord.WebSocket;
using Octokit;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace Dogey
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        private IConfigurationRoot _config;

        public async Task StartAsync()
        {
            PrettyConsole.NewLine($"Dogey v{AppHelper.Version}");
            PrettyConsole.NewLine();

            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("_configuration.json");
            _config = builder.Build();

            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 100
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Async,
                    LogLevel = LogSeverity.Verbose
                }))
                .AddSingleton(new GitHubClient(new ProductHeaderValue("Dogey"))
                {
                    Credentials = new Credentials(_config["tokens:github"])
                })
                .AddSingleton(new CustomsearchService(new BaseClientService.Initializer()
                {
                    ApiKey = _config["tokens:google"],
                    MaxUrlLength = 256
                }))
                .AddSingleton(new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = _config["tokens:google"],
                    MaxUrlLength = 256
                }))
                .AddDbContext<TagDatabase>(ServiceLifetime.Transient)
                .AddDbContext<ConfigDatabase>(ServiceLifetime.Transient)
                .AddDbContext<PatsDatabase>(ServiceLifetime.Transient)
                .AddDbContext<ScriptDatabase>(ServiceLifetime.Transient)
                .AddDbContext<PointsDatabase>(ServiceLifetime.Transient)
                .AddDbContext<DogDatabase>(ServiceLifetime.Transient)
                .AddTransient<TagManager>()
                .AddTransient<PointsManager>()
                .AddTransient<DogManager>()
                .AddTransient<ConfigManager>()
                .AddTransient<ScriptManager>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<RoslynManager>()
                .AddSingleton<ChannelWatcher>()
                .AddSingleton<LoggingService>()
                .AddSingleton<StartupService>()
                .AddSingleton<PointsService>()
                .AddSingleton<TagService>()
                .AddSingleton<Random>()
                .AddSingleton(_config);

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<LoggingService>();

            await provider.GetRequiredService<StartupService>().StartAsync();

            provider.GetRequiredService<CommandHandler>();
            provider.GetRequiredService<PointsService>();
            provider.GetRequiredService<ChannelWatcher>();
            provider.GetRequiredService<TagService>();

            await Task.Delay(-1);
        }
    }
}