using System.ComponentModel.DataAnnotations;

namespace Dogey
{
    public class GuildConfig : Entity<ulong>
    {
        [Required]
        public ulong GuildId { get; set; }
        public string Prefix { get; set; }

        public GuildConfig() { }
        public GuildConfig(ulong guildId)
        {
            GuildId = guildId;
        }
    }
}
