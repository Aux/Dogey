using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class PointsDatabase : DbContext
    {
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<PointLog> Logs { get; set; }
        public DbSet<Price> Prices { get; set; }

        public PointsDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "points.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

        }
    }
}
