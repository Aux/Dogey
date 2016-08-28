using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module, Name("General")]
    public class DogeModule
    {
        private DiscordSocketClient _client;

        public DogeModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("doge")]
        public async Task Doge(IUserMessage msg, [Remainder]string phrase)
        {
            var r = new Random();
            string dogeFile = Path.Combine(AppContext.BaseDirectory, $"trash\\{r.Next(10000, 99999)}.png");
            var baseUri = new Uri("http://dogr.io/");
            string queryUrl = "wow/{0}.png";

            string dogeText = phrase.Replace(' ', '/');

            string q = string.Format(queryUrl, Uri.UnescapeDataString(dogeText));

            using (var client = new HttpClient())
            {
                client.BaseAddress = baseUri;
                var response = await client.GetAsync(q);
                response.EnsureSuccessStatusCode();

                File.WriteAllBytes(dogeFile, await response.Content.ReadAsByteArrayAsync());
            }

            await msg.Channel.SendFileAsync(dogeFile);
            File.Delete(dogeFile);
        }
    }
}
