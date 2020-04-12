using System.Collections.Generic;

namespace Dogey
{
    public class User
    {
        public ulong Id { get; set; }
        public string Locale { get; set; }

        public List<GuildUser> Guilds { get; set; }
    }
}
