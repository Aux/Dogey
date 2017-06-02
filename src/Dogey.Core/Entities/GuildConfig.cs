using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogey
{
    public class GuildConfig : IEntity<ulong>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; private set; }
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
