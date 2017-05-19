using System.ComponentModel.DataAnnotations;

namespace Dogey
{
    public abstract class Entity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
