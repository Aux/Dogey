using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    public class MimicModule : ModuleBase
    {
        [Command("mimic")]
        public async Task Mimic([Remainder]IUser user = null)
        {
            var u = user as IGuildUser ?? Context.User as IGuildUser;

            IEnumerable<DiscordMessage> messages;
            using (var db = new LogContext())
                messages = db.Messages.Where(x => x.GuildId == Context.Guild.Id && x.AuthorId == u.Id);

            var contents = messages.Select(x => x.Content.Split(' '));

            var builder = new StringBuilder();
            for (int i = 0; i < 128; i++)
            {
                var words = contents.Where(x => !string.IsNullOrWhiteSpace(x?[i]))
                                    .Select(x => x?[i])
                                    .GroupBy(x => x)
                                    .OrderBy(x => x.Count());

                var selected = words.FirstOrDefault()?.First();
                if (string.IsNullOrWhiteSpace(selected))
                    break;
                else
                    builder.Append($" {selected}");
            }

            await ReplyAsync($"{u}: {builder.ToString()}");
        }
    }
}
