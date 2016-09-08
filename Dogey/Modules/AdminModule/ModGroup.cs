using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Dogey.Enums;
using Dogey.Extensions;
using Dogey.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.AdminModule
{
    [Module, Name("Moderator")]
    [RequireContext(ContextType.Guild)]
    [MinPermissions(AccessLevel.ChannelMod)]
    public class ModGroup
    {
        private DiscordSocketClient _client;

        public ModGroup(DiscordSocketClient client)
        {
            _client = client;
            _client.UserBanned += OnUserBanned;
            _client.UserUnbanned += OnUserUnbanned;
        }

        private async Task OnUserUnbanned(IUser u, IGuild g)
        {
            var log = await g.GetLogChannelAsync();

            if (log != null)
            {

            }
        }

        private async Task OnUserBanned(IUser u, IGuild g)
        {
            var log = await g.GetLogChannelAsync();

            if (log != null)
            {

            }
        }

        [Command("reason")]
        [Description("Give a reason for a moderator action.")]
        public async Task Reason(IUserMessage msg, int number, [Remainder]string reason)
        {
            await Task.Delay(1);
        }
    }
}
