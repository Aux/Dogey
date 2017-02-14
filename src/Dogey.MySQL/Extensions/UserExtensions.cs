using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dogey.MySQL
{
    public static class UserExtensions
    {
        public static async Task<IEnumerable<MyDiscordMessage>> GetMessageLogsAsync(this IUser user)
        {
            await Task.Delay(0);
            return null;
        }
    }
}
