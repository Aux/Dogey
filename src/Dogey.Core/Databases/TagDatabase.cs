using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class TagDatabase : DbContext
    {
        public DbSet<Tag> Tags { get; private set; }
        public DbSet<TagAlias> Aliases { get; private set; }
        public DbSet<TagDefaults> Defaults { get; private set; }

        public TagDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "tags.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Tag>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Entity<Tag>()
                .Property(x => x.CreatedAt)
                .IsRequired();
            builder.Entity<Tag>()
                .Property(x => x.UpdatedAt)
                .IsRequired();
            builder.Entity<Tag>()
                .Property(x => x.GuildId)
                .IsRequired();
            builder.Entity<Tag>()
                .Property(x => x.OwnerId)
                .IsRequired();
            builder.Entity<Tag>()
                .Property(x => x.Content)
                .IsRequired();

            builder.Entity<TagAlias>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Entity<TagAlias>()
                .Property(x => x.TagId)
                .IsRequired();
            builder.Entity<TagAlias>()
                .Property(x => x.OwnerId)
                .IsRequired();
            builder.Entity<TagAlias>()
                .Property(x => x.Name)
                .IsRequired();
            builder.Entity<TagAlias>()
                .HasOne(x => x.Tag)
                .WithMany(x => x.Aliases);

            builder.Entity<TagDefaults>()
                .HasKey(x => x.UserId);
            builder.Entity<TagDefaults>()
                .Property(x => x.UserId)
                .IsRequired();
        }
    }
}
