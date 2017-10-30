using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class PointsDatabase : DbContext
    {
        public DbSet<Point> Points { get; private set; }
        public DbSet<MessagePoint> MessagePoints { get; private set; }
        public DbSet<UserPoint> UserPoints { get; private set; }
        public DbSet<PointProfile> Profiles { get; private set; }
        public DbSet<Cost> Costs { get; private set; }

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
            builder.Entity<Point>()
                .HasKey(x => x.Id);
            builder.Entity<Point>()
                .Property(x => x.Timestamp)
                .IsRequired();
            builder.Entity<Point>()
                .Property(x => x.UserId)
                .IsRequired();
            builder.Entity<Point>()
                .Property(x => x.Amount)
                .IsRequired();

            builder.Entity<MessagePoint>()
                .Property(x => x.MessageId)
                .IsRequired();

            builder.Entity<UserPoint>()
                .Property(x => x.PayerId)
                .IsRequired();

            builder.Entity<PointProfile>()
                .HasKey(x => x.UserId);
            builder.Entity<PointProfile>()
                .Property(x => x.WalletSize)
                .IsRequired();
            builder.Entity<PointProfile>()
                .Property(x => x.TotalPoints)
                .IsRequired();

            builder.Entity<Cost>()
                .HasKey(x => x.Id);
            builder.Entity<Cost>()
                .Property(x => x.Amount)
                .IsRequired();
        }
    }
}
