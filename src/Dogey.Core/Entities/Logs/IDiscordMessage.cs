using System;

namespace Dogey
{
    public interface IDiscordMessage<TId>
    {
        DateTime CreatedAt { get; }
        DateTime? DeletedAt { get; }
        TId GuildId { get;}
        TId ChannelId { get; }
        TId MessageId { get; }
        TId AuthorId { get; }
        string Name { get; }
        string Content { get; }
    }
}
