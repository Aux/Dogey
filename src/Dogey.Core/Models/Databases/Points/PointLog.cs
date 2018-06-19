using System;

namespace Dogey
{
    public class PointLog
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public ulong SenderId { get; set; }
        public DateTime Timestamp { get; set; }
        public int Amount { get; set; }
    }
}
