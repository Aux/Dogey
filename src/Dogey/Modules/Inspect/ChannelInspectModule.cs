using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Dogey.Modules.Inspect
{
    [Name("Inspect"), Group("channel")]
    [RequireContext(ContextType.Guild)]
    public class ChannelInspectModule : DogeyModuleBase
    {
        [Command]
        [Summary("Inspect the properties of this channel")]
        public Task ChannelAsync()
            => ChannelAsync(Context.Channel as SocketGuildChannel);
        [Command]
        [Summary("Inspect the properties of a specified channel")]
        public async Task ChannelAsync([Remainder]SocketGuildChannel channel)
        {
            var embed = new EmbedBuilder()
                .WithTitle(channel.Name + $" ({channel.Id})")
                .WithFooter("Created At")
                .WithTimestamp(channel.CreatedAt);

            switch (channel)
            {
                case ITextChannel textChannel:
                    embed.WithDescription(textChannel.Topic)
                        .AddField("Mention", textChannel.Mention, true)
                        .AddField("NSFW", textChannel.IsNsfw, true);
                    break;
                case IVoiceChannel voiceChannel:
                    var users = await voiceChannel.GetUsersAsync().FlattenAsync();
                    string userLimit = voiceChannel.UserLimit?.ToString() ?? "unlimited";
                    string userList = users.Count() > 0 ? string.Join(", ", users.Select(x => x.Mention)) : "*Empty*";

                    embed.AddField($"Users ({users.Count()}/{userLimit})", userList, true);
                    break;
                default:
                    await ReplyAsync("Invalid channel");
                    return;
            }
            
            await ReplyEmbedAsync(embed);
        }
    }
}
