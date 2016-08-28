using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dogey.Modules.Info
{
    [Module, Name("Info")]
    [RequireContext(ContextType.Guild)]
    public class InfoModule
    {
        private DiscordSocketClient _client;

        public InfoModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("serverinfo")]
        [Description("Get info about this server.")]
        public async Task ServerInfo(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            var infomsg = new List<string>();
            infomsg.AddRange(new string[]
            {
                "```xl",
                $"  Server: {guild.Name}",
                $"      Id: {guild.Id}",
                $"  Region: {guild.VoiceRegionId}",
                $"   Owner: {(await guild.GetOwnerAsync() as IGuildUser)}",
                $" Created: {guild.CreatedAt.ToString("ddddd, MMM dd yyyy, hh:mm:ss tt")}",
                $"   Users: {guild.GetUsers().Count()}",
                $"Channels: ({guild.GetTextChannels().Count()})text " +
                                  $"({guild.GetVoiceChannels().Count()})voice " +
                                  $"({guild.GetTextChannels().Where(x => x.GetUsers().Count() < guild.GetUsers().Count()).Count()})hidden",
                $"    Icon: {guild.IconUrl}",
                $"   Roles: {string.Join(", ", guild.Roles.Where(x => !x.Name.Contains("@")))}",
                "```"
            });

            if (Globals.Config.IsSelfBot)
                await msg.ModifyAsync((e) =>
                {
                    e.Content = string.Join(Environment.NewLine, infomsg);
                });
            else
                await msg.Channel.SendMessageAsync(string.Join(Environment.NewLine, infomsg));
        }

        [Command("channelinfo")]
        [Description("Get info about this channel.")]
        public async Task ChannelInfo(IUserMessage msg, IChannel chn = null)
        {
            var channel = chn as ITextChannel ?? msg.Channel as ITextChannel ?? null;

            if (channel == null)
            {
                await msg.Channel.SendMessageAsync($"I could not find a channel like `{chn}`");
            }

            var infomsg = new List<string>();
            infomsg.AddRange(new string[]
            {
                "```xl",
                $"       Name: #{channel.Name} ",
                $"         Id: {channel.Id} ",
                $"      Topic: {channel.Topic}",
                $"    Created: {channel.CreatedAt}",
                $"Cached Msgs: {channel.CachedMessages.Count}",
                "```"
            });

            if (Globals.Config.IsSelfBot)
                await msg.ModifyAsync((e) =>
                {
                    e.Content = string.Join(Environment.NewLine, infomsg);
                });
            else
                await msg.Channel.SendMessageAsync(string.Join(Environment.NewLine, infomsg));
        }

        [Command("userinfo")]
        [Description("Get info about this user.")]
        public async Task UserInfo(IUserMessage msg, IUser u = null)
        {
            var user = (msg.Author as IGuildUser) ?? (u as IGuildUser);

            var infomsg = new List<string>();
            infomsg.AddRange(new string[]
            {
                "```xl",
                $"Username: {user}",
                $"Nickname: {user.Nickname}",
                $"      Id: {user.Id}",
                $" Playing: {user.Game?.Name}",
                $" Created: {user.CreatedAt}",
                $"  Joined: {user.JoinedAt}",
                $"  Avatar: {user.AvatarUrl}",
                $"   Roles: {string.Join(", ", user.Roles.Where(x => x.Name != "@everyone"))}",
                "```"
            });

            if (Globals.Config.IsSelfBot)
                await msg.ModifyAsync((e) =>
                {
                    e.Content = string.Join(Environment.NewLine, infomsg);
                });
            else
                await msg.Channel.SendMessageAsync(string.Join(Environment.NewLine, infomsg));
        }

        [Command("botinfo")]
        [Description("Get info about Dogey.")]
        public async Task BotInfo(IUserMessage msg)
        {
            var infomsg = new List<string>();
            infomsg.AddRange(new string[]
            {
                "```xl",
                $" Owner(s): ",
                $"  Library: Discord.Net ({DiscordConfig.Version})",
                $"  Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}",
                $"   Uptime: {GetUptime()}",
                $"Heap Size: {GetHeapSize()} MB",
                $"   Guilds: {(await _client.GetGuildSummariesAsync()).Count()}",
                $" Channels: {(await _client.GetGuildsAsync()).Select(async g => await g.GetChannelsAsync()).Count()}",
                $"    Users: {(await _client.GetGuildsAsync()).Select(async g => await g.GetUsersAsync()).Count()}",
                "```"
            });

            if (Globals.Config.IsSelfBot)
                await msg.ModifyAsync((e) =>
                {
                    e.Content = string.Join(Environment.NewLine, infomsg);
                });
            else
                await msg.Channel.SendMessageAsync(string.Join(Environment.NewLine, infomsg));
        }

        private static string GetUptime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() 
            => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}
