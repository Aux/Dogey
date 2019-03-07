using Dogey.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;

namespace Dogey.Databases
{
    public class ConfigDatabase : DbContext
    {
        public DbSet<GuildConfig> Guilds { get; set; }
        //public DbSet<ChannelConfig> Channels { get; set; }

        public ConfigDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "config.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }
    }

    public class ConfigController : DbController<ConfigDatabase>
    {
        public ConfigController(ConfigDatabase db) : base(db) { }

        public bool TryGetPrefix(ulong id, out string prefix)
        {
            prefix = null;
            var guildPrefix = GetGuildPrefix(id);
            if (guildPrefix != null)
            {
                prefix = guildPrefix;
                return true;
            }
            return false;
        }

        public string GetGuildPrefix(ulong id)
            => Database.Guilds.SingleOrDefault(x => x.Id == id)?.Prefix;

        public void Add(object config)
        {
            Database.Add(config);
        }
    }
}
