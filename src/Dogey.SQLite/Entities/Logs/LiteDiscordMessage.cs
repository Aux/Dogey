using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public class LiteDiscordMessage : LiteEntity<ulong>, IDiscordMessage
    {
        [Required]
        public DateTime CreatedAt { get; set; }
        public ulong? GuildId { get; set; }
        [Required]
        public ulong ChannelId { get; set; }
        [Required]
        public ulong MessageId { get; set; }
        [Required]
        public ulong AuthorId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }

        // Foreign Keys
        public LiteDiscordCommand Command { get; set; }
        public List<LiteDiscordReaction> Reactions { get; set; }

        public override Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
