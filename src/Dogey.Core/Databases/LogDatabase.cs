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

    /// <summary> Stored queries for the LogDatabase class </summary>
    public class LogController : DbController<LogDatabase>
    {
        public LogController(LogDatabase db) : base(db) { }
        
        /// <summary> Get a single log for a type that matches the specified conditions </summary>
        public ModelLog GetLogWhere<T>(Func<ModelLog, bool> predicate)
            => GetLogWhere(typeof(T).Name, predicate);
        /// <summary> Get a single log for a type that matches the specified conditions </summary>
        public ModelLog GetLogWhere(string type, Func<ModelLog, bool> predicate)
        {
            return Database.ModelLogs.SingleOrDefault(x => x.Type == type && predicate(x));
        }

        /// <summary> Get all logs for a type </summary>
        public IQueryable<ModelLog> GetLogs<T>()
            => GetLogs(typeof(T).Name);
        /// <summary> Get all logs for a type </summary>
        public IQueryable<ModelLog> GetLogs(string type)
        {
            return Database.ModelLogs.Where(x => x.Type == type);
        }

        /// <summary> Get all logs for a type that match the specified conditions </summary>
        public IQueryable<ModelLog> GetLogsWhere<T>(Func<ModelLog, bool> predicate)
            => GetLogsWhere(typeof(T).Name, predicate);
        /// <summary> Get all logs for a type that match the specified conditions </summary>
        public IQueryable<ModelLog> GetLogsWhere(string type, Func<ModelLog, bool> predicate)
        {
            return Database.ModelLogs.Where(x => x.Type == type && predicate(x));
        }
    }
}
