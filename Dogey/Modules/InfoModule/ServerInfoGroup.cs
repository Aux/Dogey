using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Info
{
    [Module("serverinfo"), Name("")]
    public class ServerInfoGroup
    {
        private DiscordSocketClient _client;

        public ServerInfoGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("name")]
        [Description("Get the name of this server.")]
        public async Task Name(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.Name);
        }

        [Command("id")]
        [Description("Get the name of this server.")]
        public async Task Id(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.Id.ToString());
        }

        [Command("region")]
        [Description("Get the name of this server.")]
        public async Task Region(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.VoiceRegionId);
        }

        [Command("owner")]
        [Description("Get the name of this server.")]
        public async Task Owner(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync((guild.GetOwnerAsync() as IGuildUser).ToString());
        }

        [Command("created")]
        [Description("Get the name of this server.")]
        public async Task Created(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.CreatedAt.ToString("ddddd, MMM dd yyyy, hh:mm:ss tt"));
        }

        [Command("users")]
        [Description("Get the name of this server.")]
        public async Task Users(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.GetUsers().Count().ToString());
        }

        [Command("channels")]
        [Description("Get the name of this server.")]
        public async Task Channels(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            
            var textchannels = await guild.GetTextChannelsAsync();
            var voicechannels = await guild.GetVoiceChannelsAsync();
            var hiddenchannels = textchannels.Where(x => x.GetUsers().Count() < guild.GetUsers().Count());

            var helpmsg = new List<string>();
            helpmsg.AddRange(new string[]
            {
                "```xl",
                $"Text ({textchannels.Count()}): {string.Join(", ", textchannels.Select(x => x.Name))}",
                $"Voice ({voicechannels.Count()}): {string.Join(", ", voicechannels.Select(x => x.Name))}",
                $"Hidden ({hiddenchannels.Count()}): {string.Join(", ", hiddenchannels.Select(x => x.Name))}",
                "```"
            });

            await msg.Channel.SendMessageAsync(string.Join("\n", helpmsg));
        }

        [Command("icon")]
        [Description("Get the name of this server.")]
        public async Task Icon(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            await msg.Channel.SendMessageAsync(guild.IconUrl);
        }
        
        [Command("emojis")]
        [Description("Get a list of this server's custom emojis.")]
        public async Task Emojis(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
        
            await msg.Channel.SendMessageAsync(string.Join(" ", guild.Emojis.Select(x => $"<:{x.Name}:{x.Id}>")));
        }

        [Command("roles")]
        [Description("Get the name of this server.")]
        public async Task Roles(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            
            await msg.Channel.SendMessageAsync(string.Join(", ", guild.Roles.Where(x => !x.Name.Contains("@"))));
        }

    }
}
