using System;
using System.ComponentModel.DataAnnotations;

namespace Dogey.SQLite
{
    public class LitePat : LiteEntity<ulong>, IPat<ulong>
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
