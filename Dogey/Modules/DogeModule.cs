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
    [Module]
    [Name("General")]
    public class DogeModule
    {
        private DiscordSocketClient _client;

        public DogeModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("doge")]
        public async Task Doge(IUserMessage msg, params string[] phrase)
        {
            var r = new Random();
            string dogeFile = $"trash\\{r.Next(10000, 99999)}.png";

            string dogeText = null;
            foreach (string line in phrase)
            {
                if (dogeText == null)
                    dogeText += line;
                else
                    dogeText += "/" + line;
            }

            using (var client = new HttpClient())
            {
                Uri doge;
                Uri.TryCreate($"http://dogr.io/wow/{Uri.UnescapeDataString(dogeText)}.png", UriKind.Absolute, out doge);

                client.BaseAddress = doge;
                using (var response = await client.GetAsync(doge))
                {
                    response.EnsureSuccessStatusCode();
                    File.WriteAllBytes(dogeFile, await response.Content.ReadAsByteArrayAsync());
                }
            }

            await msg.Channel.SendFileAsync(dogeFile);
            File.Delete(dogeFile);
        }
    }
}
