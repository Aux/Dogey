using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class TagManager : DbManager<TagDatabase>
    {
        public TagManager(TagDatabase db) 
            : base(db) { }

        public Task<Tag> GetTagAsync(ulong guildId, string name)
            => _db.Tags.SingleOrDefaultAsync(x => x.GuildId == guildId && x.Name.ToLower() == name.ToLower());
        public Task<Tag[]> GetTagsAsync(ulong guildId)
            => _db.Tags.Where(x => x.GuildId == guildId).ToArrayAsync();
        public Task<Tag[]> GetPublicTagsAsync(ulong guildId)
            => _db.Tags.Where(x => x.GuildId == guildId && x.IsPublic == true).ToArrayAsync();
        public async Task CreateAsync(Tag tag)
        {
            await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();
        }

        public Task<TagAlias> GetAliasAsync(ulong guildId, string name)
            => _db.Aliases.Include(x => x.Tag).SingleOrDefaultAsync(x => x.GuildId == guildId && x.Name.ToLower() == name.ToLower());
        public Task<TagAlias[]> GetAliasesAsync(ulong tagId)
            => _db.Aliases.Where(x => x.TagId == tagId).ToArrayAsync();
        public async Task CreateAsync(TagAlias alias)
        {
            await _db.Aliases.AddAsync(alias);
            await _db.SaveChangesAsync();
        }

        public Task<bool> DefaultExistsAsync(ulong userId)
            => _db.Defaults.AnyAsync(x => x.UserId == userId);
        public Task<TagDefaults> GetDefaultsAsync(ulong userId)
            => _db.Defaults.SingleOrDefaultAsync(x => x.UserId == userId);
        public async Task<TagDefaults> GetOrCreateDefaultsAsync(ulong userId)
        {
            if (!await DefaultExistsAsync(userId))
            {
                await CreateAsync(new TagDefaults
                {
                    UserId = userId
                });
            }
            return await GetDefaultsAsync(userId);
        }
        public async Task CreateAsync(TagDefaults defaults)
        {
            await _db.Defaults.AddAsync(defaults);
            await _db.SaveChangesAsync();
        }
        public async Task UpdateAsync(TagDefaults defaults)
        {
            _db.Defaults.Update(defaults);
            await _db.SaveChangesAsync();
        }

        public async Task<Tag> FindTagAsync(ulong guildId, string name, int stop = 3, int tolerance = 5)
        {
            var tag = await GetTagAsync(guildId, name);
            if (tag != null)
                return tag;

            var alias = await GetAliasAsync(guildId, name);
            if (alias != null)
                return alias.Tag;

            return null;
        }

        //public async Task<Tag> FindTagsAsync(ulong guildId, string name, int stop = 3, int tolerance = 5)
        //{
        //    var tagResults = (await GetTagsAsync(guildId))
        //        .ToDictionary(x => x, x => x.Aliases
        //        .Select(y => MathHelper.GetStringDistance(y.Name, name))
        //        .Sum())
        //        .OrderBy(x => x.Value)
        //        .Select(x => x.Key)
        //        .Take(stop);
        //}
    }
}
