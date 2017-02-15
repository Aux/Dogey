using Discord.Commands;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Linq;
using System.Threading.Tasks;
using Youtube = Google.Apis.YouTube.v3.Data;

namespace Dogey.Modules
{
    [Group("youtube"), Alias("yt")]
    public class YoutubeModule : ModuleBase<SocketCommandContext>
    {
        private YouTubeService _youtube;

        protected override void BeforeExecute()
        {
            _youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Configuration.Load().Token.Youtube
            });
        }
        
        [Command]
        public Task BaseAsync()
            => new HelpModule().HelpAsync(Context, "youtube");

        [Command("search")]
        public async Task SearchAsync([Remainder]string query)
        {
            var video = await SearchYoutubeAsync(query, "youtube#video");

            if (video == null)
                await ReplyAsync($"I could not find a video like `{query}`");
            else
                await ReplyAsync($"http://youtube.com/watch?v={video.Id.VideoId}");
        }

        [Command("channel")]
        public async Task ChannelAsync([Remainder]string query)
        {
            var channel = await SearchYoutubeAsync(query, "youtube#channel");

            if (channel == null)
                await ReplyAsync($"I could not find a channel like `{query}`");
            else
                await ReplyAsync($"https://www.youtube.com/user/{channel.Id.ChannelId}");
        }

        [Command("playlist")]
        public async Task PlaylistAsync([Remainder]string query)
        {
            var playlist = await SearchYoutubeAsync(query, "youtube#playlist");

            if (playlist == null)
                await ReplyAsync($"I could not find a playlist like `{query}`");
            else
                await ReplyAsync($"http://youtube.com/?list={playlist.Id.PlaylistId}");
        }

        private async Task<Youtube.SearchResult> SearchYoutubeAsync(string query, string dataType)
        {
            var request = _youtube.Search.List("snippet");
            request.Q = query;
            request.MaxResults = 1;

            var result = await request.ExecuteAsync();
            return result.Items.FirstOrDefault(x => x.Id.Kind == dataType);
        }
    }
}
