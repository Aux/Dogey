using System;

namespace Dogey
{
    public interface ITag<T>
    {
        DateTime CreatedAt { get; }
        DateTime? UpdatedAt { get; }
        string Name { get; }
        string Content { get; }
        T OwnerId { get; }
        T GuildId { get; }
    }
}
