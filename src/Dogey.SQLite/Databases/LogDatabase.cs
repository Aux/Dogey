using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace Dogey.SQLite
{
    public class LogDatabase : DbContext
    {
        public DbSet<LiteDiscordMessage> Messages { get; set; }
        public DbSet<LiteDiscordReaction> Reactions { get; set; }
        public DbSet<LiteDiscordCommand> Commands { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            string datadir = Path.Combine(AppContext.BaseDirectory, "data/log.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }
    }
}
