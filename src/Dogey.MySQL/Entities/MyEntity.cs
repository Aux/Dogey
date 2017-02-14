using System.ComponentModel.DataAnnotations;

namespace Dogey.SQLite
{
    public class MyEntity<T> : IEntity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
