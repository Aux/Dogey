using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey
{
    public class LogDatabase : DbContext
    {
        public DbSet<DiscordMessage> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            string datadir = Path.Combine(AppContext.BaseDirectory, "data/log.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }
    }
}
