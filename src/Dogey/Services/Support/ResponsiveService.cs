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
        private DogeyCommandContext _context;

        public ResponsiveService(ILogger<ResponsiveService> logger, IConfiguration config)
        {
            _logger = logger;

            var options = new ResponsiveOptions();
            config.Bind("responsive", options);
            _options = options;
        }

        public bool IsSuccessReply(IMessage msg)
            => _options.TrueReplies.Any(x => msg.Content.ToLower().Contains(x.ToLower()));

        public void SetContext(DogeyCommandContext context)
        {
            _context = context;
        }

        private async Task<T> WaitAsync<T>(TaskCompletionSource<T> tcs, bool expire)
        {
            if (expire) new Timer((s) => tcs.TrySetCanceled(), null, TimeSpan.FromSeconds(_options.ExpireSeconds), TimeSpan.Zero);
            try
            {
                return await tcs.Task;
            }
            catch (Exception)
            {
                _logger.LogInformation("Cancelled task after {0} seconds with no reply", _options.ExpireSeconds);
            }
            return default;
        }
        
        public async Task<SocketUserMessage> WaitForReplyAsync(ISocketMessageChannel channel, IUser user, bool expire = true)
        {
            var tcs = new TaskCompletionSource<SocketUserMessage>();

            _context.Client.MessageReceived += (msg) =>
            {
                if (msg is SocketUserMessage userMsg)
                {
                    if (msg.Channel.Id == channel.Id && msg.Author.Id == user.Id)
                        tcs.TrySetResult(userMsg);
                }
                return Task.CompletedTask;
            };

            return await WaitAsync(tcs, expire);
        }

        public async Task<SocketReaction> WaitForReactionAsync(IUserMessage msg, IUser user, bool expire = true)
        {
            var tcs = new TaskCompletionSource<SocketReaction>();

            _context.Client.ReactionAdded += (cache, ch, reaction) =>
            {
                if (ch.Id == msg.Channel.Id && reaction.UserId == user.Id)
                    tcs.TrySetResult(reaction);
                return Task.CompletedTask;
            };

            return await WaitAsync(tcs, expire);
        }
    }
}
