using System;

namespace Dogey.Models
{
    public class RssFeed
    {
        public ulong Id { get; set; }
        public ulong ChannelId { get; set; }
        public string Url { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
