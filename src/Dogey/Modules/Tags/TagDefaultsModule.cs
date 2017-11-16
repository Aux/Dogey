using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.Modules.Tags
{
    [Group("tag default"), Alias("tag defaults", "tagdefaults", "tagdefault", "tagdef", "tagdefs")]
    [Summary("Manage your personal default tag behavior")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class TagDefaultsModule : DogeyModuleBase
    {
        private readonly TagManager _manager;

        public TagDefaultsModule(TagManager manager)
        {
            _manager = manager;
        }

        [Command]
        public async Task DefaultAsync()
        {
            var defaults = await _manager.GetOrCreateDefaultsAsync(Context.User.Id);

            var embed = new EmbedBuilder()
                .WithTitle("Your current tag defaults are:")
                .WithDescription($"```IsCommand           {defaults.IsCommand}\n" +
                                 $"IsCurrentChannel    {defaults.IsCurrentChannel}```");

            await ReplyAsync(embed);
        }

        [Command("iscommand")]
        public async Task IsCommandAsync(bool toggle)
        {
            var defaults = await _manager.GetOrCreateDefaultsAsync(Context.User.Id);
            defaults.IsCommand = toggle;
            await _manager.UpdateAsync(defaults);

            var embed = new EmbedBuilder();
            if (toggle)
                embed.WithDescription($"Created tags will now automatically be a command.");
            else
                embed.WithDescription($"Created tags will no longer automatically be a command.");

            await ReplyAsync(embed);
        }

        [Command("iscurrentchannel")]
        public async Task IsCurrentChannelAsync(bool toggle)
        {
            var defaults = await _manager.GetOrCreateDefaultsAsync(Context.User.Id);
            defaults.IsCurrentChannel = toggle;
            await _manager.UpdateAsync(defaults);

            var embed = new EmbedBuilder();
            if (toggle)
                embed.WithDescription($"Created tags will now default to the current channel.");
            else
                embed.WithDescription($"Created tags will no longer default to the current channel.");

            await ReplyAsync(embed);
        }
    }

}
