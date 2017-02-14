using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.MySQL.Modules
{
    public class LogModule : ModuleBase<SocketCommandContext>
    {
        [Command("loginfo")]
        [Remarks("Get information about tables and rows in the mysql log database.")]
        public Task InfoAsync()
        {
            return Task.CompletedTask;
        }
    }
}
