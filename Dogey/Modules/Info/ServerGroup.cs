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
    public class ServerGroup
    {
        private DiscordSocketClient _client;

        public ServerGroup(DiscordSocketClient client)
        {
            _client = client;
        }
        
        [Command("serverinfo")]
        [Description("Get information about this server.")]
        public async Task Serverinfo(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            var owner = await guild.GetOwnerAsync() as IGuildUser;

            int msgcount;
            using (var db = new DataContext())
                msgcount = db.MessageLogs.Where(x => x.GuildId == guild.Id).Count();

            var infomsg = new List<string>
            {
                "```xl",
                $"    Name: {guild.Name} ({guild.Id})",
                $"   Owner: {owner} ({owner.Id})",
                $" Created: {guild.CreatedAt.ToString("ddddd, MMM dd yyyy, hh:mm:ss tt")}",
                $"  Region: {guild.VoiceRegionId}",
                $"   Users: {(await guild.GetUsersAsync()).Count()}",
                $"   Roles: {guild.Roles.Count()}",
                $"    Msgs: {msgcount}",
                $"Channels: ({(await guild.GetTextChannelsAsync()).Count()})text " +
                          $"({(await guild.GetVoiceChannelsAsync()).Count()})voice " +
                          $"({(await guild.GetTextChannelsAsync()).Count(x => x.GetUsersAsync().Result.Count() < guild.GetUsersAsync().Result.Count())})hidden",
                $"    Icon: {guild.IconUrl}",
                "```"
            };

            await msg.Channel.SendMessageAsync(string.Join("\n", infomsg));
        }

        [Module("serverinfo"), Name("Info")]
        [RequireContext(ContextType.Guild)]
        public class SubCommands
        {
            [Command("name")]
            [Description("Get this server's name and id.")]
            public async Task Name(IUserMessage msg)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                await msg.Channel.SendMessageAsync($"{guild.Name} ({guild.Id})");
            }

            [Command("region")]
            [Description("Get this server's voice region.")]
            public async Task Region(IUserMessage msg)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                await msg.Channel.SendMessageAsync($"Voice for this guild is hosted in **{guild.VoiceRegionId}**.");
            }

            [Command("owner")]
            [Description("Get this server's owner.")]
            public async Task Owner(IUserMessage msg)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                var owner = await guild.GetOwnerAsync() as IGuildUser;
                await msg.Channel.SendMessageAsync($"This guild's owner is **{owner}** ({owner.Id}).");
            }

            [Command("created")]
            [Description("Get the date and time this server was created.")]
            public async Task Created(IUserMessage msg)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                await msg.Channel.SendMessageAsync($"This guild was created **{guild.CreatedAt.ToString("ddddd, MMM dd yyyy '**at**' hh:mm:ss tt")}**.");
            }

            [Command("users")]
            [Description("Get the total number of users in this server.")]
            public async Task Users(IUserMessage msg)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                var users = await guild.GetUsersAsync();

                var infomsg = new List<string>
                {
                    "```xl",
                    $"  Total: {users.Count()}",
                    $" Online: {users.Count(x => x.Status == UserStatus.Online)}",
                    $"   Idle: {users.Count(x => x.Status == UserStatus.Idle)}",
                    $"Offline: {users.Count(x => x.Status == UserStatus.Offline)}",
                    "```"
                };

                await msg.Channel.SendMessageAsync(string.Join("\n", infomsg));
            }

            [Command("channels")]
            [Description("Get a list of all channels in this server.")]
            public async Task Channels(IUserMessage msg)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

                var textchannels = await guild.GetTextChannelsAsync();
                var voicechannels = await guild.GetVoiceChannelsAsync();
                var hiddenchannels = textchannels.Where(x => x.GetUsers().Count() < guild.GetUsers().Count());

                var infomsg = new List<string>
                {
                "```xl",
                $"Text ({textchannels.Count()}): {string.Join(", ", textchannels.Select(x => x.Name))}",
                $"Voice ({voicechannels.Count()}): {string.Join(", ", voicechannels.Select(x => x.Name))}",
                $"Hidden ({hiddenchannels.Count()}): {string.Join(", ", hiddenchannels.Select(x => x.Name))}",
                "```"
                };

                await msg.Channel.SendMessageAsync(string.Join("\n", infomsg));
            }

            [Command("roles")]
            [Description("Get a list of all roles in this server.")]
            public async Task Roles(IUserMessage msg, int page = 1)
            {
                await Task.Delay(1);
            }

            [Command("icon")]
            [Description("Get this server's icon.")]
            public async Task Icon(IUserMessage msg)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                await msg.Channel.SendMessageAsync(guild.IconUrl);
            }

            [Command("emojis")]
            [Description("Get a list of custom emojis in this server.")]
            public async Task Emojis(IUserMessage msg)
            {
                var guild = (msg.Channel as IGuildChannel)?.Guild;
                await msg.Channel.SendMessageAsync(string.Join(" ", guild.Emojis.Select(x => $"<:{x.Name}:{x.Id}>")));
            }
        }
    }
}
