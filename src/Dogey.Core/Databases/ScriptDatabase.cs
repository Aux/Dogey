using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class ScriptDatabase : DbContext
    {
        public DbSet<Script> Scripts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "scripts.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }
    }
}
