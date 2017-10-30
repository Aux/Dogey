using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.Modules.Dogs
{
    [Name("Dogs")]
    public class DogModule : DogeyModuleBase
    {
        private readonly DogManager _dogs;
        private readonly PointsManager _points;

        private const ulong _dogeId = 206606983250313216;
        private const ulong _dogId = 318586966788669450;

        public DogModule(DogManager dogs, PointsManager points)
        {
            _dogs = dogs;
            _points = points;
        }
        
        [Command("doge"), RequireCost(5)]
        public async Task DogeAsync()
        {
            var image = await _dogs.GetRandomDogImageAsync(_dogeId);
            var profile = await _points.GetOrCreateProfileAsync(Context.User.Id);

            var embed = new EmbedBuilder()
                .WithImageUrl(image.Url)
                .WithFooter($"{Context.User} now has {profile.TotalPoints}/{profile.WalletSize} points");

            await ReplyAsync(embed);
        }

        [Command("dog"), RequireCost(5)]
        public async Task DogAsync()
        {
            var image = await _dogs.GetRandomDogImageAsync(_dogId);
            var points = await _points.GetProfileAsync(Context.User.Id);

            var embed = new EmbedBuilder()
                .WithImageUrl(image.Url)
                .WithFooter($"{Context.User.Mention} now has {points.TotalPoints}/{points.WalletSize} points");

            await ReplyAsync(embed);
        }
    }
}
