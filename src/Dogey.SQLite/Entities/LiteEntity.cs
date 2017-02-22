using System.ComponentModel.DataAnnotations;

namespace Dogey.SQLite
{
    public abstract class LiteEntity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
