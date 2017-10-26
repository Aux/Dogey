using System;

namespace Dogey
{
    public class Pat : IEntity<ulong>
    {
        public ulong Id { get; private set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public ulong SenderId { get; set; }
        public string SenderName { get; set; }
        public ulong ReceiverId { get; set; }
    }
}
