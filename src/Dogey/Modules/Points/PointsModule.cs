using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Points
{
    [Name("Points"), Group("point"), Alias("points", "pt", "pts")]
    public class PointsModule : DogeyModuleBase
    {
        private readonly PointsManager _manager;

        public PointsModule(PointsManager manager)
        {
            _manager = manager;
        }

        [Command]
        [Summary("View your current points profile")]
        public Task PointsAsync()
            => PointsAsync(Context.User);

        [Command]
        [Summary("View the specified user's current points profile")]
        public async Task PointsAsync(SocketUser user)
        {
            var profile = await _manager.GetProfileAsync(user.Id);

            var embed = new EmbedBuilder()
                .WithDescription($"{user} currently has {profile.TotalPoints}/{profile.WalletSize} point(s).");

            await ReplyAsync(embed);
        }

        [Command("recent")]
        [Summary("View your recent points balance changes")]
        public Task RecentAsync()
            => RecentAsync(Context.User);

        [Command("recent")]
        [Summary("View the specified user's recent points balance changes")]
        public async Task RecentAsync(SocketUser user)
        {
            var points = await _manager.GetRecentPointsAsync(user.Id);

            var builder = new StringBuilder();
            foreach (var point in points)
                builder.AppendLine($"({point.MessageId}) **x{point.Modifier}**");

            var embed = new EmbedBuilder()
                .WithTitle($"Recent point changes for {user}")
                .WithDescription(builder.ToString());

            await ReplyAsync(embed);
        }
        
        [Command("upgrade")]
        [Summary("Double your wallet size")]
        [Remarks("Upgrading your wallet will double your potential max points (250 to 500, 500 to 1000, etc...), at the cost of a maxed-out wallet.")]
        public async Task UpgradeAsync()
        {
            var profile = await _manager.GetProfileAsync(Context.User.Id);

            if (profile.IsMaxPoints())
            {
                profile = await _manager.UpgradeWalletAsync(profile);
                await ReplyAsync($"Your wallet size is now {profile.WalletSize}");
                return;
            }

            await ReplyAsync($"You need {profile.WalletSize - profile.TotalPoints} more point(s) to upgrade your wallet.");
        }
    }
}
