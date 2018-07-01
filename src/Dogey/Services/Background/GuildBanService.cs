using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dogey
{
    public class GuildBanService : BackgroundService
    {
        private readonly ILogger<GuildBanService> _logger;
        private readonly DiscordSocketClient _discord;
        private readonly RootController _root;

        private CancellationToken _cancellationToken;

        public GuildBanService(ILogger<GuildBanService> logger, DiscordSocketClient discord, RootController root)
        {
            _logger = logger;
            _discord = discord;
            _root = root;
        }
        
        private Task CheckGuildAsync(SocketGuild guild)
        {
            _ = Task.Run(async () =>
            {
                bool banned = await _root.IsBannedAsync(guild);
                if (banned)
                {
                    _logger.LogInformation("Leaving banned guild `{name} ({id})`", guild.Name, guild.Id);
                    await guild.LeaveAsync();
                }
            }, _cancellationToken);
            return Task.CompletedTask;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _discord.JoinedGuild += CheckGuildAsync;
            _discord.GuildAvailable += CheckGuildAsync;
            _logger.LogInformation("Started");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _discord.JoinedGuild -= CheckGuildAsync;
            _discord.GuildAvailable -= CheckGuildAsync;
            _logger.LogInformation("Stopped");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => throw new NotImplementedException();
    }
}
