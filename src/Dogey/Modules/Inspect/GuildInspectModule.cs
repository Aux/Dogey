using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Dogey.Modules.Inspect
{
    [Name("Inspect"), Group("guild")]
    public class GuildInspectModule : DogeyModuleBase
    {
        [Command]
        [Summary("Inspect the properties of this guild")]
        public Task GuildAsync()
            => GuildAsync(Context.Guild);
        [Command]
        [Summary("Inspect the properties of a specified guild")]
        public async Task GuildAsync([Remainder]SocketGuild guild)
        {
            var embed = new EmbedBuilder()
                .WithThumbnailUrl(guild.IconUrl)
                .WithTitle(guild.Name + $" ({guild.Id})")
                .WithDescription(string.Join(" ", guild.Emotes.Select(x => x.ToString())))
                .AddField("Owner", guild.Owner.ToString() + $" ({guild.OwnerId})")
                .AddField("Users", guild.MemberCount, true)
                .AddField("Roles", guild.Roles.Count, true)
                .AddField("Channels", $"Text: {guild.TextChannels.Count}\tVoice: {guild.VoiceChannels.Count}", true)
                .WithFooter("Created At")
                .WithTimestamp(guild.CreatedAt);

            await ReplyEmbedAsync(embed);
        }
    }
}
