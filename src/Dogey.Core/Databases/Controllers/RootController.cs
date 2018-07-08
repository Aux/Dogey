using Discord;
using Discord.Addons.EmojiTools;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class RootController : DbController<RootDatabase>
    {
        public RootController(RootDatabase db) : base(db) { }
        
        public async Task<bool> IsBannedAsync(IGuild guild)
            => (await _db.GuildConfigs.SingleOrDefaultAsync(x => x.Id == guild.Id)).BannedAt != null;
        public async Task<bool> IsBannedAsync(IUser user)
            => (await _db.UserConfigs.SingleOrDefaultAsync(x => x.Id == user.Id)).BannedAt != null;

        public async Task<string> GetPrefixAsync(IGuild guild)
            => guild == null ? null : (await _db.GuildConfigs.SingleOrDefaultAsync(x => x.Id == guild.Id))?.Prefix;
        public async Task<string> GetCurrencyNameAsync(IGuild guild)
            => (await GetConfigAsync(guild)).CurrencyName ?? DogeyConstants.DefaultCurrencyName;
        public async Task<IEmote> GetSuccessEmojiAsync(IGuild guild, IGuildChannel channel = null)
        {
            string emojiText = DogeyConstants.DefaultSuccessEmoji;
            var chconfig = await GetConfigAsync(channel);
            if (!string.IsNullOrWhiteSpace(chconfig?.SuccessEmoji))
                emojiText = chconfig.SuccessEmoji;
            var gconfig = await GetConfigAsync(guild);
            if (!string.IsNullOrWhiteSpace(gconfig?.SuccessEmoji))
                emojiText = gconfig.SuccessEmoji;
            
            try
            {
                return EmojiExtensions.FromText(emojiText);
            }
            catch (ArgumentException)
            {
                return Emote.Parse(DogeyConstants.DefaultSuccessEmoji);
            }
        }

        public Task<UserConfig> GetConfigAsync(IUser user)
            => _db.UserConfigs.SingleOrDefaultAsync(x => x.Id == user.Id);
        public Task<GuildConfig> GetConfigAsync(IGuild guild)
            => _db.GuildConfigs.SingleOrDefaultAsync(x => x.Id == guild.Id);
        public Task<ChannelConfig> GetConfigAsync(IGuildChannel channel)
            => channel == null ? Task.FromResult(default(ChannelConfig)) : _db.ChannelConfigs.SingleOrDefaultAsync(x => x.Id == channel.Id);

        public async Task<UserConfig> GetOrCreateConfigAsync(IUser user)
        {
            var config = await GetConfigAsync(user);
            if (config != null) return config;
            return await CreateAsync(new UserConfig { Id = user.Id });
        }
        public async Task<GuildConfig> GetOrCreateConfigAsync(IGuild guild)
        {
            var config = await GetConfigAsync(guild);
            if (config != null) return config;
            return await CreateAsync(new GuildConfig { Id = guild.Id });
        }
        public async Task<ChannelConfig> GetOrCreateConfigAsync(IGuildChannel channel)
        {
            var config = await GetConfigAsync(channel);
            if (config != null) return config;
            return await CreateAsync(new ChannelConfig { Id = channel.Id, GuildId = channel.GuildId });
        }

        public async Task<UserConfig> CreateAsync(UserConfig config)
        {
            await _db.UserConfigs.AddAsync(config);
            await SaveAsync();
            return config;
        }
        public async Task<GuildConfig> CreateAsync(GuildConfig config)
        {
            await _db.GuildConfigs.AddAsync(config);
            await SaveAsync();
            return config;
        }
        public async Task<ChannelConfig> CreateAsync(ChannelConfig config)
        {
            await _db.ChannelConfigs.AddAsync(config);
            await SaveAsync();
            return config;
        }

        public async Task<UserConfig> ModifyAsync(UserConfig config)
        {
            config.UpdatedAt = DateTime.UtcNow;
            await SaveAsync();
            return config;
        }
        public async Task<GuildConfig> ModifyAsync(GuildConfig config)
        {
            config.UpdatedAt = DateTime.UtcNow;
            await SaveAsync();
            return config;
        }
        public async Task<ChannelConfig> ModifyAsync(ChannelConfig config)
        {
            config.UpdatedAt = DateTime.UtcNow;
            await SaveAsync();
            return config;
        }

        public Task SaveAsync()
            => _db.SaveChangesAsync();
    }
}
