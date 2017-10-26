using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Dogey.Modules.Points
{
    [RequireOwner]
    [Name("Points"), Group("point"), Alias("points", "pt", "pts")]
    public class PointsAdminModule : DogeyModuleBase
    {
        private readonly PointsManager _manager;

        public PointsAdminModule(PointsManager manager)
        {
            _manager = manager;
        }

        [Command("handicap")]
        [Summary("Set a global point modifier for the specified user")]
        public async Task HandicapAsync(SocketUser user, double multiplier)
        {
            await Task.Delay(0);
        }

        [Command("modify")]
        [Summary("Modify the user's points balance by the specified amount")]
        public async Task ModifyAsync(SocketUser user, int amount)
        {
            await Task.Delay(0);
        }

        [Command("set")]
        [Summary("Set a user's points balance to the specified amount")]
        public async Task SetAsync(SocketUser user, int amount)
        {
            await Task.Delay(0);
        }

        [Command("reset")]
        [Summary("Reset a user's points profile to default")]
        public async Task ResetAsync(SocketUser user)
        {
            await Task.Delay(0);
        }
    }
}
