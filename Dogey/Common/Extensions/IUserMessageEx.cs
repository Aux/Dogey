using Discord;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Extensions
{
    public static class IUserMessageEx
    {
        public static async Task SaveMessage(this IUserMessage msg)
        {
            var log = new MessageLog(msg);
            using (var db = new DataContext())
            {
                db.MessageLogs.Add(log);
                await db.SaveChangesAsync();
            }
        }
    }
}
