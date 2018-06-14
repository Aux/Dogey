using System;

namespace Dogey
{
    public class GuildConfig
    {
        public ulong Id { get; set; }
        public string Prefix { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
