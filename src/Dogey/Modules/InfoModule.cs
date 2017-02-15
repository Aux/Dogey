using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("info")]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        public Task BaseAsync()
            => new HelpModule().HelpAsync(Context, "info");

        [Command("summary")]
        public Task SummaryAsync()
        {
            return Task.CompletedTask;
        }

        [Command("performance")]
        public Task PerformanceAsync()
        {
            return Task.CompletedTask;
        }

        [Command("activity")]
        public Task ActivityAsync()
        {
            return Task.CompletedTask;
        }
    }
}
