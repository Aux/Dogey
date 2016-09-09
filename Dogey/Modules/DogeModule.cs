using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module, Name("General")]
    [RequireContext(ContextType.Guild)]
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

        [Command("pat")]
        public async Task Pat(IUserMessage msg, IUser user = null)
        {
            var g = (msg.Channel as IGuildChannel).Guild;
            var u = user as IGuildUser ?? await g.GetCurrentUserAsync();
            
            int count;
            using (var db = new DataContext())
            {
                var pat = db.Pat.Where(x => x.UserId == u.Id).FirstOrDefault();

                if (pat == null)
                {
                    db.Pat.Add(new Pats() { UserId = u.Id });
                    await db.SaveChangesAsync();
                    count = 1;
                }
                else
                {
                    count = pat.Count;
                }
            }

            await msg.Channel.SendMessageAsync($"{u.Username} has been pet {count} times <:auxHappy:213686501089738752>");
        }
    }
}
