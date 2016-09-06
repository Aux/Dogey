using Discord;
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
        public ulong? ChannelId { get; set; }
        public ulong UserId { get; set; }
        public int? CommandId { get; set; }

        public GuildLog(IMessage msg)
        {
            GuildId = (msg.Channel as IGuildChannel)?.Guild.Id;
            ChannelId = msg.Channel?.Id;
            UserId = msg.Author.Id;
        }

        public async Task<ITextChannel> Channel(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            return await guild.GetTextChannelAsync((ulong)ChannelId);
        }

        public async Task<IGuildUser> User(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            return await guild.GetUserAsync(UserId);
        }

        public CustomCommand Command()
        {
            return null;
            //using (var db = new DataContext())
            //    return db.Commands.Where(x => x.Id == CommandId).FirstOrDefault();
        }
    }
}
