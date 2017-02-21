using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public class LiteDiscordCommand : LiteEntity<ulong>, IDiscordCommand
    {
        [Required]
        public string Name { get; set; }
        public string Parameters { get; set; }
        [Required]
        public double ExecuteTime { get; set; }

        // Foreign Keys
        public ulong LogMessageId { get; set; }
        public LiteDiscordMessage LogMessage { get; set; }

        public override Task SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
