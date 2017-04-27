using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.SQLite.Modules
{
    [Name("Config")]
    public class GuildModule : ModuleBase<SocketCommandContext>
    {
        private ConfigDatabase _db;

        protected override void BeforeExecute()
        {
            _db = new ConfigDatabase();
        }

        protected override void AfterExecute()
        {
            _db.Dispose();
        }

        [Command("prefix")]
        [Summary("Check what prefix this guild has configured.")]
        public async Task PrefixAsync()
        {
            var config = await _db.GetConfigAsync(Context.Guild.Id);

            if (config.Prefix == null)
                await ReplyAsync("This guild has no prefix");
            else
                await ReplyAsync($"This guild's prefix is `{config.Prefix}`");
        }

        [Command("setprefix")]
        [Summary("Change or remove this guild's string prefix.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task SetPrefixAsync([Remainder]string prefix)
        {
            var config = await _db.GetConfigAsync(Context.Guild.Id);
            await _db.SetPrefixAsync(config, prefix);

            await ReplyAsync($"This guild's prefix is now `{prefix}`");
        }
    }
}
