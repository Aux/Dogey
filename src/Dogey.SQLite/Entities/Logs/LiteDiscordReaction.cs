using Discord.WebSocket;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dogey.SQLite
{
    public class LiteDiscordReaction : LiteEntity<ulong>, IDiscordReaction
    {
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public ulong AuthorId { get; set; }
        [Required]
        public ulong MessageId { get; set; }
        public ulong? EmojiId { get; set; }
        public string EmojiName { get; set; }
        public DateTime? DeletedAt { get; set; }

        // Foreign Keys
        public ulong? LogMessageId { get; set; }
        public LiteDiscordMessage LogMessage { get; set; }

        public LiteDiscordReaction(SocketReaction reaction)
        {
            CreatedAt = DateTime.UtcNow;
            AuthorId = reaction.UserId;
            MessageId = reaction.MessageId;
            EmojiId = reaction.Emoji.Id;
            EmojiName = reaction.Emoji.Name;
        }
    }
}
