using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module, Name("Owner")]
    public class OwnerModule
    {
        private DiscordSocketClient _client;

        public OwnerModule(DiscordSocketClient client)
        {
            _client = client;
        }
        
        [Command("say")]
        [Description("Make Dogey say something.")]
        public async Task Username(IUserMessage msg, [Remainder]string text)
        {
            await Task.Delay(1);
        }

        [Module("set"), Name("Owner")]
        public class SubCommands
        {
            [Command("username"), Alias("user", "u")]
            [Description("Change Dogey's username.")]
            public async Task Username(IUserMessage msg, [Remainder]string name)
            {
                await Task.Delay(1);
            }

            [Command("nickname"), Alias("nick", "n")]
            [Description("Change Dogey's nickname.")]
            public async Task Nickname(IUserMessage msg, [Remainder]string name)
            {
                await Task.Delay(1);
            }

            [Command("avatar"), Alias("icon", "a", "i")]
            [Description("Change Dogey's avatar.")]
            public async Task Avatar(IUserMessage msg, string url)
            {
                await Task.Delay(1);
            }

            [Command("game"), Alias("g")]
            [Description("Change Dogey's game.")]
            public async Task Game(IUserMessage msg, [Remainder]string game)
            {
                await Task.Delay(1);
            }
        }
    }
}
