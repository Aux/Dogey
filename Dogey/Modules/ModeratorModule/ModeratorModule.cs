using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module, Name("Moderator")]
    [RequireContext(ContextType.Guild)]
    public class ModeratorModule
    {
        private DiscordSocketClient _client;

        public ModeratorModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("clear")]
        [Description("Dogey will remove his most recent messages.")]
        public async Task Clear(IUserMessage msg)
        {
            var self = await _client.GetCurrentUserAsync();

            var messages = (await msg.Channel.GetMessagesAsync(100)).Where(x => x.Author.Id == self.Id);

            if (messages.Count() > 0)
            {
                await msg.Channel.DeleteMessagesAsync(messages);
                var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** of my message(s) in {(msg.Channel as ITextChannel).Mention}");

                await DogeyTool.AutoDeleteMsg(m, 10000);
            }
        }
    }
}
