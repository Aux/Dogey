using System.Threading.Tasks;

namespace Dogey.Redis
{
    public abstract class RedisEntity : IEntity<ulong>
    {
        public ulong Id { get; set; }

        public abstract Task SaveChangesAsync();
    }
}
