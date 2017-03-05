using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("clean"), RequireContext(ContextType.Guild)]
    [Remarks("Clean messages from a channel.")]
    public class CleanModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        public async Task CleanAsync()
        {
            var self = Context.Guild.CurrentUser;
            var messages = (await GetMessageAsync(100)).Where(x => x.Author.Id == self.Id);

            if (self.GetPermissions(Context.Channel as SocketGuildChannel).ManageMessages)
                await DeleteMessagesAsync(messages);
            else
                foreach (var msg in messages)
                    await msg.DeleteAsync();

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s)");
            await DelayDeleteMessageAsync(reply);
        }

        [Command]
        public async Task AllAsync(int history = 25)
        {
            var messages = await GetMessageAsync(history);
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s)");
            await DelayDeleteMessageAsync(reply);
        }

        [Command("user")]
        public async Task UserAsync(SocketUser user, int history = 25)
        {
            var messages = (await GetMessageAsync(history)).Where(x => x.Author.Id == user.Id);
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s) by **{user}**");
            await DelayDeleteMessageAsync(reply);
        }

        [Command("bots")]
        public async Task BotsAsync(int history = 25)
        {
            var messages = (await GetMessageAsync(history)).Where(x => x.Author.IsBot);
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s) by bots");
            await DelayDeleteMessageAsync(reply);
        }

        [Command("webhooks")]
        public async Task WebhooksAsync(int history = 25)
        {
            var messages = (await GetMessageAsync(history)).Where(x => x.IsWebhook);
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s) by webhooks");
            await DelayDeleteMessageAsync(reply);
        }

        [Command("contains")]
        public async Task ContainsAsync(string text, int history = 25)
        {
            var messages = (await GetMessageAsync(history)).Where(x => x.Content.ToLower().Contains(text.ToLower()));
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s) containing `{text}`.");
            await DelayDeleteMessageAsync(reply);
        }

        [Command("attachments")]
        public async Task AttachmentsAsync(int history = 25)
        {
            var messages = (await GetMessageAsync(history)).Where(x => x.Attachments.Count() != 0);
            await DeleteMessagesAsync(messages);

            var reply = await ReplyAsync($"Deleted **{messages.Count()}** message(s) with attachments.");
            await DelayDeleteMessageAsync(reply);
        }
        
        private Task<IEnumerable<IMessage>> GetMessageAsync(int count)
            => Context.Channel.GetMessagesAsync(count).Flatten();

        private Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
            => Context.Channel.DeleteMessagesAsync(messages);

        private async Task DelayDeleteMessageAsync(IMessage message, int ms = 5000)
        {
            await Task.Delay(ms);
            await message.DeleteAsync();
        }
    }
}
