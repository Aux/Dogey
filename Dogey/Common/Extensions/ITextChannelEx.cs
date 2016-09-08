using Discord;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Extensions
{
    public static class ITextChannelEx
    {
        public static IEnumerable<CustomCommand> GetCustomCommands(this ITextChannel channel)
        {
            using (var db = new DataContext())
                return db.Commands.Where(x => x.ChannelId == channel.Id && x.GuildId == channel.Guild.Id);
        }
    }
}
