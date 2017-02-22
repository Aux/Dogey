using System;

namespace Dogey
{
    public interface IDiscordReaction
    {
        DateTime CreatedAt { get; }
        ulong MessageId { get; }
        ulong AuthorId { get; }
        ulong? EmojiId { get; }
        string EmojiName { get; }
        DateTime? DeletedAt { get; }
    }
}
