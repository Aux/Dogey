using Discord;
using Discord.Commands;
using Dogey.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Commands
{
    public class GDPRModule : DogeyModuleBase
    {
        private readonly RootDatabase _db;
        private User _currentUser;

        public GDPRModule(LocaleService locale, RootDatabase db) : base(locale) 
        {
            _db = db;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            _currentUser = _db.Users.SingleOrDefault(x => x.Id == Context.User.Id);
        }

        [Command("exportuser")]
        public async Task ExportUserDataAsync()
        {
            var user = await _db.Users.Include(x => x.Guilds).SingleOrDefaultAsync(x => x.Id == Context.User.Id);
            if (user == null)
            {
                await ReplyAsync(_locale.GetString("gdpr:user_data_null", Context.User, _currentUser.Locale));
                return;
            }

            var directory = Path.Combine(AppContext.BaseDirectory, $"common/temp");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var filePath = Path.Combine(directory, $"{Context.User.Id}-data.json");
            using (var file = File.CreateText(filePath))
            {
                var serializer = new JsonSerializer()
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                serializer.Serialize(file, user);
            }

            await Context.User.SendFileAsync(filePath);
            await ReplyAsync(_locale.GetString("gdpr:user_data_get", Context.User, _currentUser.Locale));
            File.Delete(filePath);
        }
    }
}
