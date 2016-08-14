using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module]
    public class InfoBase
    {
        private DiscordSocketClient _client;

        public InfoBase(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("serverinfo")]
        [Description("Get info about this server.")]
        public async Task ServerInfo(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            var infomsg = new List<string>();
            infomsg.AddRange(new string[]
            {
                "```xl",
                $"  Server: {guild.Name}",
                $"      Id: {guild.Id}",
                $"  Region: {guild.VoiceRegionId}",
                $"   Owner: {(await guild.GetOwnerAsync() as IGuildUser) ?? null}",
                $" Created: {guild.CreatedAt.ToString("ddddd, MMM dd yyyy, hh:mm:ss tt")}",
                $"   Users: {guild.GetUsers().Count()}",
                $"Channels: ({guild.GetTextChannels().Count()})text " +
                                  $"({guild.GetVoiceChannels().Count()})voice " +
                                  $"({guild.GetTextChannels().Where(x => x.GetUsers().Count() < guild.GetUsers().Count()).Count()})hidden",
                $"    Icon: {guild.IconUrl}",
                $"  Emojis: {string.Join(", ", guild.Emojis)}",
                $"   Roles: {string.Join(", ", guild.Roles.Where(x => !x.Name.Contains("@")))}",
                "```"
            });
            
            await msg.Channel.SendMessageAsync(string.Join(Environment.NewLine, infomsg));
        }

        [Command("userinfo")]
        [Description("Get info about this user.")]
        public async Task UserInfo(IMessage msg, string user = null)
        {
            await msg.Channel.SendMessageAsync("Not implemented.");
        }

        [Command("botinfo")]
        [Description("Get info about Dogey.")]
        public async Task BotInfo(IMessage msg)
        {
            await msg.Channel.SendMessageAsync("Not implemented.");
        }
    }
}
