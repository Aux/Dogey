using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey.Modules.SearchModule
{
    [Module, Name("Search")]
    [RequireContext(ContextType.Guild)]
    public class StackExchangeGroup
    {
        private DiscordSocketClient _client;

        public StackExchangeGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("stackexchange"), Alias("question", "q")]
        [Description("Search for tags on a stackexchange site.")]
        [Example("q stackoverflow c# json string")]
        public async Task StackExchange(IUserMessage msg, string site, [Remainder]string keywords)
        {
            var message = await msg.Channel.SendMessageAsync("Searching...");
            var baseUri = new Uri("http://api.stackexchange.com/");
            string queryUrl = "2.2/search/advanced?page=1&pagesize=1&order=desc&min=1&sort=votes&q={0}&answers=1&site={1}";

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
                {
                    await message.ModifyAsync((e) =>
                    {
                        e.Content = $"I was unable to find a question like `{keywords}`.";
                    });
                }
                else
                {
                    await message.ModifyAsync((e) =>
                    {
                        e.Content = obj["items"][0]["link"].ToString();
                    });
                }
            }
        }
    }
}
