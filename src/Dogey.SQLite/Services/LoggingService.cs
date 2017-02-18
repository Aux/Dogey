using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public class LoggingService
    {
        private DiscordSocketClient _client;

        public LoggingService(DiscordSocketClient client)
        {
            _client = client;

            _client.MessageReceived += OnMessageReceivedAsync;
            _client.ReactionAdded += OnReactionAddedAsync;
            _client.ReactionRemoved += OnReactionRemovedAsync;
            _client.ReactionsCleared += OnReactionsClearedAsync;
        }

        private Task OnMessageReceivedAsync(SocketMessage message)
        {
            var log = new LiteDiscordMessage()
            {
                ChannelId = message.Channel.Id,
                AuthorId = message.Author.Id,
                MessageId = message.Id,
                Content = message.Content,
                Name = message.Author.Username,
                CreatedAt = message.Timestamp.UtcDateTime
            };

            if (message.Channel is SocketTextChannel channel)
            {
                log.Name = (message.Author as SocketGuildUser)?.Nickname ?? message.Author.Username;
                log.GuildId = channel.Guild.Id;
            }

            return log.SaveChangesAsync();
        }

        private Task OnReactionAddedAsync(ulong id, Optional<SocketUserMessage> msg, SocketReaction reaction)
        {
            throw new NotImplementedException();
        }

        private Task OnReactionRemovedAsync(ulong id, Optional<SocketUserMessage> msg, SocketReaction reaction)
        {
            throw new NotImplementedException();
        }

        private Task OnReactionsClearedAsync(ulong id, Optional<SocketUserMessage> msg)
        {
            throw new NotImplementedException();
        }
    }
}
