using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    [RequireContext(ContextType.Guild)]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class ConfigModule : DogeyModuleBase
    {
        private readonly RootController _root;

        public ConfigModule(RootController root)
        {
            _root = root;
        }

        [Command("config")]
        public async Task ViewAsync()
        {
            var config = await _root.GetOrCreateConfigAsync(Context.Guild);
            var moduleNames = await _root.GetDisabledModulesAsync(Context.Guild);

            var builder = new EmbedBuilder()
                .AddField("Prefix", config.Prefix ?? "*none*")
                .AddField("Disabled Modules", moduleNames.Count > 0 ? string.Join(", ", moduleNames) : "*none*")
                .WithFooter("Updated")
                .WithTimestamp(config.UpdatedAt);
            await ReplyEmbedAsync(builder);
        }

        [Command("resetprefix")]
        public Task SetPrefixAsync()
            => SetPrefixAsync(null);

        [Command("setprefix")]
        public async Task SetPrefixAsync([Remainder]string prefix)
        {
            var config = await _root.GetOrCreateConfigAsync(Context.Guild);
            config.Prefix = prefix;
            await _root.ModifyAsync(config);
            await ReplyAsync($"The command prefix is now `{prefix}`");
        }

        [Command("enable")]
        public async Task EnableAsync(ModuleInfo module)
        {
            var config = await _root.GetConfigAsync(Context.Guild, module.Name);
            if (config == null)
            {
                await ReplyAsync("This module is already enabled");
                return;
            }

            await _root.DeleteAsync(config);
            await ReplySuccessAsync();
        }

        [Command("disable")]
        public async Task DisableAsync(ModuleInfo module)
        {
            if (!module.Preconditions.Any(x => x is RequireEnabledAttribute))
            {
                await ReplyAsync("This module cannot be disabled");
                return;
            }

            var exists = await _root.ModuleEnabledAsync(Context.Guild, module.Name);
            if (exists)
            {
                await ReplyAsync("This module is already disabled");
                return;
            }
            
            await _root.CreateAsync(new ModuleConfig
            {
                GuildId = Context.Guild.Id,
                ModuleName = module.Name
            });
            await ReplySuccessAsync();
        }
    }
}
