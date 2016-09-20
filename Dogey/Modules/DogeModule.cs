using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Dogey.Enums;
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
        [Ratelimit(30, RateMeasure.Minutes)]
        public async Task Pat(IUserMessage msg, [Remainder]IUser user = null)
        {
            var g = (msg.Channel as IGuildChannel).Guild;
            var u = user as IGuildUser ?? msg.Author as IGuildUser;

            if (msg.Author.Id == u.Id)
            {
                await msg.Channel.SendMessageAsync("You can't pat yourself <:auxSad:213686503509852160>");
                return;
            }
            
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
                    pat.Count++;
                    db.Pat.Update(pat);
                    await db.SaveChangesAsync();
                    count = pat.Count;
                }
            }

            await msg.Channel.SendMessageAsync($"{u.Username} has been pet {count} times <:auxHappy:213686501089738752>");
        }

        [Command("pats")]
        public async Task Pats(IUserMessage msg, [Remainder]IUser user = null)
        {
            var g = (msg.Channel as IGuildChannel).Guild;
            var u = user as IGuildUser ?? msg.Author as IGuildUser;
            
            using (var db = new DataContext())
            {
                int? pats = db.Pat.Where(x => x.UserId == u.Id).FirstOrDefault()?.Count;
                if (pats != null)
                    await msg.Channel.SendMessageAsync($"{u.Username} has been pet {pats} times <:auxHappy:213686501089738752>");
                else
                    await msg.Channel.SendMessageAsync($"{u.Username} has been pet 0 times <:auxSad:213686503509852160>");
            }
        }
    }
}
