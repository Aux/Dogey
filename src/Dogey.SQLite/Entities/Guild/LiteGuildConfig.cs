using System.ComponentModel.DataAnnotations;

namespace Dogey.SQLite
{
    public class LiteGuildConfig : LiteEntity<ulong>, IGuildConfig<ulong>
    {
        [Required]
        public ulong GuildId { get; set; }
        public string Prefix { get; set; }
    }
}
