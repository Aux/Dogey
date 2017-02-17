using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public abstract class LiteEntity : IDbEntity<ulong>
    {
        [Key]
        public ulong Id { get; set; }

        public abstract Task SaveChangesAsync();
    }
}
