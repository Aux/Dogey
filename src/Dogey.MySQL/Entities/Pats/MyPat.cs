using System;
using System.ComponentModel.DataAnnotations;

namespace Dogey.MySQL
{
    public class MyPat : MyEntity<long>, IPat<long>
    {
        [Required, Timestamp]
        public DateTime Timestamp { get; set; }
        [Required]
        public long SenderId { get; set; }
        public string SenderName { get; set; }
        [Required]
        public long ReceiverId { get; set; }
    }
}
