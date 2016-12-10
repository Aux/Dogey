using System;

namespace Dogey
{
    public class DiscordMessage
    {
        public uint Id { get; set; }
        public DateTime Timestamp { get; set; }
        public ulong? GuildId { get; set; }
        public ulong? ChannelId { get; set; }
        public ulong AuthorId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public string Attachment { get; set; }
    }
}
