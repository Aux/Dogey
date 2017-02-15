using System;

namespace Dogey.Redis.Entities.Logs
{
    public class RedisDiscordMessage : RedisEntity<long>, IDiscordMessage
    {
        public DateTime CreatedAt { get; set; }
        public ulong? GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong MessageId { get; set; }
        public ulong AuthorId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
