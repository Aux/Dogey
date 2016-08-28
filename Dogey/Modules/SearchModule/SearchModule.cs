using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.SearchModule
{
    [Module, Name("Search")]
    public class SearchModule
    {
        private DiscordSocketClient _client;

        public SearchModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("youtube")]
        [Description("Search youtube with the provided text.")]
        public async Task Youtube(IUserMessage msg, [Remainder]string keywords)
        {
            var message = await msg.Channel.SendMessageAsync("Searching...");
            string result = await SearchWith.Youtube(keywords);
            await message.ModifyAsync((e) =>
            {
                e.Content = result;
            });
        }

        [Command("tags")]
        [Description("Search a Stackexchange site for tags.")]
        public async Task Tags(IUserMessage msg, string site, string keywords)
        {
            var message = await msg.Channel.SendMessageAsync("Searching...");
            string result = await SearchWith.StackExchangeTags(site, keywords);
            await message.ModifyAsync((e) =>
            {
                e.Content = result;
            });
        }

        [Command("superuser")]
        [Description("Search Superuser with the provided text.")]
        public async Task Superuser(IUserMessage msg, string tag, [Remainder]string keywords)
        {
            var message = await msg.Channel.SendMessageAsync("Searching...");
            string result = await SearchWith.StackExchange("superuser", tag, keywords);
            await message.ModifyAsync((e) =>
            {
                e.Content = result;
            });
        }

        [Command("stackoverflow")]
        [Description("Search Stackoverflow with the provided text.")]
        public async Task Stackoverflow(IUserMessage msg, string tag, [Remainder]string keywords)
        {
            var message = await msg.Channel.SendMessageAsync("Searching...");
            string result = await SearchWith.StackExchange("stackoverflow", tag, keywords);
            await message.ModifyAsync((e) =>
            {
                e.Content = result;
            });
        }

        [Command("arqade")]
        [Description("Search Arqade with the provided text.")]
        public async Task Arqade(IUserMessage msg, string tag, [Remainder]string keywords)
        {
            var message = await msg.Channel.SendMessageAsync("Searching...");
            string result = await SearchWith.StackExchange("arqade", tag, keywords);
            await message.ModifyAsync((e) =>
            {
                e.Content = result;
            });
        }

        [Command("gif")]
        [Description("Search for a gif with the provided text")]
        public async Task Gif(IUserMessage msg, [Remainder]string keywords)
        {
            var message = await msg.Channel.SendMessageAsync("Searching...");
            string result = await SearchWith.Gif(keywords);
            await message.ModifyAsync((e) =>
            {
                e.Content = result;
            });
        }
        
        [Command("lookup")]
        [Description("Retrieve information about the provided ip or domain.")]
        public async Task Lookup(IUserMessage msg, [Remainder]string keywords)
        {
            if (Globals.Config.Owner.Contains(msg.Author.Id))
            {
                var message = await msg.Channel.SendMessageAsync("Searching...");
                string result = await SearchWith.IpLookup(keywords);
                await message.ModifyAsync((e) =>
                {
                    e.Content = result;
                });
            }
        }
    }
}
