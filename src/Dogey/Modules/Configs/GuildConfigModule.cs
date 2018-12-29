using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Configs
{
    [Group("guild")]
    public class GuildConfigModule : DogeyModuleBase
    {
        public GuildConfigModule(RootController root)
            : base(root) { }

        [Command]
        [Summary("View information about a guild")]
        public Task GuildAsync()
            => GuildAsync(Context.Guild);
        [Command]
        [Summary("View information about a guild")]
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
        
        [Command("configuration"), Alias("config", "cfg")]
        [Summary("Display all configurations for this guild")]
        public async Task ConfigAsync()
        {
            var config = await _root.GetOrCreateConfigAsync(Context.Guild);
            var embed = new EmbedBuilder()
                .WithTitle("Guild Defaults")
                .AddField("Prefix", config.Prefix ?? "*none*", true)
                .AddField("Currency", config.CurrencyName ?? "point", true)
                .WithFooter("Updated At")
                .WithTimestamp(config.UpdatedAt);

            await ReplyEmbedAsync(embed);
        }

        [Command("prefix"), Alias("p")]
        [Summary("View the guild's command prefix")]
        public async Task PrefixAsync()
        {
            string prefix = await _root.GetPrefixAsync(Context.Guild);
            if (string.IsNullOrWhiteSpace(prefix))
                await ReplyAsync($"Prefix: `{prefix}`");
            else
                await ReplyAsync("No prefix is currently set");
        }
        [Command("setprefix"), Alias("setp")]
        [Summary("Change the guild's command prefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetPrefixAsync([Remainder]string prefix = null)
        {
            var config = await _root.GetOrCreateConfigAsync(Context.Guild);
            config.Prefix = prefix;
            await ReplySuccessAsync();
        }

        [Command("successemoji"), Alias("successemote")]
        [Summary("View the guild's success emoji")]
        public async Task SuccessEmojiAsync()
        {
            var emoji = await _root.GetSuccessEmojiAsync(Context.Guild);
            if (emoji != null)
                await ReplyAsync("Success Emoji: " + emoji.ToString());
            else
                await ReplyAsync("Success Emoji: " + new Emoji(DogeyConstants.DefaultSuccessEmoji).ToString());
        }
        [Command("setsuccessemoji"), Alias("setsuccessemote")]
        [Summary("Change the guild's success emoji")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetSuccessEmojiAsync(IEmote emote)
        {
            var config = await _root.GetOrCreateConfigAsync(Context.Guild);
            config.SuccessEmoji = emote.ToString();
            await ReplySuccessAsync();
        }
    }
}
