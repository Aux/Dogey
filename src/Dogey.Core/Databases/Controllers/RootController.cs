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
        public async Task<bool> IsBannedAsync(IGuild guild)
            => (await _db.GuildConfigs.SingleOrDefaultAsync(x => x.Id == guild.Id)).BannedAt != null;
        public async Task<bool> IsBannedAsync(IUser user)
            => (await _db.UserConfigs.SingleOrDefaultAsync(x => x.Id == user.Id)).BannedAt != null;

        public async Task<string> GetPrefixAsync(ulong guildId)
            => (await _db.GuildConfigs.SingleOrDefaultAsync(x => x.Id == guildId))?.Prefix;
        public async Task<string> GetCurrencyNameAsync(IGuild guild)
            => (await GetConfigAsync(guild)).CurrencyName ?? "point";
        public Task<List<string>> GetDisabledModulesAsync(IGuild guild)
            => _db.ModuleConfigs.Where(x => x.GuildId == guild.Id).Select(x => x.ModuleName).ToListAsync();

        public Task<UserConfig> GetConfigAsync(IUser user)
            => _db.UserConfigs.SingleOrDefaultAsync(x => x.Id == user.Id);
        public Task<GuildConfig> GetConfigAsync(IGuild guild)
            => _db.GuildConfigs.SingleOrDefaultAsync(x => x.Id == guild.Id);
        public Task<ModuleConfig> GetConfigAsync(IGuild guild, string moduleName)
            => _db.ModuleConfigs.SingleOrDefaultAsync(x => x.GuildId == guild.Id && x.ModuleName.ToLower() == moduleName.ToLower());

        public async Task<GuildConfig> GetOrCreateConfigAsync(IGuild guild)
        {
            var config = await GetConfigAsync(guild);
            if (config != null) return config;
            return await CreateAsync(new GuildConfig { Id = guild.Id });
        }
        public async Task<UserConfig> GetOrCreateConfigAsync(IUser user)
        {
            var config = await GetConfigAsync(user);
            if (config != null) return config;
            return await CreateAsync(new UserConfig { Id = user.Id });
        }

        public async Task<UserConfig> CreateAsync(UserConfig config)
        {
            await _db.UserConfigs.AddAsync(config);
            await _db.SaveChangesAsync();
            return config;
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

        public async Task<UserConfig> ModifyAsync(UserConfig config)
        {
            config.UpdatedAt = DateTime.UtcNow;
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
