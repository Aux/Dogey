using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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

        private async Task OnMessageReceivedAsync(SocketMessage message)
        {
            var log = new LiteDiscordMessage(message);

            using (var db = new LogDatabase())
            {
                await db.Messages.AddAsync(log);
                await db.SaveChangesAsync();
            }
        }

        private async Task OnReactionAddedAsync(ulong id, Optional<SocketUserMessage> msg, SocketReaction reaction)
        {
            var log = new LiteDiscordReaction(reaction);

            using (var db = new LogDatabase())
            {
                var message = await db.Messages.FirstOrDefaultAsync(x => x.MessageId == reaction.MessageId);

                if (message != null)
                    log.LogMessageId = message.Id;

                await db.Reactions.AddAsync(log);
                await db.SaveChangesAsync();
            }
        }

        private async Task OnReactionRemovedAsync(ulong id, Optional<SocketUserMessage> msg, SocketReaction reaction)
        {
            using (var db = new LogDatabase())
            {
                var log = await db.Reactions.FirstOrDefaultAsync(x 
                    => x.MessageId == reaction.MessageId 
                    && x.AuthorId == reaction.UserId 
                    && x.EmojiId == reaction.Emoji.Id
                    && x.EmojiName == reaction.Emoji.Name);

                log.DeletedAt = DateTime.UtcNow;
                db.Update(log);
                await db.SaveChangesAsync();
            }
        }

        private async Task OnReactionsClearedAsync(ulong id, Optional<SocketUserMessage> msg)
        {
            using (var db = new LogDatabase())
            {
                var logs = db.Reactions.Where(x => x.MessageId == msg.Value.Id);

                foreach (var log in logs)
                    log.DeletedAt = DateTime.UtcNow;

                db.UpdateRange(logs);
                await db.SaveChangesAsync();
            }
        }
    }
}
