using System.ComponentModel.DataAnnotations;

namespace Dogey.MySQL
{
    public class MyGuildConfig : MyEntity<long>, IGuildConfig<long>
    {
        [Required]
        public long GuildId { get; set; }
        public string Prefix { get; set; }

        public MyGuildConfig() { }
        public MyGuildConfig(ulong guildId)
        {
            GuildId = (long)guildId;
        }
    }
}
