using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    public class MimicModule : ModuleBase
    {
        [Command("mimic")]
        public async Task Mimic([Remainder]IUser user = null)
        {
            try
            {
                var u = user as IGuildUser ?? Context.User as IGuildUser;

                using (var db = new LogContext())
                {
                    var words = db.Messages.Where(x => x.GuildId == Context.Guild.Id && x.AuthorId == u.Id &&
                                !(x.Content.Contains("~") || x.Content.Contains("http"))).Select(x => x.Content.Split(' '));

                    var sentence = new List<string>();
                    for (int i = 0; i < words.Count(); i++)
                    {
                        string selected;
                        if (i == 0)
                            selected = words.GroupBy(x => x.ElementAt(i).ToLower()).OrderBy(x => x.Count()).FirstOrDefault()?.Key;
                        else
                            selected = words.Where(x => x.Count() < i && x.ElementAt(i).ToLower() == sentence.Last().ToLower()).GroupBy(x => x.ElementAt(i)).OrderBy(x => x.Count()).FirstOrDefault()?.Key;

                        if (string.IsNullOrWhiteSpace(selected))
                            break;

                        sentence.Add(selected);
                    }

                    await ReplyAsync($"{u}: {string.Join(" ", sentence)}");
                }
            } catch (Exception ex)
            {
                await ReplyAsync(ex.ToString());
            }
        }
    }
}
