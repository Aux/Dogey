using System;
using System.Collections.Generic;

namespace Dogey
{
    public interface ITag<T>
    {
        DateTime CreatedAt { get; }
        DateTime? UpdatedAt { get; }
        List<string> Names { get; }
        string Content { get; }
        T OwnerId { get; }
        T GuildId { get; }
    }
}
