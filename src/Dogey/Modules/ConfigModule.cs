using Discord;
using Discord.Commands;
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
            await Task.Delay(0);
        }

        [Command("resetprefix")]
        public Task SetPrefixAsync()
            => SetPrefixAsync(null);

        [Command("setprefix")]
        public async Task SetPrefixAsync([Remainder]string prefix)
        {
            var config = await _root.GetConfigAsync(Context.Guild);
            config.Prefix = prefix;
            await _root.ModifyAsync(config);
            await ReplyAsync($"The command prefix is now `{prefix}`");
        }

        [Command("enable")]
        public async Task EnableAsync(ModuleInfo module)
        {
            var config = new ModuleConfig
            {
                GuildId = Context.Guild.Id,
                ModuleName = module.Name
            };

            await _root.CreateAsync(config);
            await ReplySuccessAsync();
        }

        [Command("disable")]
        public async Task DisableAsync(ModuleInfo module)
        {
            var config = await _root.GetConfigAsync(Context.Guild, module.Name);
            await _root.DeleteAsync(config);
            await ReplySuccessAsync();
        }
    }
}
