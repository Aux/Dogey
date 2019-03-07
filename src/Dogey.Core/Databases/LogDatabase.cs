using System;
using System.IO;
using System.Linq;
using Dogey.Models;
using Microsoft.EntityFrameworkCore;

namespace Dogey.Databases
{
    public class LogDatabase : DbContext
    {
        public DbSet<ModelLog> ModelLogs { get; set; }

        public LogDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "logs.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }
    }

    public class LogController : DbController<LogDatabase>
    {
        public LogController(LogDatabase db) : base(db) { }
        
        public ModelLog GetLogWhere<T>(Func<ModelLog, bool> predicate)
            => GetLogWhere(typeof(T).Name, predicate);
        public ModelLog GetLogWhere(string type, Func<ModelLog, bool> predicate)
        {
            return Database.ModelLogs.SingleOrDefault(x => x.Type == type && predicate(x));
        }

        public IQueryable<ModelLog> GetLogs<T>()
            => GetLogs(typeof(T).Name);
        public IQueryable<ModelLog> GetLogs(string type)
        {
            return Database.ModelLogs.Where(x => x.Type == type);
        }

        public IQueryable<ModelLog> GetLogsWhere<T>(Func<ModelLog, bool> predicate)
            => GetLogsWhere(typeof(T).Name, predicate);
        public IQueryable<ModelLog> GetLogsWhere(string type, Func<ModelLog, bool> predicate)
        {
            return Database.ModelLogs.Where(x => x.Type == type && predicate(x));
        }
    }
}
