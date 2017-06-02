using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class TagDatabase : DbContext
    {
        public DbSet<Tag> Tags { get; private set; }
        public DbSet<TagLog> Logs { get; private set; }

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
            builder.Entity<Tag>()
                .Property(x => x.Names)
                .IsRequired();
            builder.Entity<Tag>()
                .Ignore(x => x.Aliases);

            builder.Entity<TagLog>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
            builder.Entity<TagLog>()
                .Property(x => x.GuildId)
                .IsRequired();
            builder.Entity<TagLog>()
                .Property(x => x.ChannelId)
                .IsRequired();
            builder.Entity<TagLog>()
                .Property(x => x.UserId)
                .IsRequired();
            builder.Entity<TagLog>()
                .Property(x => x.TagId)
                .IsRequired();
        }
    }
}
