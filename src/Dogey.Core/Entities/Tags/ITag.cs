using System;
using System.Collections.Generic;

namespace Dogey
{
    public interface ITag<TId, TAlias>
    {
        DateTime CreatedAt { get; }
        DateTime? UpdatedAt { get; }
        TAlias Aliases { get; }
        string Content { get; }
        TId OwnerId { get; }
        TId GuildId { get; }
    }
}
