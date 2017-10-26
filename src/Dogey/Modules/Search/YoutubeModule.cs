using Discord.Commands;
using Google.Apis.YouTube.v3;
using System.Linq;
using System.Threading.Tasks;
using Youtube = Google.Apis.YouTube.v3.Data;

namespace Dogey.Modules
{
    [Group("youtube"), Alias("yt"), Name("Youtube")]
    [Summary("Search for things on youtube.")]
    public class YoutubeModule : DogeyModuleBase
    {
        private readonly YouTubeService _youtube;

        public YoutubeModule(YouTubeService youtube)
        {
            _youtube = youtube;
        }
        
        [Command]
        [Remarks("Search for a video matching the provided text")]
        public async Task SearchAsync([Remainder]string query)
        {
            var video = await SearchYoutubeAsync(query, "youtube#video");

            if (video == null)
                await ReplyAsync($"I could not find a video like `{query}`");
            else
                await ReplyAsync($"http://youtube.com/watch?v={video.Id.VideoId}");
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
