using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Dogey.Models;
using Dogey.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.InfoModule
{
    [Module, Name("Info")]
    [RequireContext(ContextType.Guild)]
    public class ChannelGroup
    {
        private DiscordSocketClient _client;

        public ChannelGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("channelinfo")]
        [Description("Get information about this user.")]
        public async Task Userinfo(IUserMessage msg, IChannel channel = null)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            var c = channel as IGuildChannel ?? msg.Channel as IGuildChannel;

            int msgcount;
            using (var db = new DataContext())
                msgcount = db.MessageLogs.Where(x => x.ChannelId == c.Id && x.GuildId == guild.Id).Count();

            var infomsg = new List<string>
            {
                "```xl",
                $"     Name: {c} ({c.Id})",
                $"  Created: {c.CreatedAt}",
                $"Overrides: {c.PermissionOverwrites.Count()}",
                $"     Msgs: {msgcount}"
            };

            var t = c as ITextChannel;
            var v = c as IVoiceChannel;
            if (t != null)
            {
                infomsg.AddRange(new string[]
                {
                    $"  Topic: {t.Topic}"
                });
            }
            if (v != null)
            {
                infomsg.AddRange(new string[]
                {
                    $"UserLimit: {v.UserLimit}",
                    $"    Users: {v.GetUsers().Count()}"
                });
            }

            infomsg.Add("```");
            await Utility.SendMessage(msg, string.Join("\n", infomsg));
        }

        [Module("channelinfo"), Name("Info")]
        public class SubCommands
        {
            [Command("roles")]
            public async Task Roles(IUserMessage msg, IChannel channel = null)
            {
                await Task.Delay(1);
            }
        }
    }
}
