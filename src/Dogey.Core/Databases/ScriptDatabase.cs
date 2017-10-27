using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class ScriptDatabase : DbContext
    {
        public DbSet<Script> Scripts { get; set; }

        public ScriptDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "scripts.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Script>()
                .HasKey(x => x.Id);
            builder.Entity<Script>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Entity<Script>()
                .Property(x => x.CreatedAt)
                .IsRequired();
            builder.Entity<Script>()
                .Property(x => x.Content)
                .IsRequired();
            builder.Entity<Script>()
                .Property(x => x.OwnerId)
                .IsRequired();
            builder.Entity<Script>()
                .Property(x => x.Names)
                .IsRequired();
            builder.Entity<Script>()
                .Ignore(x => x.Aliases);
        }
    }
}
