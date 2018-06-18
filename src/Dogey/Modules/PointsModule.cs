using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("points")]
    public class PointsModule : DogeyModuleBase
    {
        private readonly PointsController _points;

        public PointsModule(PointsController points)
        {
            _points = points;
        }

        [Command]
        public Task PointsAsync()
            => PointsAsync(Context.User);

        [Command]
        public async Task PointsAsync([Remainder]SocketUser user)
        {
            var wallet = await _points.GetOrCreateWalletAsync(user);

            var embed = new EmbedBuilder()
                .WithDescription($"{user.Mention} currently has {wallet.Balance} point(s).");
        }

        [Command("give"), Alias("gift")]
        public async Task GiveAsync(SocketUser user, int amount)
        {
            var sender = await _points.GetOrCreateWalletAsync(Context.User);
            if (sender.Balance < amount)
            {
                await ReplyAsync($"You don't have enough points to do that.");
                return;
            }
            
            var receiver = await _points.GetOrCreateWalletAsync(user);

            await _points.CreateAsync(new PointLog
            {
                UserId = sender.Id,
                SenderId = receiver.Id,
                Amount = amount * -1
            });
            await _points.CreateAsync(new PointLog
            {
                UserId = receiver.Id,
                SenderId = sender.Id,
                Amount = amount
            });

            await ReplyAsync($"{Context.User.Mention} has given {MentionUtils.MentionUser(receiver.Id)} **{amount}** point(s).");
        }
    }
}
