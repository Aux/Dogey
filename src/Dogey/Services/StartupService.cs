using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Dogey.Config;
using Dogey.Logging;
using Dogey.Modules.Readers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Dogey.Services
{
    public class StartupService : Service
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IServiceProvider _provider;
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        public StartupService(
            ILoggerFactory loggerFactory,
            IServiceProvider provider,
            IConfiguration config,
            DiscordSocketClient discord,
            CommandService commands)
        {
            _loggerFactory = loggerFactory;
            _provider = provider;
            _config = config;
            _discord = discord;
            _commands = commands;
        }

        public override void Start()
            => StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        public override void Stop()
            => throw new NotImplementedException();
        
        public async Task StartAsync()
        {
            var fileLoggerOptions = new LoggingOptions();
            _config.Bind("filelogger", fileLoggerOptions);
            _loggerFactory.AddProvider(new DogeyLoggerProvider(fileLoggerOptions));

            await _discord.LoginAsync(TokenType.Bot, _config["discord:token"]);
            await _discord.StartAsync();

            var guildReader = new GuildTypeReader();
            _commands.AddTypeReader<IGuild>(guildReader);
            _commands.AddTypeReader<RestGuild>(guildReader);
            _commands.AddTypeReader<SocketGuild>(guildReader);
            _commands.AddTypeReader<Uri>(new UriTypeReader());

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
    }
}
