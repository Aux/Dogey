using Microsoft.EntityFrameworkCore;

namespace Dogey
{
    public abstract class DbController<T> where T : DbContext
    {
        internal readonly T _db;

        public DbController(T db)
        {
            _db = db;
        }
    }
}
