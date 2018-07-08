using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class StartupService
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
        
        public async Task StartAsync()
        {
            var fileLoggerOptions = new FileLoggerOptions();
            _config.Bind("filelogger", fileLoggerOptions);
            _loggerFactory.AddProvider(new DogeyLoggerProvider(fileLoggerOptions));
            
            await _discord.LoginAsync(TokenType.Bot, _config["tokens:discord"]);
            await _discord.StartAsync();

            var guildReader = new GuildTypeReader();
            _commands.AddTypeReader<IGuild>(guildReader);
            _commands.AddTypeReader<RestGuild>(guildReader);
            _commands.AddTypeReader<SocketGuild>(guildReader);
            _commands.AddTypeReader<Uri>(new UriTypeReader());
            _commands.AddTypeReader<Emote>(new EmoteTypeReader());
            _commands.AddTypeReader<ModuleInfo>(new ModuleInfoTypeReader());

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }
    }
}
