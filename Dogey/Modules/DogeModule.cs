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
        
        [Command("archive")]
        public async Task Archive(IUserMessage msg, int count)
        {
            var messages = await msg.Channel.GetMessagesAsync(count);

            if (messages.Count() > 0)
            {
                var archive = new List<string>();

                archive.Add("Timestamp, ChannelId, AuthorId, AuthorName, Message, Pinned, TTS, Attachments");
                foreach (var m in messages)
                    archive.Add($"\"{m.Timestamp}\", \"{m.Channel.Id}\", \"{m.Author.Id}\", \"{m.Author.Username}\", \"{m.Content}\", \"{m.IsPinned}\", \"{m.IsTTS}\", \"{m.Attachments.Count()}\"");

                using (var stream = new StreamWriter(new MemoryStream()))
                {
                    stream.Write(string.Join("\n", archive));
                    await msg.Channel.SendFileAsync(stream.BaseStream, $"Archive {(msg.Channel as IGuildChannel).Name}.csv");
                    await Task.Delay(10000);
                }
            }
            else
            {
                var m = await msg.Channel.SendMessageAsync("I could not find any messages to archive.");
            }
        }
    }
}
