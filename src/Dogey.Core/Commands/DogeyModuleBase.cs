using Discord;
using Discord.Commands;
using Dogey.Services;
using System.Threading.Tasks;

namespace Dogey.Commands
{
    public abstract class DogeyModuleBase : ModuleBase<DogeyCommandContext>
    {
        protected readonly LocaleService _locale;

        public DogeyModuleBase(LocaleService locale)
        {
            _locale = locale;
        }

        public Task ReplyAsync(Embed embed, RequestOptions options = null)
            => ReplyAsync("", false, embed, options);
    }
}
