using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        
        public StartupService(
            DiscordSocketClient discord,
            CommandService commands,
            IConfiguration config)
        {
            _config = config;
            _discord = discord;
            _commands = commands;
        }
        
        public async Task StartAsync()
        {
            await _discord.LoginAsync(TokenType.Bot, _config["tokens:discord"]);
            await _discord.StartAsync();

            var guildReader = new GuildTypeReader();
            _commands.AddTypeReader<IGuild>(guildReader);
            _commands.AddTypeReader<RestGuild>(guildReader);
            _commands.AddTypeReader<SocketGuild>(guildReader);
            _commands.AddTypeReader<Uri>(new UriTypeReader());
            _commands.AddTypeReader<Emote>(new EmoteTypeReader());
            _commands.AddTypeReader<ModuleInfo>(new ModuleInfoTypeReader());

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
    }
}
