using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Search
{
    [Module, Name("Custom")]
    [RequireContext(ContextType.Guild)]
    public class CustomGroup
    {
        private DiscordSocketClient _client;

        public CustomGroup(DiscordSocketClient client)
        {
            _client = client;
        }

    }
}
