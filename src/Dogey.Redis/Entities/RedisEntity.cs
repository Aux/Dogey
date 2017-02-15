namespace Dogey.Redis
{
    public class RedisEntity<T> : IEntity<T>
    {
        public T Id { get; set; }
    }
}
