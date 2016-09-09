using Discord;
using Dogey.Enums;
using Dogey.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Models
{
    public class GuildLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public ulong? GuildId { get; set; }
        public ulong? UserId { get; set; }
        public ulong? MsgId { get; set; }
        public ModAction Action { get; set; }
        public int CaseNum { get; set; }
        public string Reason { get; set; }
        
        public async Task<IGuildUser> User(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            return await guild.GetUserAsync((ulong)UserId);
        }

        public static async Task SendLog(IUser u, IGuild g, ModAction m)
        {
            var channel = await g.GetLogChannelAsync();
            string prefix = await g.GetCustomPrefixAsync();

            int casenum;
            using (var db = new DataContext())
                casenum = db.GuildLogs.Count(x => x.GuildId == g.Id);

                ulong? msgId = null;
            if (channel != null)
            {
                string msg = $"**{Enum.GetName(typeof(ModAction), m)}** | Case #{casenum}\n" +
                             $"**User:** {u} ({u.Id})\n" +
                             $"`Responsible moderator, please type {prefix}reason <case> <reason>`";

                msgId = (await channel.SendMessageAsync(msg)).Id;
            }

            using (var db = new DataContext())
            {
                var log = new GuildLog()
                {
                    GuildId = g.Id,
                    MsgId = msgId,
                    Action = m,
                    CaseNum = casenum++
                };

                db.GuildLogs.Add(log);
                await db.SaveChangesAsync();
            }
        }

        public static async Task<GuildLog> UpdateLog(IUser u, IGuild g, int casenum, string reason)
        {
            using (var db = new DataContext())
            {
                var log = db.GuildLogs.Where(x => x.GuildId == g.Id && x.CaseNum == casenum).FirstOrDefault();
                log.Reason = reason;
                log.UserId = u.Id;

                db.GuildLogs.Update(log);
                await db.SaveChangesAsync();
                return log;
            }
        }
    }
}
