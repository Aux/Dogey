using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.API
{
    public class NasaModule : DogeyModuleBase
    {
        private readonly NasaApiService _nasa;

        public NasaModule(NasaApiService nasa)
        {
            _nasa = nasa;
        }

        [Command("apod")]
        public async Task GetApodAsync()
        {
            var apod = await _nasa.GetApodAsync();
            await ReplyApodAsync(apod);
        }

        [Command("apod")]
        public async Task GetApodAsync([Remainder]DateTime date)
        {
            var apod = await _nasa.GetApodAsync(date);
            await ReplyApodAsync(apod);
        }

        private async Task ReplyApodAsync(Apod apod)
        {
            var embed = new EmbedBuilder()
                .WithTitle(apod.Title)
                .WithDescription(apod.Explanation)
                .WithImageUrl(apod.ImageUrl)
                .WithTimestamp(apod.Date);
            await ReplyEmbedAsync(embed);
        }
    }
}
