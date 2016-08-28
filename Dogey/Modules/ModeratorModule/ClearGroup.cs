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
    [Module("clear"), Name("Moderator")]
    [RequireContext(ContextType.Guild)]
    [RequirePermission(ChannelPermission.ManageMessages)]
    public class ClearGroup
    {
        private DiscordSocketClient _client;

        public ClearGroup(DiscordSocketClient client)
        {
            _client = client;
        }
        
        [Command("all")]
        [Description("Clear messages from the current channel.")]
        public async Task ClearAll(IUserMessage msg, int count = 25)
        {
            await DogeyTool.AutoDeleteMsg(msg, 5000);
            var messages = await msg.Channel.GetMessagesAsync(count);

            if (messages.Count() > 0)
            {
                await msg.Channel.DeleteMessagesAsync(messages);
                var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** message(s) in {(msg.Channel as ITextChannel).Mention}");

                await DogeyTool.AutoDeleteMsg(m, 10000);
            } else
            {
                var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
            }
        }

        [Command("user")]
        [Description("Clear messages from the current channel.")]
        public async Task ClearUser(IUserMessage msg, IUser user, int count = 25)
        {
            await DogeyTool.AutoDeleteMsg(msg, 5000);
            var u = (user as IGuildUser) ?? null;

            if (u == null)
            {
                await msg.Channel.SendMessageAsync($"I can't find a user like that.");
            } else
            {
                var messages = (await msg.Channel.GetMessagesAsync(count)).Where(x => x.Author.Id == u.Id);

                if (messages.Count() > 0)
                {
                    await msg.Channel.DeleteMessagesAsync(messages);
                    var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** message(s) by {u.Mention} in {(msg.Channel as ITextChannel).Mention}");

                    await DogeyTool.AutoDeleteMsg(m, 10000);
                }
                else
                {
                    var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
                }
            }
        }

        [Command("contains")]
        [Description("Clear messages from the current channel.")]
        public async Task ClearContains(IUserMessage msg, string keyword, int count = 25)
        {
            await DogeyTool.AutoDeleteMsg(msg, 5000);
            var messages = (await msg.Channel.GetMessagesAsync(count)).Where(x => x.Content.Contains(keyword));

            if (messages.Count() > 0)
            {
                await msg.Channel.DeleteMessagesAsync(messages);
                var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** message(s) in {(msg.Channel as ITextChannel).Mention}");

                await DogeyTool.AutoDeleteMsg(m, 10000);
            }
            else
            {
                var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
            }
        }

        [Command("bots")]
        [Description("Clear messages from the current channel.")]
        public async Task ClearBots(IUserMessage msg, int count = 25)
        {
            await DogeyTool.AutoDeleteMsg(msg, 5000);
            var messages = (await msg.Channel.GetMessagesAsync(count)).Where(x => x.Author.IsBot);

            if (messages.Count() > 0)
            {
                await msg.Channel.DeleteMessagesAsync(messages);
                var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** bot message(s) in {(msg.Channel as ITextChannel).Mention}");

                await DogeyTool.AutoDeleteMsg(m, 10000);
            }
            else
            {
                var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
            }
        }

        [Command("embeds")]
        [Description("Clear messages from the current channel.")]
        public async Task ClearEmbeds(IUserMessage msg, int count = 25)
        {
            await DogeyTool.AutoDeleteMsg(msg, 5000);
            var messages = (await msg.Channel.GetMessagesAsync(count)).Where(x => x.Embeds.Count() > 1);

            if (messages.Count() > 0)
            {
                await msg.Channel.DeleteMessagesAsync(messages);
                var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** message(s) with embeds in {(msg.Channel as ITextChannel).Mention}");

                await DogeyTool.AutoDeleteMsg(m, 10000);
            }
            else
            {
                var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
            }
        }

        [Command("files")]
        [Description("Clear messages from the current channel.")]
        public async Task ClearFiles(IUserMessage msg, int count = 25)
        {
            await DogeyTool.AutoDeleteMsg(msg, 5000);
            var messages = (await msg.Channel.GetMessagesAsync(count)).Where(x => x.Attachments.Count() > 1);

            if (messages.Count() > 0)
            {
                await msg.Channel.DeleteMessagesAsync(messages);
                var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** message(s) with attachments in {(msg.Channel as ITextChannel).Mention}");

                await DogeyTool.AutoDeleteMsg(m, 10000);
            }
            else
            {
                var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
            }
        }
    }
}
