using Discord.Commands;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Linq;
using System.Threading.Tasks;
using Youtube = Google.Apis.YouTube.v3.Data;

namespace Dogey.Modules
{
    [Group("youtube"), Alias("yt")]
    [Remarks("Search for things on youtube.")]
    public class YoutubeModule : ModuleBase<SocketCommandContext>
    {
        private YouTubeService _youtube;
        private CommandService _service;

        public YoutubeModule(CommandService service)
        {
            _service = service;
        }
        
        protected override void BeforeExecute()
        {
            string token = Configuration.Load().Token.Google;

            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("The Youtube module has not yet been configured. Please add the youtube token to the configuration file.");
            
            _youtube = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = token
            });
        }
        
        [Command]
        public Task BaseAsync()
            => new HelpModule(_service).HelpAsync(Context, "youtube");

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
