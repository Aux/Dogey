using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("clean"), RequireContext(ContextType.Guild)]
    [Remarks("Clean messages from a channel.")]
    public class CleanModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task CleanAsync(int count = 25)
        { 
            var messages = await Context.Channel.GetMessagesAsync(count, CacheMode.AllowDownload).Flatten();
            await Context.Channel.DeleteMessagesAsync(messages);
            var tempMsg = await ReplyAsync($"Deleted **{messages.Count()}** message(s)");
            await Task.Delay(5000);
            await tempMsg.DeleteAsync();
        }

        [Command("contains")]
        public async Task ContainsAsync(string content, int count = 25)
        {
            var messages = await Context.Channel.GetMessagesAsync(count, CacheMode.AllowDownload).Flatten();
            messages = messages.Where(x => x.Content.Contains(content));

            await Context.Channel.DeleteMessagesAsync(messages);
            var tempMsg = await ReplyAsync($"Deleted **{messages.Count()}** message(s) containing **{content}**");
            await Task.Delay(5000);
            await tempMsg.DeleteAsync();
        }

        [Command("user")]
        public async Task UserAsync(SocketUser user, int count = 25)
        {
            var messages = await Context.Channel.GetMessagesAsync(count, CacheMode.AllowDownload).Flatten();
            messages = messages.Where(x => x.Author.Id == user.Id);

            await Context.Channel.DeleteMessagesAsync(messages);
            var tempMsg = await ReplyAsync($"Deleted **{messages.Count()}** message(s) by **{user}**");
            await Task.Delay(5000);
            await tempMsg.DeleteAsync();
        }

        [Command("bots")]
        public async Task BotsAsync(int count = 25)
        {
            var messages = await Context.Channel.GetMessagesAsync(count, CacheMode.AllowDownload).Flatten();
            messages = messages.Where(x => x.Author.IsBot);

            await Context.Channel.DeleteMessagesAsync(messages);
            var tempMsg = await ReplyAsync($"Deleted **{messages.Count()}** bot message(s)");
            await Task.Delay(5000);
            await tempMsg.DeleteAsync();
        }
    }
}
