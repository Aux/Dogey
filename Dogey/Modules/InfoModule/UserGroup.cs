using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.InfoModule
{
    [Module, Name("Info")]
    [RequireContext(ContextType.Guild)]
    public class UserGroup
    {
        private DiscordSocketClient _client;

        public UserGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("userinfo")]
        [Description("Get information about this user.")]
        public async Task Userinfo(IUserMessage msg, IUser user = null)
        {
            await Task.Delay(1);
        }

        [Module("userinfo"), Name("Info")]
        public class SubCommands
        {

        }
    }
}
