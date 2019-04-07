using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Config;
using Dogey.Databases;
using Dogey.Services;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Dogey
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(string[] args)
        {
            TryGenerateConfiguration();
            var builder = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddYamlFile("_config.yml");
            Configuration = builder.Build();
        }

        public static bool TryGenerateConfiguration()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "_config.yml");
            if (File.Exists(filePath)) return false;

            var serializer = new SerializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            var yaml = serializer.Serialize(new AppOptions(true));
            File.WriteAllText(filePath, yaml);
            return true;
        }

        public async Task RunAsync()
        {
            Colorful.Console.WriteAscii("Dogey", System.Drawing.Color.DarkGreen);

            var services = new ServiceCollection();
            ConfigureServices(services);
            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<LoggingService>().Start();
            provider.GetRequiredService<StartupService>().Start();
            provider.GetRequiredService<CommandHandlingService>().Start();
            provider.GetRequiredService<RssService>().Start();

            await Task.Delay(-1);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Clients
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 1000
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false,
                QuotationMarkAliasMap = null,
                LogLevel = LogSeverity.Verbose
            }))
            .AddSingleton(new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = Configuration["google:token"]
            }))
            .AddSingleton(new GitHubClient(new ProductHeaderValue("Dogey"))
            {
                Credentials = new Credentials(Configuration["github:token"])
            })

            // Internal
            .AddSingleton<StartupService>()
            .AddSingleton<LoggingService>()
            .AddSingleton<CommandHandlingService>()
            .AddSingleton<RssService>()
            .AddSingleton<ScriptHandlingService>()

            .AddTransient<PrefixService>()
            .AddTransient<ResponsiveService>()

            .AddTransient<ConfigDatabase>()
            .AddTransient<ConfigController>()
            .AddTransient<LogDatabase>()
            .AddTransient<LogController>()

            // Etc
            .AddLogging()
            .AddSingleton<Random>()
            .AddSingleton<HttpClient>()
            .AddSingleton(Configuration);
        }
    }
}
