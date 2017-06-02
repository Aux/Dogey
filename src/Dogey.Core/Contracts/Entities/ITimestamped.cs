using System;

namespace Dogey
{
    public interface ITimestamped
    {
        DateTime CreatedAt { get; }
        DateTime UpdatedAt { get; }
    }
}
