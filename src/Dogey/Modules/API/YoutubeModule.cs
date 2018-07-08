using Discord.Commands;
using Google.Apis.YouTube.v3;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    public class YoutubeModule : DogeyModuleBase
    {
        public const string BaseUrl = "http://youtu.be/";

        private readonly YouTubeService _youtube;

        public string GetVideoUrl(string id)
            => BaseUrl + id;

        public YoutubeModule(YouTubeService youtube, RootController root)
            : base(root)
        {
            _youtube = youtube;
        }

        [Command("youtube"), Alias("yt")]
        public async Task SearchAsync([Remainder]string query)
        {
            var request = _youtube.Search.List("snippet");
            request.Q = query;
            request.MaxResults = 1;

            var result = await request.ExecuteAsync();
            var video = result.Items.FirstOrDefault(x => x.Id.Kind == "youtube#video");

            if (video == null)
                await ReplyAsync("No results found.");
            await ReplyAsync(GetVideoUrl(video.Id.VideoId));
        }

    }
}
