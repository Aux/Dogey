using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Search
{
    [Module, Name("Search")]
    public class GoogleGroup
    {
        private DiscordSocketClient _client;

        public GoogleGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("youtube"), Alias("yt")]
        [Description("Search for a video on youtube with the provided keywords.")]
        [Example("yt the duck song")]
        public async Task Youtube(IUserMessage msg, [Remainder]string keywords)
        {
            var message = await msg.Channel.SendMessageAsync("Searching...");

            const string queryUrl = "https://www.youtube.com/watch?v={0}";
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Globals.Config.Token.Google,
                ApplicationName = "Dogey"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = keywords;
            searchListRequest.MaxResults = 1;

            var searchListResponse = await searchListRequest.ExecuteAsync();

            string video = null;
            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    video = string.Format(queryUrl, searchResult.Id.VideoId);
                }
            }

            await message.ModifyAsync((e) =>
            {
                if (string.IsNullOrEmpty(video))
                    e.Content = $"I was unable to find a video on youtube like `{keywords}`.";
                else
                    e.Content = video;
            });
        }
    }
}
