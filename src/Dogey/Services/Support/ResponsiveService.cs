using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dogey
{
    public class ResponsiveService
    {
        private readonly ILogger<ResponsiveService> _logger;
        private readonly ResponsiveOptions _options;
        private readonly DiscordSocketClient _discord;

        public ResponsiveService(ILogger<ResponsiveService> logger, IConfiguration config, DiscordSocketClient discord)
        {
            _logger = logger;
            _discord = discord;

            var options = new ResponsiveOptions();
            config.Bind("responsive", options);
            _options = options;
        }
        
        private async Task<T> WaitAsync<T>(TaskCompletionSource<T> tcs, TimeSpan? expireAfter = null)
        {
            new Timer((s) => tcs.TrySetCanceled(), null, expireAfter == null ? TimeSpan.FromSeconds(_options.DefaultExpireSeconds) : (TimeSpan)expireAfter, TimeSpan.Zero);
            try
            {
                return await tcs.Task;
            }
            catch (Exception)
            {
                _logger.LogInformation("Cancelled task after {0} seconds with no reply", _options.DefaultExpireSeconds);
            }
            return default;
        }
        
        public async Task<SocketMessage> WaitForMessageAsync(Func<SocketMessage, bool> condition, TimeSpan? expireAfter = null)
        {
            var tcs = new TaskCompletionSource<SocketMessage>();

            _discord.MessageReceived += (msg) =>
            {
                if (condition(msg))
                    tcs.TrySetResult(msg);
                return Task.CompletedTask;
            };

            return await WaitAsync(tcs, expireAfter);
        }
        
        public async Task<SocketReaction> WaitForReactionAsync(Func<Cacheable<IUserMessage, ulong>, ISocketMessageChannel, SocketReaction, bool> condition, TimeSpan? expireAfter = null)
        {
            var tcs = new TaskCompletionSource<SocketReaction>();

            _discord.ReactionAdded += (cache, ch, r) =>
            {
                if (condition(cache, ch, r))
                    tcs.TrySetResult(r);
                return Task.CompletedTask;
            };

            return await WaitAsync(tcs, expireAfter);
        }
    }
}
