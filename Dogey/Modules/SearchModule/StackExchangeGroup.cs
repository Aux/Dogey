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
    [RequireContext(ContextType.Guild)]
    public class StackExchangeGroup
    {
        private DiscordSocketClient _client;

        public StackExchangeGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("stackexchange")]
        [Description("Search for tags on a stackexchange site.")]
        public async Task StackExchange(IUserMessage msg, string site, string keyword)
        {
            await Task.Delay(1);
        }
    }
}
