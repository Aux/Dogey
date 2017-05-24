using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dogey
{
    public class PatsDatabase : DbContext
    {
        public DbSet<Pat> Pats { get; set; }

        public PatsDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "pats.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public Task<int> CountReceivedPatsAsync(ulong userId)
            => Pats.CountAsync(x => x.ReceiverId == userId);

        public Task<int> CountSentPatsAsync(ulong userId)
            => Pats.CountAsync(x => x.SenderId == userId);

        public Task<int> CountRecentPatsAsync(ulong userId)
            => Pats.CountAsync(x => x.SenderId == userId && x.Timestamp.AddMinutes(10) > DateTime.Now);

        public Task CreatePatAsync(SocketUser sender, SocketUser receiver)
        {
            var pat = new Pat()
            {
                SenderId = sender.Id,
                SenderName = sender.Username,
                ReceiverId = receiver.Id
            };

            Pats.Add(pat);
            return SaveChangesAsync();
        }
    }
}
