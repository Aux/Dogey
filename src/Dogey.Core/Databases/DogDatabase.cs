using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class DogDatabase : DbContext
    {
        public DbSet<DogImage> Dogs { get; private set; }
        public DbSet<Pat> Pats { get; private set; }

        public DogDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "dog.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DogImage>()
                .HasKey(x => x.Id);
            builder.Entity<DogImage>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();
            builder.Entity<DogImage>()
                .Property(x => x.MessageId)
                .IsRequired();
            builder.Entity<DogImage>()
                .Property(x => x.Url)
                .IsRequired();

            builder.Entity<Pat>()
                .HasKey(x => x.Id);
            builder.Entity<Pat>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();
            builder.Entity<Pat>()
                .Property(x => x.Timestamp)
                .IsRequired();
            builder.Entity<Pat>()
                .Property(x => x.SenderId)
                .IsRequired();
            builder.Entity<Pat>()
                .Property(x => x.ReceiverId)
                .IsRequired();
        }
    }
}
