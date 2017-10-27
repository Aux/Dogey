using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class ConfigDatabase : DbContext
    {
        public DbSet<GuildConfig> GuildConfigs { get; set; }

        public ConfigDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "config.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GuildConfig>()
                .HasKey(x => x.Id);
            builder.Entity<GuildConfig>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Entity<GuildConfig>()
                .Property(x => x.GuildId)
                .IsRequired();
        }
    }
}
