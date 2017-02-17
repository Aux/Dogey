using System;
using System.Threading.Tasks;

namespace Dogey.Redis.Entities.Logs
{
    public class RedisDiscordMessage : RedisEntity, IDiscordMessage
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

        public override Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
