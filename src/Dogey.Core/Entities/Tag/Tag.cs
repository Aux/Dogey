using System;
using System.Collections.Generic;

namespace Dogey
{
    public class Tag
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public ulong Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong? ChannelId { get; set; }
        public ulong OwnerId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public bool IsCommand { get; set; }
        public bool IsPublic { get; set; }

        // Foreign Keys
        public List<TagAlias> Aliases { get; set; }
    }
}
