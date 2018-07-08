using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Configs
{
    [Group("channel")]
    public class ChannelConfigModule : DogeyModuleBase
    {
        public ChannelConfigModule(RootController root)
            : base(root) { }

        [Command]
        [Summary("View information about a channel")]
        public Task ChannelAsync()
            => ChannelAsync(Context.Channel);
        [Command]
        [Summary("View information about a channel")]
        public async Task ChannelAsync([Remainder]IChannel channel)
        {
            var embed = new EmbedBuilder()
                .WithTitle(channel.Name + $" ({channel.Id})")
                .WithFooter("Created At")
                .WithTimestamp(channel.CreatedAt);

            if (channel is ITextChannel textChannel)
            {
                embed.WithDescription(textChannel.Topic)
                    .AddField("Mention", textChannel.Mention, true)
                    .AddField("NSFW", textChannel.IsNsfw, true);
            }
            else
            if (channel is IVoiceChannel voiceChannel)
            {
                var users = await voiceChannel.GetUsersAsync().FlattenAsync();
                string userLimit = voiceChannel.UserLimit?.ToString() ?? "unlimited";
                string userList = users.Count() > 0 ? string.Join(", ", users.Select(x => x.Mention)) : "*Empty*";

                embed.AddField($"Users ({users.Count()}/{userLimit})", userList, true);
            }
            
            await ReplyEmbedAsync(embed);
        }

        [Command("configuration"), Alias("config", "cfg")]
        [Summary("Display all configurations for this channel")]
        public async Task ConfigAsync()
        {
            var config = await _root.GetOrCreateConfigAsync(Context.Channel as IGuildChannel);
            var embed = new EmbedBuilder()
                .WithTitle("Channel Defaults")
                .AddField("SuccessEmoji", config.SuccessEmoji)
                .AddField("Banned", config.BannedAt.ToString() ?? "*No*")
                .WithFooter("Updated At")
                .WithTimestamp(config.UpdatedAt);

            await ReplyEmbedAsync(embed);
        }
    }
}
