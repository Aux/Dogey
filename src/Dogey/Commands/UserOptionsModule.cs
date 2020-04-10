using Discord.Commands;
using Dogey.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Commands
{
    [Group("options")]
    public class UserOptionsModule : DogeyModuleBase
    {
        public UserOptionsModule(LocaleService locale) : base(locale) { }

        [Command]
        public Task GetOptionsAsync()
        {
            return Task.CompletedTask;
        }

        [Command("locale")]
        public async Task GetOrSetLocaleAsync(string locale = null)
        {
            if (string.IsNullOrWhiteSpace(locale))
            {
                // Return user's locale name and code
                return;
            }

            // Check if provided locale has a valid file
            // Save user's new locale
            // Reply with confirmation
            await ReplyAsync(_locale.GetString("options:locale_set"));
        }
    }
}
