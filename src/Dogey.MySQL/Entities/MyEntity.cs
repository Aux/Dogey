using System.ComponentModel.DataAnnotations;

namespace Dogey.MySQL
{
    public abstract class MyEntity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
