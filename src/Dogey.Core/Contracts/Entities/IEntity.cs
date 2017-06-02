namespace Dogey
{
    public interface IEntity<TId>
    {
        TId Id { get; }
    }
}
