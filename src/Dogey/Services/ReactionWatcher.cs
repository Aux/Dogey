using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Dogey.Models;

namespace Dogey.Services
{
    /// <summary> Watches for reaction events and forwards them to services that implement them </summary>
    public class ReactionWatcher : Service
    {
        public event Action<ReactionInterest, SocketReaction> ReactionAdded;
        public event Action<ReactionInterest, SocketReaction> ReactionRemoved;

        private readonly DiscordSocketClient _discord;

        public ReactionWatcher(DiscordSocketClient discord)
        {
            _discord = discord;
        }

        public override void Start()
        {
            _discord.ReactionAdded += OnReactionAddedAsync;
            _discord.ReactionRemoved += OnReactionRemovedAsync;
        }

        public override void Stop()
        {
            _discord.ReactionAdded -= OnReactionAddedAsync;
            _discord.ReactionRemoved -= OnReactionRemovedAsync;
        }

        private Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            throw new NotImplementedException();
        }

        private Task OnReactionRemovedAsync(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel channel, SocketReaction reaction)
        {
            throw new NotImplementedException();
        }
    }
}
