using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class SearchModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("youtube")
                    .Alias(new string[] { "yt" })
                    .Description("Search youtube for a video matching the specified text.")
                    .Parameter("keywords", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                        {
                            ApiKey = Program.config.GoogleToken,
                            ApplicationName = this.GetType().ToString()
                        });

                        var searchListRequest = youtubeService.Search.List("snippet");
                        searchListRequest.Q = e.Args[0];
                        searchListRequest.MaxResults = 50;

                        var searchListResponse = await searchListRequest.ExecuteAsync();
                        
                        foreach (var searchResult in searchListResponse.Items)
                        {
                            if (searchResult.Id.Kind == "youtube#video")
                            {
                                await e.Channel.SendMessage($"https://www.youtube.com/watch?v={searchResult.Id.VideoId}");
                                return;
                            }
                        }
                    });
                cmd.CreateCommand("gif")
                    .Description("Search for a gif matching the specified text.")
                    .Parameter("keywords", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var r = new Random();
                        string tempFile = $"servers\\{e.Server.Id}\\{r.Next(10000, 99999)}.gif";
                        string getUrl = $"http://api.giphy.com/v1/gifs/random?api_key=dc6zaTOxFJmzC&tag={Uri.UnescapeDataString(e.Args[0])}";
                        
                        using (WebClient client = new WebClient())
                        {
                            string json = client.DownloadString(getUrl);
                            string image = JObject.Parse(json)["data"]["image_original_url"].ToString();

                            client.DownloadFile(image, tempFile);
                        }
                        
                        await e.Channel.SendFile(tempFile);
                        System.IO.File.Delete(tempFile);
                    });

                DogeyConsole.Log(LogSeverity.Info, "SearchModule", "Loaded.");
            });
        } 
    }
}
