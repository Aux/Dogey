using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Name("Config")]
    [Summary("Bot configuration options")]
    public class GuildModule : DogeyModuleBase
    {
        private readonly ConfigManager _manager;
        
        public GuildModule(ConfigManager manager)
        {
            _manager = manager;
        }

        [Command("prefix")]
        [Summary("Check what prefix this guild has configured.")]
        public async Task PrefixAsync()
        {
            var config = await _manager.GetOrCreateConfigAsync(Context.Guild.Id);

            if (config.Prefix == null)
                await ReplyAsync($"This guild's prefix is {Context.Client.CurrentUser.Mention}");
            else
                await ReplyAsync($"This guild's prefix is `{config.Prefix}`");
        }

        [Command("setprefix")]
        [Summary("Change or remove this guild's string prefix.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetPrefixAsync([Remainder]string prefix)
        {
            var config = await _manager.GetOrCreateConfigAsync(Context.Guild.Id);
            await _manager.SetPrefixAsync(config, prefix);

            await ReplyAsync($"This guild's prefix is now `{prefix}`");
        }
    }
}
