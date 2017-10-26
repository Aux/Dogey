using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Dogey.Modules.Points
{
    [Name("Points")]
    public class PayModule : DogeyModuleBase
    {
        private readonly PointsManager _manager;

        public PayModule(PointsManager manager)
        {
            _manager = manager;
        }

        [Command("pay")]
        [Summary("Pay another user an amount of points from your wallet")]
        public async Task PayAsync(SocketUser user, uint amount)
        {
            await Task.Delay(0);
        }
    }
}
