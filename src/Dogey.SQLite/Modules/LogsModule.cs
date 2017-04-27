using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.SQLite.Modules
{
    [Group("logs"), Name("Logs")]
    public class LogsModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Summary("Get some general information about the sqlite databases.")]
        public Task LogsAsync()
        {
            return Task.CompletedTask;
        }
    }
}
