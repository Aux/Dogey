using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class TagManager : DbManager<TagDatabase>
    {
        public TagManager(TagDatabase db) 
            : base(db) { }
        
        public Task<Tag> GetTagAsync(ulong id)
            => _db.Tags.FirstOrDefaultAsync(x => x.Id == id);

        public Task<Tag> GetTagAsync(string name, IGuild guild)
            => _db.Tags.FirstOrDefaultAsync(x => x.Aliases.Contains(name.ToLower()) && x.GuildId == guild.Id);
        
        public async Task<IEnumerable<Tag>> GetTagsAsync(IGuild guild)
            => await _db.Tags.Where(x => x.GuildId == guild.Id).ToListAsync();

        public async Task<IEnumerable<Tag>> GetTagsAsync(IGuild guild, IUser user)
            => await _db.Tags.Where(x => x.GuildId == guild.Id && x.OwnerId == user.Id).ToListAsync();
        
        public Task<int> CountLogsAsync(ulong id)
            => _db.Logs.CountAsync(x => x.TagId == id);

        public Task<int> CountLogsAsync(ulong id, IUser user)
            => _db.Logs.CountAsync(x => x.TagId == id && x.UserId == user.Id);

        public Task<int> CountLogsAsync(ulong id, IChannel channel)
            => _db.Logs.CountAsync(x => x.TagId == id && x.ChannelId == channel.Id);

        public Task<int> CountLogsAsync(IUser user)
            => _db.Logs.CountAsync(x => x.UserId == user.Id);

        public Task<int> CountLogsAsync(IChannel channel)
            => _db.Logs.CountAsync(x => x.ChannelId == channel.Id);

        public Task<int> CountLogsAsync(IGuild guild)
            => _db.Logs.CountAsync(x => x.GuildId == guild.Id);

        public async Task<bool> IsDupeExecutionAsync(ulong id)
            => await _db.Logs.AnyAsync(x => x.TagId == id && x.Timestamp.AddSeconds(30) >= DateTime.UtcNow);
        
        public async Task<IEnumerable<Tag>> FindTagsAsync(string name, IGuild guild, int stop = 3, int tolerance = 5)
        {
            return (await GetTagsAsync(guild))
                .ToDictionary(x => x, x => x.Aliases
                .Select(y => MathHelper.GetStringDistance(y, name))
                .Sum())
                .OrderBy(x => x.Value)
                .Select(x => x.Key)
                .Take(stop);
        }
        
        public async Task AddLogAsync(Tag tag, ICommandContext context)
        {
            var log = new TagLog(tag.Id, context);

            _db.Logs.Add(log);
            await _db.SaveChangesAsync();
        }
        
        public async  Task CreateTagAsync(string name, string content, ICommandContext context)
        {
            var tag = new Tag(name, content, context);

            await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();
        }
        
        public async Task DeleteTagAsync(Tag tag)
        {
            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync();
        }
        
        public async Task ModifyTagAsync(Tag tag, string content)
        {
            tag.SetContent(content);

            _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
        }
        
        public async Task SetOwnerAsync(Tag tag, IUser user)
        {
            tag.SetOwnerId(user.Id);

            _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
        }
        
        public async Task AddAliasesAsync(Tag tag, string[] aliases)
        {
            tag.AddNames(aliases);

            _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
        }
        
        public async Task RemoveAliasesAsync(Tag tag, string[] aliases)
        {
            tag.RemoveNames(aliases);

            _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
        }

        // Create
        public async Task CreateAsync(Tag tag)
        {
            await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();
        }
    }
}
