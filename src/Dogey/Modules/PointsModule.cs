using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [RequireEnabled]
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
            await ReplyEmbedAsync(embed);
        }

        [Command("give"), Alias("gift")]
        public async Task GiveAsync(SocketUser user, [Range(1, int.MaxValue)]int amount)
        {
            var log = await _points.TradePointsAsync(Context.User, user, amount);
            if (log == null)
            {
                await ReplyAsync($"You don't have enough points to do that.");
                return;
            }
            
            await ReplyAsync($"{Context.User.Mention} has given {user.Mention} **{amount}** point(s).");
        }
    }
}
