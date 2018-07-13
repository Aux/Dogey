using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Octokit;
using RestEase;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Dogey
{
    public class Startup
    {
        public CancellationTokenSource CancellationTokenSource { get; }
        public IConfiguration Configuration { get; }

        public Startup(string[] args)
        {
            TryGenerateConfiguration();
            CancellationTokenSource = new CancellationTokenSource();
            var builder = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddJsonFile("_configuration.json");
            Configuration = builder.Build();
        }

        public static async Task RunAsync(string[] args)
        {
            var startup = new Startup(args);
            await startup.RunAsync();
        }

        public async Task RunAsync()
        {
            Colorful.Console.WriteAscii("Dogey", System.Drawing.Color.DarkGreen);

            var services = new ServiceCollection();
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<LoggingService>();
            await provider.GetRequiredService<GuildBanService>().StartAsync(CancellationTokenSource.Token);
            await provider.GetRequiredService<StartupService>().StartAsync();
            await provider.GetRequiredService<CommandHandlingService>().StartAsync(CancellationTokenSource.Token);
            await provider.GetRequiredService<PointEarningService>().StartAsync(CancellationTokenSource.Token);

            await Task.Delay(-1);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services

                // Clients
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    DefaultRunMode = RunMode.Async,
                    CaseSensitiveCommands = false,
                    LogLevel = LogSeverity.Verbose
                }))
                .AddSingleton(new GitHubClient(new ProductHeaderValue("Dogey"))
                {
                    Credentials = new Credentials(Configuration["tokens:github"])
                })
                .AddSingleton(new CustomsearchService(new BaseClientService.Initializer()
                {
                    ApiKey = Configuration["google:token"],
                    MaxUrlLength = 256
                }))
                .AddSingleton(new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = Configuration["google:token"],
                    MaxUrlLength = 256
                }))

                // Databases
                .AddDbContext<RootDatabase>()
                .AddDbContext<PointsDatabase>()
                .AddTransient<RootController>()
                .AddTransient<PointsController>()

                // Api Interfaces
                .AddSingleton<HttpClient>()
                .AddSingleton<NasaApiService>()
                .AddSingleton(RestClient.For<INasaApi>(NasaApiService.GetClient()))
                .AddSingleton<WeatherApiService>()
                .AddSingleton(RestClient.For<IWeatherApi>(WeatherApiService.GetClient()))
                .AddSingleton<DogApiService>()
                .AddSingleton(RestClient.For<IDogApi>(DogApiService.GetClient()))
                .AddSingleton<NumbersApiService>()
                .AddSingleton(RestClient.For<INumbersApi>(NumbersApiService.GetClient()))
                .AddSingleton<TriviaApiService>()
                .AddSingleton(RestClient.For<ITriviaApi>(TriviaApiService.GetClient()))

                // Background
                .AddSingleton<GuildBanService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<PointEarningService>()

                // Etc
                .AddLogging()
                .AddTransient<ResponsiveService>()
                .AddTransient<StartupService>()
                .AddTransient<RoslynService>()
                .AddSingleton<RatelimitService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<Random>()
                .AddSingleton(CancellationTokenSource)
                .AddSingleton(Configuration);
        }

        public static bool TryGenerateConfiguration()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "_configuration.json");
            if (File.Exists(filePath)) return false;

            var json = JsonConvert.SerializeObject(new AppSettings(true), Formatting.Indented);
            File.WriteAllText(filePath, json);
            return true;
        }
    }
}
