using System;

namespace Dogey
{
    public class ChannelConfig
    {
        public ulong Id { get; set; }
        public ulong GuildId { get; set; }
        public string SuccessEmoji { get; set; }
        public DateTime? BannedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
