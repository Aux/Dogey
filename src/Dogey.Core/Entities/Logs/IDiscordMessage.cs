using System;

namespace Dogey
{
    public interface IDiscordMessage
    {
        DateTime CreatedAt { get; }
        ulong? GuildId { get;}
        ulong ChannelId { get; }
        ulong MessageId { get; }
        ulong AuthorId { get; }
        string Name { get; }
        string Content { get; }
        DateTime? DeletedAt { get; }
        bool? IsDeleted { get; }
    }
}
