using System.Threading.Tasks;

namespace Dogey
{
    public interface IDbEntity<T> : IEntity<T>
    {
        Task SaveChangesAsync();
    }
}
