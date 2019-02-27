using Microsoft.EntityFrameworkCore;

namespace Dogey.Databases
{
    public abstract class DbController<T> where T : DbContext
    {
        public readonly T Database;

        public DbController(T db)
        {
            Database = db;
        }
    }
}
