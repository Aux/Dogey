using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public class LiteTag : LiteEntity<ulong>, ITag<ulong>
    {
        [Timestamp]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public ulong OwnerId { get; set; }
        [Required]
        public ulong GuildId { get; set; }

        public override Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
