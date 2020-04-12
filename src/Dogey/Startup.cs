using Discord.Commands;
using Discord.WebSocket;
using Dogey.Logging;
using Dogey.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory, "common"))
                .AddYamlFile("config.yml")
                .AddCommandLine(args);
            _config = builder.Build();
        }

        public async Task StartAsync()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var provider = services.BuildServiceProvider();

            var discord = provider.GetRequiredService<DiscordSocketClient>();
            await discord.LoginAsync(Discord.TokenType.Bot, _config["discord:token"]);
            await discord.StartAsync();

            var commands = provider.GetRequiredService<CommandService>();
            await commands.AddModulesAsync(Assembly.GetExecutingAssembly(), provider);

            provider.GetRequiredService<ILoggerFactory>().AddProvider(new BotLoggerProvider(_config));
            provider.GetRequiredService<LoggingService>().Start();
            provider.GetRequiredService<CommandHandlingService>().Start();

            await Task.Delay(-1);
        }

        private void ConfigureServices(ServiceCollection services)
        {
            services
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    MessageCacheSize = 50,
                    LogLevel = Discord.LogSeverity.Verbose
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    CaseSensitiveCommands = false,
                    IgnoreExtraArgs = false
                }))
                .AddSingleton(_config)
                .AddSingleton<LocaleService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<CommandHandlingService>()
                .AddDbContext<RootDatabase>()
                .AddLogging();
        }
    }
}
