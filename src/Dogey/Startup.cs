using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
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
            provider.GetRequiredService<PointsService>();
            provider.GetRequiredService<CommandHandler>();

            await Task.Delay(-1);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services
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
                .AddDbContext<RootDatabase>()
                .AddDbContext<PointsDatabase>()
                .AddTransient<RootController>()
                .AddTransient<PointsController>()
                .AddTransient<RoslynService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<GuildBanService>()
                .AddSingleton<StartupService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<Random>()
                .AddSingleton(Configuration);
        }
    }
}
