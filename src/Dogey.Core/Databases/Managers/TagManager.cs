using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class TagManager : DbManager<TagDatabase>
    {
        public TagManager(TagDatabase db) 
            : base(db) { }

        /// <summary> Get a tag by it's id </summary>
        public Task<Tag> GetTagAsync(ulong id)
            => _db.Tags.FirstOrDefaultAsync(x => x.Id == id);
        /// <summary> Get a tag in the specified guild by name </summary>
        public Task<Tag> GetTagAsync(string name, IGuild guild)
            => _db.Tags.FirstOrDefaultAsync(x => x.Aliases.Contains(name.ToLower()) && x.GuildId == guild.Id);
        
        /// <summary> Get all tags associated with the specified guild </summary>
        public async Task<IEnumerable<Tag>> GetTagsAsync(IGuild guild)
            => await _db.Tags.Where(x => x.GuildId == guild.Id).ToListAsync();
        /// <summary> Get all tags associated with the specified guild and user </summary>
        public async Task<IEnumerable<Tag>> GetTagsAsync(IGuild guild, IUser user)
            => await _db.Tags.Where(x => x.GuildId == guild.Id && x.OwnerId == user.Id).ToListAsync();

        /// <summary> Get the total usage of a tag </summary>
        public Task<int> CountLogsAsync(ulong id)
            => _db.Logs.CountAsync(x => x.TagId == id);
        /// <summary> Get the total usage of a tag for a user </summary>
        public Task<int> CountLogsAsync(ulong id, IUser user)
            => _db.Logs.CountAsync(x => x.TagId == id && x.UserId == user.Id);
        /// <summary> Get the total usage of a tag for a channel </summary>
        public Task<int> CountLogsAsync(ulong id, IChannel channel)
            => _db.Logs.CountAsync(x => x.TagId == id && x.ChannelId == channel.Id);
        /// <summary> Get the total usage of all tags for a user </summary>
        public Task<int> CountLogsAsync(IUser user)
            => _db.Logs.CountAsync(x => x.UserId == user.Id);
        /// <summary> Get the total usage of all tags for a channel </summary>
        public Task<int> CountLogsAsync(IChannel channel)
            => _db.Logs.CountAsync(x => x.ChannelId == channel.Id);
        /// <summary> Get the total usage of all tags for a guild </summary>
        public Task<int> CountLogsAsync(IGuild guild)
            => _db.Logs.CountAsync(x => x.GuildId == guild.Id);

        /// <summary> Find tags similar to the specified name </summary>
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

        /// <summary> Add a new log for the specified tag </summary>
        public async Task AddLogAsync(Tag tag, ICommandContext context)
        {
            var log = new TagLog(tag.Id, context);

            _db.Logs.Add(log);
            await _db.SaveChangesAsync();
        }

        /// <summary> Create a new tag </summary>
        public async  Task CreateTagAsync(string name, string content, ICommandContext context)
        {
            var tag = new Tag(name, content, context);

            await _db.Tags.AddAsync(tag);
            await _db.SaveChangesAsync();
        }

        /// <summary> Delete an existing tag </summary>
        public async Task DeleteTagAsync(Tag tag)
        {
            _db.Tags.Remove(tag);
            await _db.SaveChangesAsync();
        }

        /// <summary> Modify the content of an existing tag </summary>
        public async Task ModifyTagAsync(Tag tag, string content)
        {
            tag.SetContent(content);

            _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
        }

        /// <summary> Change the owner of a tag </summary>
        public async Task SetOwnerAsync(Tag tag, IUser user)
        {
            tag.SetOwnerId(user.Id);

            _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
        }

        /// <summary> Add a range of aliases to a tag </summary>
        public async Task AddAliasesAsync(Tag tag, string[] aliases)
        {
            tag.AddNames(aliases);

            _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
        }

        /// <summary> Remove a range of aliases from a tag </summary>
        public async Task RemoveAliasesAsync(Tag tag, string[] aliases)
        {
            tag.RemoveNames(aliases);

            _db.Tags.Update(tag);
            await _db.SaveChangesAsync();
        }
    }
}
