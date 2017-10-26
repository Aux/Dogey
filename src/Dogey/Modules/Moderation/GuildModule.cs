using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Name("Config")]
    [Summary("Bot configuration options")]
    public class GuildModule : DogeyModuleBase
    {
        private readonly ConfigDatabase _db;
        
        public GuildModule(IServiceProvider provider)
        {
            _db = provider.GetService<ConfigDatabase>();
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
