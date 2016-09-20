using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey.Modules.Search
{
    [Module, Name("Search")]
    [RequireContext(ContextType.Guild)]
    public class ImageGroup
    {
        private DiscordSocketClient _client;

        public ImageGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("gif")]
        [Description("Search for a gif with the provided keywords.")]
        public async Task Youtube(IUserMessage msg, [Remainder]string keywords)
        {
            var message = await msg.Channel.SendMessageAsync("Searching...");
            var getUrl = new Uri("http://api.giphy.com/");

            using (var client = new HttpClient())
            {
                client.BaseAddress = getUrl;
                var response = await client.GetAsync(Uri.EscapeDataString($"v1/gifs/random?api_key=dc6zaTOxFJmzC&tag={Uri.UnescapeDataString(keywords)}"));
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();

                var obj = JObject.Parse(jsonResponse);
                await message.ModifyAsync((e) =>
                {
                    e.Content = obj["data"]["image_original_url"].ToString();
                }); 
            }
        }
    }
}
