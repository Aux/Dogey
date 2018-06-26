using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Octokit;
using RestEase;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(string[] args)
        {
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
            var services = new ServiceCollection();
            ConfigureServices(services);

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<LoggingService>();
            provider.GetRequiredService<GuildBanService>();
            await provider.GetRequiredService<StartupService>().StartAsync();
            //provider.GetRequiredService<PointsService>();
            provider.GetRequiredService<CommandHandler>();

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
                .AddSingleton<WeatherApiService>()
                .AddSingleton(RestClient.For<IWeatherApi>(WeatherApiService.GetClient()))
                .AddSingleton<DogApiService>()
                .AddSingleton(RestClient.For<IDogApi>(DogApiService.GetClient()))
                .AddSingleton<NumbersApiService>()
                .AddSingleton(RestClient.For<INumbersApi>(NumbersApiService.GetClient()))

                // Etc
                .AddTransient<RoslynService>()
                .AddSingleton<RatelimitService>()
                .AddSingleton<PointsService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<GuildBanService>()
                .AddSingleton<StartupService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<Random>()
                .AddSingleton(Configuration);
        }
    }
}
