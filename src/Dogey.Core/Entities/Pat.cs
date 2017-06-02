using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogey
{
    public class Pat : IEntity<ulong>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; private set; }
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [Required]
        public ulong SenderId { get; set; }
        public string SenderName { get; set; }
        [Required]
        public ulong ReceiverId { get; set; }
    }
}
