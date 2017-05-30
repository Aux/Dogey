using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class TagDatabase : DbContext
    {
        public DbSet<Tag> Tags { get; set; }

        public TagDatabase()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "tags.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }
        
        public Task<Tag> GetTagAsync(ulong guildId, string name)
            => Tags.FirstOrDefaultAsync(x => x.GuildId == guildId && x.Aliases.Any(y => y == name.ToLower()));

        public Task<Tag[]> GetTagsAsync(ulong guildId, ulong? userId = null)
            => Tags.Where(x => x.GuildId == guildId && (userId == null ? true : x.OwnerId == userId)).ToArrayAsync();

        public Task<Tag[]> FindTagsAsync(ulong guildId, string name, int stop)
        {
            int tolerance = Configuration.Load().RelatedTagsLimit;
            var tags = Tags.Where(x => x.GuildId == guildId && x.Aliases.Any(y => MathHelper.GetStringDistance(name, y) <= tolerance));
            var selected = tags.OrderBy(x => x.Aliases.Sum(y => MathHelper.GetStringDistance(name, y))).Take(stop);
            return selected.ToArrayAsync();
        }

        public async Task CreateTagAsync(DogeyCommandContext context, string name, string content)
        {
            var duplicate = await Tags.AnyAsync(x => x.GuildId == context.Guild.Id && x.Aliases.Any(y => y == name));

            if (duplicate)
                throw new ArgumentException($"The tag `{name}` already exists.");

            var tag = new Tag()
            {
                GuildId = context.Guild.Id,
                OwnerId = context.User.Id,
                Aliases = new List<string>() { name },
                Content = content
            };

            await Tags.AddAsync(tag);
            await SaveChangesAsync();
        }

        public async Task DeleteTagAsync(DogeyCommandContext context, string name)
        {
            var tag = await Tags.FirstOrDefaultAsync(x => x.GuildId == context.Guild.Id && x.Aliases.Any(y => y == name));

            if (tag == null)
                throw new ArgumentException($"The tag `{name}` does not exist.");

            var user = context.User as SocketGuildUser;
            if (tag.OwnerId != user.Id && !user.GuildPermissions.ManageMessages)
                throw new UnauthorizedAccessException($"You are not the owner of the tag `{name}`.");

            Tags.Remove(tag);
            await SaveChangesAsync();
        }

        public async Task AddAliasAsync(DogeyCommandContext context, string name, string[] aliases)
        {
            var tag = await Tags.FirstOrDefaultAsync(x => x.GuildId == context.Guild.Id && x.Aliases.Any(y => y == name));

            if (tag == null)
                throw new ArgumentException($"The tag `{name}` does not exist.");

            var user = context.User as SocketGuildUser;
            if (tag.OwnerId != user.Id && !user.GuildPermissions.ManageMessages)
                throw new UnauthorizedAccessException($"You are not the owner of the tag `{name}`.");

            tag.Aliases.AddRange(aliases);
            await SaveChangesAsync();
        }

        public async Task RemoveAliasAsync(DogeyCommandContext context, string name, string[] aliases)
        {
            var tag = await Tags.FirstOrDefaultAsync(x => x.GuildId == context.Guild.Id && x.Aliases.Any(y => y == name));

            if (tag == null)
                throw new ArgumentException($"The tag `{name}` does not exist.");

            var user = context.User as SocketGuildUser;
            if (tag.OwnerId != user.Id && !user.GuildPermissions.ManageMessages)
                throw new UnauthorizedAccessException($"You are not the owner of the tag `{name}`.");

            foreach (var alias in aliases)
                tag.Aliases.Remove(alias);

            await SaveChangesAsync();
        }
    }
}
