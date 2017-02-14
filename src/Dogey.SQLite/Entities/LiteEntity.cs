using System.ComponentModel.DataAnnotations;

namespace Dogey.SQLite
{
    public class LiteEntity<T> : IEntity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
