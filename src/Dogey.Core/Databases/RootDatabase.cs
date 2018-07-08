using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class RootDatabase : DbContext
    {
        public DbSet<UserConfig> UserConfigs { get; set; }
        public DbSet<GuildConfig> GuildConfigs { get; set; }
        public DbSet<ChannelConfig> ChannelConfigs { get; set; }

        public RootDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "root.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

        }
    }
}
