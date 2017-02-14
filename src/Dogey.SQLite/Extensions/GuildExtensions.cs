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
    }
}
