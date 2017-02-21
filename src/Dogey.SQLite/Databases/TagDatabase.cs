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
    public class TagDatabase : DbContext
    {
        public DbSet<LiteTag> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");

            string datadir = Path.Combine(AppContext.BaseDirectory, "data/tags.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public Task<LiteTag> GetTagAsync(SocketCommandContext context, string name)
            => Tags.FirstOrDefaultAsync(x => x.GuildId == context.Guild.Id && x.Name == name.ToLower());
        
        public Task<List<LiteTag>> FindTagsAsync(SocketCommandContext context, string name, int stop)
        {
            int tolerance = 5;
            var tags = Tags.Where(x => LevenshteinDistance.Compute(name, x.Name) <= tolerance).Take(stop);
            return Task.FromResult(tags.ToList());
        }

        public async Task CreateTagAsync(SocketCommandContext context, string name, string content)
        {
            var duplicate = await Tags.AnyAsync(x => x.GuildId == context.Guild.Id && x.Name == name);

            if (duplicate)
                throw new ArgumentException($"The tag `{name}` already exists.");

            var tag = new LiteTag()
            {
                GuildId = context.Guild.Id,
                OwnerId = context.User.Id,
                Name = name,
                Content = content
            };

            await Tags.AddAsync(tag);
            await SaveChangesAsync();
        }

        public async Task DeleteTagAsync(SocketCommandContext context, string name)
        {
            var tag = await Tags.FirstOrDefaultAsync(x => x.GuildId == context.Guild.Id && x.Name == name);

            if (tag == null)
                throw new ArgumentException($"The tag `{name}` does not exist.");

            var user = context.User as SocketGuildUser;
            if (tag.OwnerId != user.Id && !user.GuildPermissions.ManageMessages)
                throw new UnauthorizedAccessException($"You are not the owner of the tag `{name}`.");

            Tags.Remove(tag);
            await SaveChangesAsync();
        }
    }
}
