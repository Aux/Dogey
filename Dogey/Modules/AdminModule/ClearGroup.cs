using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.AdminModule
{
    [Module, Name("Moderator")]
    [RequireContext(ContextType.Guild)]
    public class ClearGroup
    {
        private DiscordSocketClient _client;

        public ClearGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("clear")]
        [Description("Clear Dogey's recent messages.")]
        public async Task Clear(IUserMessage msg)
        {
            var self = await _client.GetCurrentUserAsync();

            var messages = (await msg.Channel.GetMessagesAsync(100)).Where(x => x.Author.Id == self.Id);

            if (messages.Count() > 0)
            {
                foreach (var delmsg in messages)
                    await (delmsg as IUserMessage).DeleteAsync();

                var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** of my message(s) in {(msg.Channel as ITextChannel).Mention}");

                await Utility.AutoDeleteMsg(m, 10000);
            }
        }

        [Module("clear"), Name("Moderator")]
        [RequireContext(ContextType.Guild)]
        [RequirePermission(ChannelPermission.ManageMessages)]
        public class SubCommands
        {
            [Command("all")]
            [Description("Clear all recent messages.")]
            public async Task ClearAll(IUserMessage msg, int count = 25)
            {
                await Utility.AutoDeleteMsg(msg, 5000);
                var messages = await msg.Channel.GetMessagesAsync(count);

                if (messages.Count() > 0)
                {
                    await msg.Channel.DeleteMessagesAsync(messages);
                    var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** message(s) in {(msg.Channel as ITextChannel).Mention}");

                    await Utility.AutoDeleteMsg(m, 10000);
                }
                else
                {
                    var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
                }
            }

            [Command("user")]
            [Description("Clear a user's recent messages.")]
            public async Task ClearUser(IUserMessage msg, IUser user, int count = 25)
            {
                await Utility.AutoDeleteMsg(msg, 5000);
                var u = (user as IGuildUser) ?? null;

                if (u == null)
                {
                    await msg.Channel.SendMessageAsync($"I can't find a user like that.");
                }
                else
                {
                    var messages = (await msg.Channel.GetMessagesAsync(count)).Where(x => x.Author.Id == u.Id);

                    if (messages.Count() > 0)
                    {
                        await msg.Channel.DeleteMessagesAsync(messages);
                        var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** message(s) by {u.Mention} in {(msg.Channel as ITextChannel).Mention}");

                        await Utility.AutoDeleteMsg(m, 10000);
                    }
                    else
                    {
                        var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
                    }
                }
            }

            [Command("contains")]
            [Description("Clear recent messages that contain a specific string.")]
            public async Task ClearContains(IUserMessage msg, string keyword, int count = 25)
            {
                await Utility.AutoDeleteMsg(msg, 5000);
                var messages = (await msg.Channel.GetMessagesAsync(count)).Where(x => x.Content.Contains(keyword));

                if (messages.Count() > 0)
                {
                    await msg.Channel.DeleteMessagesAsync(messages);
                    var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** message(s) in {(msg.Channel as ITextChannel).Mention}");

                    await Utility.AutoDeleteMsg(m, 10000);
                }
                else
                {
                    var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
                }
            }

            [Command("bots")]
            [Description("Clear all recent bot messages.")]
            public async Task ClearBots(IUserMessage msg, int count = 25)
            {
                await Utility.AutoDeleteMsg(msg, 5000);
                var messages = (await msg.Channel.GetMessagesAsync(count)).Where(x => x.Author.IsBot);

                if (messages.Count() > 0)
                {
                    await msg.Channel.DeleteMessagesAsync(messages);
                    var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** bot message(s) in {(msg.Channel as ITextChannel).Mention}");

                    await Utility.AutoDeleteMsg(m, 10000);
                }
                else
                {
                    var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
                }
            }

            [Command("embeds")]
            [Description("Clear recent messages that have an embedded link.")]
            public async Task ClearEmbeds(IUserMessage msg, int count = 25)
            {
                await Utility.AutoDeleteMsg(msg, 5000);
                var messages = (await msg.Channel.GetMessagesAsync(count)).Where(x => x.Embeds.Count() > 1);

                if (messages.Count() > 0)
                {
                    await msg.Channel.DeleteMessagesAsync(messages);
                    var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** message(s) with embeds in {(msg.Channel as ITextChannel).Mention}");

                    await Utility.AutoDeleteMsg(m, 10000);
                }
                else
                {
                    var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
                }
            }

            [Command("files")]
            [Description("Clear recent messages that have attachments.")]
            public async Task ClearFiles(IUserMessage msg, int count = 25)
            {
                await Utility.AutoDeleteMsg(msg, 5000);
                var messages = (await msg.Channel.GetMessagesAsync(count)).Where(x => x.Attachments.Count() > 1);

                if (messages.Count() > 0)
                {
                    await msg.Channel.DeleteMessagesAsync(messages);
                    var m = await msg.Channel.SendMessageAsync($"Deleted **{messages.Count()}** message(s) with attachments in {(msg.Channel as ITextChannel).Mention}");

                    await Utility.AutoDeleteMsg(m, 10000);
                }
                else
                {
                    var m = await msg.Channel.SendMessageAsync("I could not find any messages to delete.");
                }
            }
        }
    }
}
