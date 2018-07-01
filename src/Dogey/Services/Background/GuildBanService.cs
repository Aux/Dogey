using Discord;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dogey
{
    public class GuildBanService : BackgroundService
    {
        private readonly DiscordSocketClient _discord;
        private readonly LoggingService _logger;
        private readonly RootController _root;

        public GuildBanService(DiscordSocketClient discord, LoggingService logger, RootController root)
        {
            _discord = discord;
            _logger = logger;
            _root = root;
        }
        
        private async Task CheckGuildAsync(SocketGuild guild)
        {
            bool banned = await _root.IsBannedAsync(guild);
            if (banned)
            {
                await _logger.LogAsync(LogSeverity.Info, nameof(GuildBanService), $"Leaving banned guild `{guild.Name} ({guild.Id})`");
                await guild.LeaveAsync();
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _discord.JoinedGuild += CheckGuildAsync;
            _discord.GuildAvailable += CheckGuildAsync;
            await _logger.LogAsync(LogSeverity.Info, nameof(GuildBanService), "Started");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _discord.JoinedGuild -= CheckGuildAsync;
            _discord.GuildAvailable -= CheckGuildAsync;
            await _logger.LogAsync(LogSeverity.Info, nameof(GuildBanService), "Stopped");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => throw new NotImplementedException();
    }
}
