using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Info
{
    [Module("serverinfo")]
    [Description("Info")]
    public class ServerInfo
    {
        private DiscordSocketClient _client;

        public ServerInfo(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("name")]
        [Description("Get the name of this server.")]
        public async Task Name(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.Name);
        }

        [Command("id")]
        [Description("Get the name of this server.")]
        public async Task Id(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.Id.ToString());
        }

        [Command("region")]
        [Description("Get the name of this server.")]
        public async Task Region(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.VoiceRegionId);
        }

        [Command("owner")]
        [Description("Get the name of this server.")]
        public async Task Owner(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync((guild.GetOwnerAsync() as IGuildUser).ToString());
        }

        [Command("created")]
        [Description("Get the name of this server.")]
        public async Task Created(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.CreatedAt.ToString("ddddd, MMM dd yyyy, hh:mm:ss tt"));
        }

        [Command("usercount")]
        [Description("Get the name of this server.")]
        public async Task UserCount(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.GetUsers().Count().ToString());
        }

        [Command("channelcount")]
        [Description("Get the name of this server.")]
        public async Task ChannelCount(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync($"{ guild.GetTextChannels().Count()})text " +
                                  $"({guild.GetVoiceChannels().Count()})voice " +
                                  $"({guild.GetTextChannels().Where(x => x.GetUsers().Count() < guild.GetUsers().Count()).Count()})hidden");
        }

        [Command("icon")]
        [Description("Get the name of this server.")]
        public async Task Icon(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.IconUrl);
        }
        
        [Command("emojis")]
        [Description("Get a list of this server's custom emojis.")]
        public async Task Emojis(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
        
            await msg.Channel.SendMessageAsync(string.Join(" ", guild.Emojis.Select(x => $"<:{x.Name}:{x.Id}>")));
        }

        [Command("roles")]
        [Description("Get the name of this server.")]
        public async Task Roles(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            
            await msg.Channel.SendMessageAsync(string.Join(", ", guild.Roles.Where(x => !x.Name.Contains("@"))));
        }

    }
}
