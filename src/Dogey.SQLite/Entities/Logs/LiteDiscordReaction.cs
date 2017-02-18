using System;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public class LiteDiscordReaction : LiteEntity, IDiscordReaction
    {
        public DateTime CreatedAt { get; set; }
        public ulong MessageId { get; set; }
        public ulong AuthorId { get; set; }
        public ulong? EmojiId { get; set; }
        public ulong EmojiName { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }

        // Foreign Keys
        public ulong LoggedMessageId { get; set; }
        public LiteDiscordMessage LoggedMessage { get; set; }

        public override Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
