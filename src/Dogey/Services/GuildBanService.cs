using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class GuildBanService
    {
        private readonly DiscordSocketClient _discord;
        private readonly LoggingService _logger;
        private readonly RootController _root;

        public GuildBanService(DiscordSocketClient discord, LoggingService logger, RootController root)
        {
            _discord = discord;
            _logger = logger;
            _root = root;

            _discord.JoinedGuild += CheckGuildAsync;
            _discord.GuildAvailable += CheckGuildAsync;
        }
        
        private async Task CheckGuildAsync(SocketGuild guild)
        {
            bool banned = await _root.IsBannedAsync(guild);
            if (banned)
            {
                await _logger.LogAsync(LogSeverity.Info, "OnGuildJoined", $"Leaving banned guild `{guild.Name} ({guild.Id})`");
                await guild.LeaveAsync();
            }
        }
    }
}
