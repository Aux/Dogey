using System.Threading.Tasks;

namespace Dogey.Redis
{
    public abstract class RedisEntity : IDbEntity<ulong>
    {
        public ulong Id { get; set; }

        public abstract Task SaveChangesAsync();
    }
}
