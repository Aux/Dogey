namespace Dogey
{
    public interface IOwnable<TId>
    {
        TId OwnerId { get; }
    }
}
