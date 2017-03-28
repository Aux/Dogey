using Discord;
using Discord.Commands;
using NTwitch.Rest;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("twitch")]
    public class TwitchModule : ModuleBase<SocketCommandContext>
    {
        private TwitchRestClient _client;

        public TwitchModule(TwitchRestClient client)
        {
            _client = client;
        }

        [Command("online")]
        public async Task CheckStreamAsync(string channel)
        {
            var user = (await _client.GetUsersAsync(channel)).FirstOrDefault();
            if (user == null)
            {
                await ReplyAsync($"Could not find a stream like `{channel}`");
                return;
            }

            var stream = await _client.GetStreamAsync(user.Id);
            if (stream == null)
                await ReplyAsync("", embed: GetOfflineEmbed(user));
            else
                await ReplyAsync("", embed: GetOnlineEmbed(stream));
        }

        private Embed GetOfflineEmbed(RestUser user)
        {
            var builder = new EmbedBuilder();

            builder.Title = $"{user.DisplayName} is not online";
            builder.ThumbnailUrl = user.LogoUrl;

            return builder;
        }

        private Embed GetOnlineEmbed(RestStream stream)
        {
            var builder = new EmbedBuilder();

            builder.ThumbnailUrl = stream.Channel.LogoUrl;
            builder.Title = $"{stream.Channel.DisplayName} is live!";
            builder.Url = stream.Channel.Url;
            builder.Description = stream.Channel.Status;
            builder.ImageUrl = stream.Previews["large"];

            builder.AddField(x =>
            {
                x.Name = "Viewers";
                x.Value = stream.Viewers;
                x.IsInline = true;
            });

            builder.WithFooter(x =>
            {
                x.IconUrl = stream.Channel.LogoUrl;
                x.Text = stream.CreatedAt.ToString();
            });

            return builder;
        }
    }
}
