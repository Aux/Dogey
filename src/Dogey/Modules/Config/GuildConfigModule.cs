using Discord;
using Discord.Commands;
using Dogey.Services;
using System.Threading.Tasks;

namespace Dogey.Modules.Config
{
    [Name("Config"), Group("config")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class GuildConfigModule : DogeyModuleBase
    {
        private readonly PrefixService _prefix;

        public GuildConfigModule(PrefixService prefix)
        {
            _prefix = prefix;
        }

        [Command("prefix")]
        public async Task PrefixAsync()
        {
            var prefix = _prefix.GetPrefix(Context);
            await ReplyAsync($"The prefix for this guild is currently `{prefix}`");
        }
        [Command("prefix")]
        public async Task PrefixAsync([Remainder]string prefix)
        {
            _prefix.SetPrefix(Context.Guild, prefix);
            await ReplyAsync($"Successfully changed the prefix to `{prefix}`");
        }
    }
}
