using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.Owner
{
    [RequireOwner]
    [Group("points")]
    public class PointsOwnerModule : DogeyModuleBase
    {
        private readonly PointsController _points;

        public PointsOwnerModule(PointsController points)
        {
            _points = points;
        }

        [Command("setmultiplier")]
        public async Task SetMultiplierAsync(SocketUser user, double multiplier)
        {
            var wallet = await _points.GetOrCreateWalletAsync(user);
            wallet.Multiplier = multiplier;

            await _points.ModifyAsync(wallet);
            await ReplySuccessAsync();
        }

        [Command("resetmultiplier")]
        public async Task ReetMultiplierAsync([Remainder]SocketUser user)
        {
            var wallet = await _points.GetOrCreateWalletAsync(user);
            wallet.Multiplier = null;

            await _points.ModifyAsync(wallet);
            await ReplySuccessAsync();
        }

        [Command("modify")]
        public async Task ModifyPointsAsync(SocketUser user, int amount)
        {
            var wallet = await _points.GetOrCreateWalletAsync(user);
            wallet.Balance += amount;

            var log = await _points.CreateAsync(new PointLog
            {
                Timestamp = DateTime.UtcNow,
                UserId = user.Id,
                SenderId = 0,
                Amount = amount
            });

            await _points.ModifyAsync(wallet);
            await ReplySuccessAsync();
        }
    }
}
