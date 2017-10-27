using System;

namespace Dogey
{
    public class Point
    {
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public ulong MessageId { get; set; }
        public ulong UserId { get; set; }
        public int Modifier { get; set; }
    }
}
