using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Dogey.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module, Name("Owner")]
    [MinPermissions(AccessLevel.Owner)]
    public class OwnerModule
    {
        private DiscordSocketClient _client;

        public OwnerModule(DiscordSocketClient client)
        {
            _client = client;
        }
        
        [Command("say")]
        [Description("Make Dogey say something.")]
        public async Task Say(IUserMessage msg, [Remainder]string text)
        {
            await msg.Channel.SendMessageAsync(text);
        }

        [Module("set"), Name("Owner")]
        [MinPermissions(AccessLevel.Owner)]
        public class SubCommands
        {
            private DiscordSocketClient _client;

            public SubCommands(DiscordSocketClient client)
            {
                _client = client;
            }
            
            [Command("username"), Alias("user", "u")]
            [Description("Change Dogey's username.")]
            public async Task Username(IUserMessage msg, [Remainder]string name)
            {
                var self = await _client.GetCurrentUserAsync();

                await self.ModifyAsync(e => e.Username = name);
                await msg.Channel.SendMessageAsync($"I changed my username to `{name}`.");
            }

            [RequireContext(ContextType.Guild)]
            [Command("nickname"), Alias("nick", "n")]
            [Description("Change Dogey's nickname.")]
            public async Task Nickname(IUserMessage msg, [Remainder]string name)
            {
                var guild = (msg.Channel as ITextChannel)?.Guild;
                var self = await guild.GetCurrentUserAsync();

                await self.ModifyAsync(e => e.Nickname = name);
                await msg.Channel.SendMessageAsync($"I changed my nickname to `{name}`.");
            }

            [Command("avatar"), Alias("icon", "a", "i")]
            [Description("Change Dogey's avatar.")]
            public async Task Avatar(IUserMessage msg, string url)
            {
                var self = await _client.GetCurrentUserAsync();
                var q = Uri.EscapeDataString(url);

                using (var client = new HttpClient())
                {
                    var imagestream = await client.GetStreamAsync(q);
                    await self.ModifyAsync(e => e.Avatar = imagestream);
                }
                
                await msg.Channel.SendMessageAsync($"My avatar has changed successfully.");
            }

            [Command("game"), Alias("g")]
            [Description("Change Dogey's game.")]
            public async Task Game(IUserMessage msg, [Remainder]string game)
            {
                var self = await _client.GetCurrentUserAsync();

                await self.ModifyStatusAsync(x => x.Game = new Game(game));
                await msg.Channel.SendMessageAsync($"I am now playing `{game}`.");
            }
        }
    }
}
