using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

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

        public Task<LiteDiscordMessage> GetMessageAsync(ulong msgId)
            => Messages.FirstOrDefaultAsync(x => x.MessageId == msgId);

        public Task<LiteDiscordCommand> GetCommandAsync(ulong msgId)
            => Commands.FirstOrDefaultAsync(x => x.MessageId == msgId);
    }
}
