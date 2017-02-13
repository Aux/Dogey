using System;
using System.ComponentModel.DataAnnotations;

namespace Dogey
{
    public class DiscordMessage
    {
        [Key]
        public ulong Id { get; set; }
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
        public string Content { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
