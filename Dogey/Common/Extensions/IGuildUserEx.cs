using Discord;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Extensions
{
    public static class IGuildUserEx
    {
        public static IEnumerable<CustomCommand> GetCustomCommands(this IGuildUser user, IGuild guild)
        {
            using (var db = new DataContext())
                return db.Commands.Where(x => x.OwnerId == user.Id && x.GuildId == guild.Id);
        }
    }
}
