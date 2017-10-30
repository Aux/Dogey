using System;

namespace Dogey
{
    public class Point
    {
        public ulong Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public ulong UserId { get; set; }
        public long Amount { get; set; }
    }
}
