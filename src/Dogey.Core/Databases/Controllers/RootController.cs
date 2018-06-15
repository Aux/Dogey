using Discord;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class RootController : DbController<RootDatabase>
    {
        public RootController(RootDatabase db) : base(db) { }

        public async Task<bool> ModuleEnabledAsync(IGuild guild, string moduleName)
            => await _db.ModuleConfigs.AnyAsync(x => x.GuildId == guild.Id && (x.ModuleName.ToLower() == moduleName.ToLower() || x.ModuleName.ToLower() == moduleName.ToLower() + "module"));

        public async Task<string> GetPrefixAsync(ulong guildId)
            => (await _db.GuildConfigs.SingleOrDefaultAsync(x => x.Id == guildId))?.Prefix;
        public Task<List<string>> GetDisabledModulesAsync(IGuild guild)
            => _db.ModuleConfigs.Where(x => x.GuildId == guild.Id).Select(x => x.ModuleName).ToListAsync();

        public Task<GuildConfig> GetConfigAsync(IGuild guild)
            => _db.GuildConfigs.SingleOrDefaultAsync(x => x.Id == guild.Id);
        public Task<ModuleConfig> GetConfigAsync(IGuild guild, string moduleName)
            => _db.ModuleConfigs.SingleOrDefaultAsync(x => x.GuildId == guild.Id && x.ModuleName.ToLower() == moduleName.ToLower());

        public async Task<GuildConfig> GetOrCreateConfigAsync(IGuild guild)
        {
            bool exists = await _db.GuildConfigs.AnyAsync(x => x.Id == guild.Id);
            if (exists)
                return await GetConfigAsync(guild);
            return await CreateAsync(new GuildConfig { Id = guild.Id });
        }

        public async Task<GuildConfig> CreateAsync(GuildConfig config)
        {
            await _db.GuildConfigs.AddAsync(config);
            await _db.SaveChangesAsync();
            return config;
        }
        public async Task<ModuleConfig> CreateAsync(ModuleConfig config)
        {
            await _db.ModuleConfigs.AddAsync(config);
            await _db.SaveChangesAsync();
            return config;
        }

        public async Task<GuildConfig> ModifyAsync(GuildConfig config)
        {
            config.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return config;
        }
        
        public async Task DeleteAsync(ModuleConfig config)
        {
            _db.ModuleConfigs.Remove(config);
            await _db.SaveChangesAsync();
        }
    }
}
