using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public abstract class LiteEntity<T> : IDbEntity<T>
    {
        [Key]
        public T Id { get; set; }

        public abstract Task SaveChangesAsync();
    }
}
