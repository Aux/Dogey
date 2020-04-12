using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class RootDatabase : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<GuildUser> GuildUsers { get; set; }
        public DbSet<Guild> Guilds { get; set; }

        public RootDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "common/data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "root.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<User>()
                .HasMany(x => x.Guilds)
                .WithOne(x => x.User);

            modelBuilder.Entity<GuildUser>()
                .HasKey(x => new { x.GuildId, x.UserId });
            modelBuilder.Entity<GuildUser>()
                .HasOne(x => x.Guild)
                .WithMany(x => x.Users);

            modelBuilder.Entity<Guild>()
                .HasKey(x => x.Id);
            modelBuilder.Entity<Guild>()
                .HasMany(x => x.Users)
                .WithOne(x => x.Guild);
        }
    }
}
