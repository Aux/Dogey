using System;

namespace Dogey.Models
{
    public class RssFeed : IEquatable<RssFeed>
    {
        public ulong Id { get; set; }
        public ulong ChannelId { get; set; }
        public string Url { get; set; }
        public string Regex { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public bool Equals(RssFeed other)
            => other.Id == Id;
    }
}
