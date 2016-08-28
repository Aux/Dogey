using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.SearchModule
{
    public static class SearchWith
    {
        public static async Task<string> Youtube(string query)
        {
            const string queryUrl = "https://www.youtube.com/watch?v={0}";
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Globals.Config.Token.Google,
                ApplicationName = "Dogey"
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = 1;

            var searchListResponse = await searchListRequest.ExecuteAsync();

            foreach (var searchResult in searchListResponse.Items)
            {
                if (searchResult.Id.Kind == "youtube#video")
                {
                    return string.Format(queryUrl, searchResult.Id.VideoId);
                }
            }
            return $"I was unable to find a video on youtube like `{query}`.";
        }
            
        public static async Task<string> StackExchange(string site, string tag, string query)
        {
            var baseUri = new Uri("http://api.stackexchange.com/");
            string queryUrl = "2.2/search/advanced?page=1&pagesize=1&order=desc&sort=relevance&q={0}&answers=1&tagged={1}&site={2}";
            
            string q = string.Format(queryUrl, 
                Uri.EscapeDataString(query), 
                Uri.EscapeDataString(tag), 
                Uri.EscapeDataString(site));

            using (var client = new HttpClient())
            {
                client.BaseAddress = baseUri;
                var response = await client.GetAsync(q);
                response.EnsureSuccessStatusCode();
                
                string jsonResponse = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(jsonResponse);

                if (!obj["items"].HasValues)
                    return $"I was unable to find a question with the tag `{tag}` like `{query}`.";
                else
                    return obj["items"][0]["link"].ToString();
            }
        }

        public static async Task<string> StackExchangeTags(string site, string keywords)
        {
            var baseUri = new Uri("http://api.stackexchange.com/");
            string queryUrl = "2.2/tags?page=1&pagesize=20&order=desc&sort=popular&inname={0}&site={1}";

            string q = string.Format(queryUrl,
                Uri.EscapeDataString(keywords),
                Uri.EscapeDataString(site));

            using (var client = new HttpClient())
            {
                client.BaseAddress = baseUri;
                var response = await client.GetAsync(q);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(jsonResponse);

                if (!obj["items"].HasValues)
                    return $"I was unable to find a tag like `{keywords}` on `{site}`.";
                else
                    return $"```xl\n{string.Join(", ", obj["items"].Select(x => x["name"].ToString()))}```";
            }
        }

        public static async Task<string> Gif(string keywords)
        {
            var getUrl = new Uri("http://api.giphy.com/");

            using (var client = new HttpClient())
            {
                client.BaseAddress = getUrl;
                var response = await client.GetAsync(Uri.EscapeDataString($"v1/gifs/random?api_key=dc6zaTOxFJmzC&tag={Uri.UnescapeDataString(keywords)}"));
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(jsonResponse);
                return obj["data"]["image_original_url"].ToString();
            }
        }

        public static async Task<string> IpLookup(string ip)
        {
            var getUrl = new Uri($"http://ip-api.com/");
            string mapsUrl = "https://www.google.com/maps/@{0},{1},15z";

            using (var client = new HttpClient())
            {
                client.BaseAddress = getUrl;
                var response = await client.GetAsync(Uri.EscapeDataString($"json/{ip}"));
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(jsonResponse);

                if (obj["status"].ToString() == "success")
                {
                    var msg = new List<string>();
                    msg.Add($"**Map**: {string.Format(mapsUrl, obj["lat"].ToString(), obj["lon"].ToString())}");
                    msg.Add("```erlang");
                    msg.Add($" Country: {obj["country"].ToString()}");
                    msg.Add($"  Region: {obj["regionName"].ToString()}");
                    msg.Add($"    City: {obj["city"].ToString()}");
                    msg.Add($"     Zip: {obj["zip"].ToString()}");
                    msg.Add($"Timezone: {obj["timezone"].ToString()}");
                    msg.Add($"     ISP: {obj["isp"].ToString()}");
                    msg.Add("```");

                    return string.Join("\n", msg);
                }
                else
                {
                    return obj["message"].ToString();
                }
            }
        }
    }
}
