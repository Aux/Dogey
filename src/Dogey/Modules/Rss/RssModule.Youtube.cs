using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dogey.Modules.Rss
{
    public partial class RssModule : DogeyModuleBase
    {
        [Command("addyoutube")]
        public Task AddYoutubeAsync(string channelName, SocketTextChannel channel = null, Regex regex = null)
            => AddYoutubeAsync(channelName, channel ?? Context.Channel, regex.ToString());
        [Command("addyoutubeid")]
        public Task AddYoutubeIdAsync(string channelId, SocketTextChannel channel = null, Regex regex = null)
            => AddYoutubeAsync(channelId, channel ?? Context.Channel, regex.ToString(), true);

        private async Task AddYoutubeAsync(string value, IChannel channel, string regex = null, bool isId = false)
        {
            var request = _youtube.Search.List("snippet");
            if (isId)
                request.ChannelId = value;
            else
                request.Q = value;
            request.MaxResults = 1;

            var response = await request.ExecuteAsync();
            var ytchannel = response.Items.SingleOrDefault(x => x.Id.Kind == "youtube#channel");
            if (ytchannel == null)
            {
                await ReplyAsync($"A channel like `{value}` was not found");
                return;
            }

            if (AddFeed($"https://www.youtube.com/feeds/videos.xml?channel_id={ytchannel.Snippet.ChannelId}", channel, regex)) 
                await ReplyAsync($"Added feed for youtube channel `{ytchannel.Snippet.ChannelTitle}`");
        }
    }
}
