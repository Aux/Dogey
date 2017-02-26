using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    internal class TagDatabase : DbContext
    {
        public DbSet<LiteTag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "tags.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }
        
        public Task<LiteTag> GetTagAsync(ulong guildId, string name)
            => Tags.FirstOrDefaultAsync(x => x.GuildId == guildId && x.Aliases.Any(y => y == name.ToLower()));
        
        public Task<List<LiteTag>> FindTagsAsync(ulong guildId, string name, int stop)
        {
            int tolerance = LiteConfiguration.Load().RelatedTagsLimit;
            var tags = Tags.Where(x => x.GuildId == guildId && x.Aliases.Any(y => LevenshteinDistance.Compute(name, y) <= tolerance));
            var selected = tags.OrderBy(x => x.Aliases.Sum(y => LevenshteinDistance.Compute(name, y))).Take(stop);
            return Task.FromResult(selected.ToList());
        }

        public async Task CreateTagAsync(SocketCommandContext context, string name, string content)
        {
            var duplicate = await Tags.AnyAsync(x => x.GuildId == context.Guild.Id && x.Aliases.Any(y => y == name));

            if (duplicate)
                throw new ArgumentException($"The tag `{name}` already exists.");

            var tag = new LiteTag()
            {
                GuildId = context.Guild.Id,
                OwnerId = context.User.Id,
                Aliases = new List<string>() { name },
                Content = content
            };

            await Tags.AddAsync(tag);
            await SaveChangesAsync();
        }

        public async Task DeleteTagAsync(SocketCommandContext context, string name)
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

        public async Task AddAliasAsync(SocketCommandContext context, string name, string[] aliases)
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

        public async Task RemoveAliasAsync(SocketCommandContext context, string name, string[] aliases)
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
