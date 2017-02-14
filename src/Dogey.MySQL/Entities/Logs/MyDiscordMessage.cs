using System;
using System.ComponentModel.DataAnnotations;

namespace Dogey.MySQL
{
    public class MyDiscordMessage : MyEntity<long>, IDiscordMessage
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
    }
}
