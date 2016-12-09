using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class DiscordMessage
    {
        public DateTime Timestamp { get; set; }
        public ulong? GuildId { get; set; }
        public ulong? ChannelId { get; set; }
        public ulong AuthorId { get; set; }
        public string Content { get; set; }
    }
}
