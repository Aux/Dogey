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
        private readonly RootController _root;

        public PointsModule(PointsController points, RootController root)
        {
            _points = points;
            _root = root;
        }
        
        [Command]
        public Task PointsAsync()
            => PointsAsync(Context.User);

        [Command]
        public async Task PointsAsync([Remainder]SocketUser user)
        {
            var currency = await _root.GetCurrencyNameAsync(Context.Guild);
            var wallet = await _points.GetOrCreateWalletAsync(user);

            var embed = new EmbedBuilder()
                .WithDescription($"{user.Mention} currently has {wallet.Balance} {currency}(s).");
        }

        [Command("give"), Alias("gift")]
        public async Task GiveAsync(SocketUser user, int amount)
        {
            var currency = await _root.GetCurrencyNameAsync(Context.Guild);
            var sender = await _points.GetOrCreateWalletAsync(Context.User);
            if (sender.Balance < amount)
            {
                await ReplyAsync($"You don't have enough {currency}s to do that.");
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

            await ReplyAsync($"{Context.User.Mention} has given {MentionUtils.MentionUser(receiver.Id)} **{amount}** {currency}(s).");
        }
    }
}
