using System;

namespace Dogey
{
    public class PointLog
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public string SenderId { get; set; }
        public DateTime Timestamp { get; set; }
        public EarningType EarningType { get; set; }
        public int Amount { get; set; }
    }
}
