using Discord.Commands;
using System;

namespace Dogey
{
    public class TagLog : IEntity<ulong>
    {
        public DateTime Timestamp { get; private set; } = DateTime.UtcNow;
        public ulong Id { get; private set; }
        public ulong GuildId { get; private set; }
        public ulong ChannelId { get; private set; }
        public ulong UserId { get; private set; }
        public ulong TagId { get; private set; }

        // Foreign Keys
        public Tag Tag { get; private set; }

        public TagLog() { }
        public TagLog(ulong tagId, ICommandContext context)
        {
            TagId = tagId;
            GuildId = context.Guild.Id;
            ChannelId = context.Channel.Id;
            UserId = context.User.Id;
        }
    }
}
