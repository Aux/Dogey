using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.Redis.Modules
{
    public class LogModule : ModuleBase<SocketCommandContext>
    {
        [Command("loginfo")]
        [Remarks("Get information about log objects in the redis database.")]
        public Task InfoAsync()
        {
            return Task.CompletedTask;
        }
    }
}
