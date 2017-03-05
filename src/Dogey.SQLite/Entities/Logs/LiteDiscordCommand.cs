using System.ComponentModel.DataAnnotations;

namespace Dogey.SQLite
{
    public class LiteDiscordCommand : LiteEntity<ulong>, IDiscordCommand
    {
        [Required]
        public ulong MessageId { get; set; }
        [Required]
        public ulong LogMessageId { get; set; }
        [Required]
        public long ExecuteTime { get; set; }

        // Foreign Keys
        public LiteDiscordMessage LogMessage { get; set; }

        public LiteDiscordCommand() { }
        public LiteDiscordCommand(ulong logId, ulong msgId, long time)
        {
            LogMessageId = logId;
            MessageId = msgId;
            ExecuteTime = time;
        }
    }
}
