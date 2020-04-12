using Discord.Commands;
using Dogey.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Commands
{
    [Group("options")]
    public class UserOptionsModule : DogeyModuleBase
    {
        private readonly RootDatabase _db;
        private User _currentUser;

        public UserOptionsModule(LocaleService locale, RootDatabase db) : base(locale) 
        {
            _db = db;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            _currentUser = _db.Users.SingleOrDefault(x => x.Id == Context.User.Id);
        }

        [Command]
        public Task GetOptionsAsync()
        {
            return Task.CompletedTask;
        }

        [Command("locale"), RequireNode("options.locale")]
        public async Task GetOrSetLocaleAsync(string locale = null)
        {
            var paramDict = new Dictionary<string, object>();

            if (string.IsNullOrWhiteSpace(locale))
            {
                var value = _locale.GetDefaultLocale();
                paramDict.Add("Code", value.Key);

                await ReplyAsync(_locale.GetString("options:locale_get", paramDict, _currentUser.Locale));
                return;
            }

            if (!_locale.LocaleIds.Contains(locale))
            {
                paramDict.Add("Code", locale);
                await ReplyAsync(_locale.GetString("options:locale_not_found", paramDict, _currentUser.Locale));
                return;
            }

            var user = await _db.Users.SingleOrDefaultAsync(x => x.Id == Context.User.Id);
            if (user == null)
            {
                user = new User
                {
                    Id = Context.User.Id
                };
            }

            user.Locale = locale;
            _db.Update(user);

            paramDict.Add("Code", locale);
            await ReplyAsync(_locale.GetString("options:locale_set", paramDict, user.Locale));
        }

        [Command("locales"), RequireNode("options.locale")]
        public async Task GetLocalesAsync()
        {
            await ReplyAsync(string.Join(_locale.GetString("formatting:list", null, _currentUser.Locale), _locale.LocaleIds));
        }
    }
}
