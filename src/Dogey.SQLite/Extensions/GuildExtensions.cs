using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public static class GuildExtensions
    {
        public static async Task<IEnumerable<LiteDiscordMessage>> GetMessageLogsAsync(this IGuild guild)
        {
            await Task.Delay(0);
            return null;
        }

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
