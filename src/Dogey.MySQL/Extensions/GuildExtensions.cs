using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dogey.MySQL
{
    public static class GuildExtensions
    {
        public static async Task<IEnumerable<MyDiscordMessage>> GetMessageLogsAsync(this IGuild guild)
        {
            await Task.Delay(0);
            return null;
        }
    }
}
