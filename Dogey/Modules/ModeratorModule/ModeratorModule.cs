using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
    }
}
