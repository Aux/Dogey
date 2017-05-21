using Discord.Commands;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Youtube = Google.Apis.YouTube.v3.Data;

namespace Dogey.Modules
{
    [Group("youtube"), Alias("yt"), Name("Youtube")]
    [Summary("Search for things on youtube.")]
    public class YoutubeModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;
        private readonly YouTubeService _youtube;

        public YoutubeModule(IServiceProvider provider)
        {
            _commands = provider.GetService<CommandService>();
            _youtube = provider.GetService<YouTubeService>();
        }
        
        [Command]
        public Task BaseAsync()
            => new HelpModule(_commands).HelpAsync(Context, "youtube");

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
