using System;

namespace Dogey
{
    public interface IPat<TId>
    {
        DateTime Timestamp { get; set; }
        TId SenderId { get; set; }
        string SenderName { get; set; }
        TId ReceiverId { get; set; }
    }
}
