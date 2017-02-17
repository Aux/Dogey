using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Dogey.MySQL
{
    public abstract class MyEntity : IDbEntity<ulong>
    {
        [Key]
        public ulong Id { get; set; }

        public abstract Task SaveChangesAsync();
    }
}
