using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public class ConfigDatabase : DbContext
    {
        public DbSet<LiteGuildConfig> GuildConfigs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            string datadir = Path.Combine(AppContext.BaseDirectory, "data/config.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public async Task<LiteGuildConfig> GetConfigAsync(ulong guildId)
        {
            var config = await GuildConfigs.FirstOrDefaultAsync(x => x.Id == guildId);

            if (config != null)
                return config;

            config = new LiteGuildConfig(guildId);
            await GuildConfigs.AddAsync(config);
            await SaveChangesAsync();
            return config;
        }

        public async Task SetPrefixAsync(ulong guildId, string prefix)
        {
            var config = await GetConfigAsync(guildId);
            await SetPrefixAsync(config, prefix);
        }

        public async Task SetPrefixAsync(LiteGuildConfig config, string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                config.Prefix = null;
            else
                config.Prefix = prefix;

            GuildConfigs.Update(config);
            await SaveChangesAsync();
        }
    }
}
