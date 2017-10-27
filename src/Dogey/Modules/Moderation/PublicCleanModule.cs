using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Moderation
{
    [Group("clean"), Name("Clean")]
    [Summary("Clean messages from a channel.")]
    [RequireContext(ContextType.Guild)]
    public class PublicCleanModule : DogeyModuleBase
    {
        [Command]
        [Summary("Clean all of Dogey's recent messages")]
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

        private Task<IEnumerable<IMessage>> GetMessageAsync(int count)
            => Context.Channel.GetMessagesAsync(count).Flatten();

        private Task DeleteMessagesAsync(IEnumerable<IMessage> messages)
            => Context.Channel.DeleteMessagesAsync(messages);

        private async Task DelayDeleteMessageAsync(IMessage message, int ms = 10000)
        {
            await Task.Delay(ms);
            await message.DeleteAsync();
        }
    }
}
