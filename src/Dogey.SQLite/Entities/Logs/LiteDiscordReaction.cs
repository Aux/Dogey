using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

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
        public bool? IsDeleted { get; set; }

        // Foreign Keys
        public ulong LogMessageId { get; set; }
        public LiteDiscordMessage Message { get; set; }

        public override Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
