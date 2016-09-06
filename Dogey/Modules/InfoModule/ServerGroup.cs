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
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            var owner = await guild.GetOwnerAsync() as IGuildUser;
            var channels = await guild.GetChannelsAsync();

            var infomsg = new List<string>
            {
                "```xl",
                $"      Id: {guild.Id}",
                $"    Name: {guild.Name}",
                $"   Owner: {owner} ({owner.Id})",
                $" Created: {guild.CreatedAt.ToString("ddddd, MMM dd yyyy, hh:mm:ss tt")}",
                $"  Region: {guild.VoiceRegionId}",
                $"   Users: {(await guild.GetUsersAsync()).Count()}",
                $"   Roles: {guild.Roles.Count()}",
                $"Channels: ({(await guild.GetTextChannelsAsync()).Count()})text " +
                          $"({(await guild.GetVoiceChannelsAsync()).Count()})voice " +
                          $"({(await guild.GetTextChannelsAsync()).Count(x => x.GetUsersAsync().Result.Count() < guild.GetUsersAsync().Result.Count())})hidden",
                $"    Icon: {guild.IconUrl}",
                "```"
            };

            if (Globals.Config.IsSelfbot)
                await msg.ModifyAsync((e) => e.Content = string.Join("\n", infomsg));
            else
                await msg.Channel.SendMessageAsync(string.Join("\n", infomsg));
        }

        [Module("serverinfo"), Name("Info")]
        [RequireContext(ContextType.Guild)]
        public class SubCommands
        {
            [Command("id")]
            [Description("Get information about this server.")]
            public async Task Id(IUserMessage msg)
            {
                await Task.Delay(1);
            }

            [Command("name")]
            [Description("Get information about this server.")]
            public async Task Name(IUserMessage msg)
            {
                await Task.Delay(1);
            }

            [Command("region")]
            [Description("Get information about this server.")]
            public async Task Region(IUserMessage msg)
            {
                await Task.Delay(1);
            }

            [Command("owner")]
            [Description("Get information about this server.")]
            public async Task Owner(IUserMessage msg)
            {
                await Task.Delay(1);
            }

            [Command("created")]
            [Description("Get information about this server.")]
            public async Task Created(IUserMessage msg)
            {
                await Task.Delay(1);
            }

            [Command("users")]
            [Description("Get information about this server.")]
            public async Task Users(IUserMessage msg)
            {
                await Task.Delay(1);
            }

            [Command("channels")]
            [Description("Get information about this server.")]
            public async Task Channels(IUserMessage msg)
            {
                await Task.Delay(1);
            }

            [Command("roles")]
            [Description("Get information about this server.")]
            public async Task Roles(IUserMessage msg)
            {
                await Task.Delay(1);
            }

            [Command("icon")]
            [Description("Get information about this server.")]
            public async Task Icon(IUserMessage msg)
            {
                await Task.Delay(1);
            }

            [Command("emojis")]
            [Description("Get information about this server.")]
            public async Task Emojis(IUserMessage msg)
            {
                await Task.Delay(1);
            }
        }
    }
}
