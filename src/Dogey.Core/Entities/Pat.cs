using System;
using System.ComponentModel.DataAnnotations;

namespace Dogey
{
    public class Pat : Entity<ulong>
    {
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [Required]
        public ulong SenderId { get; set; }
        public string SenderName { get; set; }
        [Required]
        public ulong ReceiverId { get; set; }
    }
}
