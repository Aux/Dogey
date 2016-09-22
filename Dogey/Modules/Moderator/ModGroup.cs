using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Dogey.Enums;
using Dogey.Extensions;
using Dogey.Models;
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
            await GuildLog.SendLog(u, g, ModAction.Unban);
        }

        private async Task OnUserBanned(IUser u, IGuild g)
        {
            await GuildLog.SendLog(u, g, ModAction.Ban);
        }

        [Command("reason")]
        [Description("Give a reason for a moderator action.")]
        [Example("reason 13 They broke the rules")]
        public async Task Reason(IUserMessage msg, int number, [Remainder]string reason)
        {
            var guild = (msg.Channel as IGuildChannel).Guild;
            var log = await GuildLog.UpdateLog(msg.Author, guild, number, reason);

            if (log.MsgId != null)
            {
                var channel = await guild.GetLogChannelAsync();
                var message = (await channel.GetMessageAsync((ulong)log.MsgId)) as IUserMessage;
                await message.ModifyAsync(x =>
                {
                    int index = message.Content.LastIndexOf('\n');
                    string newmsg = message.Content.Substring(0, index);

                    x.Content = newmsg + $"\n**Reason:** {reason}\n**Moderator:** {msg.Author}";
                });
            }

            await msg.Channel.SendMessageAsync(":thumbsup:");
        }
    }
}
