using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dogey
{
    public class ReactionRoleService : BackgroundService
    {
        public List<ReactionRole> ReactionRoles { get; }

        private readonly ILogger<ReactionRoleService> _logger;
        private readonly DiscordSocketClient _discord;
        private readonly RootController _root;

        private CancellationToken _cancellationToken;

        public ReactionRoleService(ILogger<ReactionRoleService> logger, DiscordSocketClient discord, RootController root)
        {
            ReactionRoles = new List<ReactionRole>();

            _logger = logger;
            _discord = discord;
            _root = root;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            var watches = await _root.GetReactionRolesAsync();
            ReactionRoles.AddRange(watches);
            
            _discord.ReactionAdded += OnReactionAddedAsync;
            _discord.ReactionRemoved += OnReactionRemovedAsync;

            _logger.LogInformation("Started");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            _discord.ReactionAdded -= OnReactionAddedAsync;
            _discord.ReactionRemoved -= OnReactionRemovedAsync;

            _logger.LogInformation("Stopped");
        }

        private async Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            var reactionRole = ReactionRoles.SingleOrDefault(x => x.MessageId == cache.Id);
            if (reactionRole == null) return;

            var author = reaction.User.Value as SocketGuildUser;
            if (author == null) return;
            
            if (author.Roles.Any(x => x.Id == reactionRole.RoleId)) return;
            var role = author.Guild.GetRole(reactionRole.RoleId);

            await author.AddRoleAsync(role);
        }

        private async Task OnReactionRemovedAsync(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            await Task.Delay(0);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => throw new NotImplementedException();
    }
}
