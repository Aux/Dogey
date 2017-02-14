using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dogey.MySQL
{
    public static class ChannelExtensions
    {
        public static async Task<IEnumerable<MyDiscordMessage>> GetMessageLogsAsync(this IMessageChannel channel)
        {
            await Task.Delay(0);
            return null;
        }
    }
}
