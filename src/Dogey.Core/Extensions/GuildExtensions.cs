using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dogey
{
    public static class GuildExtensions
    {
        public static async Task<string> GetPrefixAsync(this IGuild guild)
        {
            using (var db = new ConfigDatabase())
            {
                var config = await db.GetConfigAsync(guild.Id);
                return config.Prefix;
            }
        }
    }
}
