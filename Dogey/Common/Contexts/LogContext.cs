using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class LogContext : DbContext
    {
        public DbSet<DiscordMessage> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            string datadir = Path.Combine(AppContext.BaseDirectory, "data/dogey.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Id Definitions

            builder.Entity<DiscordMessage>()
                .Property(x => x.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
