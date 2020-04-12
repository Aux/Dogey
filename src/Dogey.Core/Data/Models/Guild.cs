using System.Collections.Generic;

namespace Dogey
{
    public class Guild
    {
        public ulong Id { get; set; }
        public string StringPrefix { get; set; }
        public bool AllowMentionPrefix { get; set; } = true;

        public List<GuildUser> Users { get; set; }
    }
}
