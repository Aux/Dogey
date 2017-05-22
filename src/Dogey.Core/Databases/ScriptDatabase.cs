using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class ScriptDatabase : DbContext
    {
        public DbSet<Script> Scripts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "scripts.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public Task<Script> GetScriptAsync(string name)
            => Scripts.FirstOrDefaultAsync(x => x.Names.Contains(name.ToLower()));

        public Task<Script[]> GetScriptsAsync(ulong userId)
            => Scripts.Where(x => x.OwnerId == userId).ToArrayAsync();

        public Task<Script[]> FindScriptsAsync(string name, int stop)
        {
            int tolerance = Configuration.Load().RelatedTagsLimit;
            var scripts = Scripts.Where(x => x.Aliases.Any(y => Levenshtein.GetDistance(name, y) <= tolerance));
            var selected = scripts.OrderBy(x => x.Aliases.Sum(y => Levenshtein.GetDistance(name, y))).Take(stop);
            return selected.ToArrayAsync();
        }

        public async Task CreateScriptAsync(ulong ownerId, string name, string content)
        {
            var duplicate = await Scripts.AnyAsync(x => x.Aliases.Any(y => y == name.ToLower()));

            if (duplicate)
                throw new ArgumentException($"A script named `{name}` already exists.");

            var script = new Script
            {
                OwnerId = ownerId,
                Aliases = new List<string>() { name },
                Content = content
            };

            await Scripts.AddAsync(script);
            await SaveChangesAsync();
        }

        public async Task DeleteScriptAsync(string name)
        {
            var script = await Scripts.FirstOrDefaultAsync(x => x.Aliases.Any(y => y == name.ToLower()));

            if (script == null)
                throw new ArgumentException($"A script named `{name}` does not exist.");

            Scripts.Remove(script);
            await SaveChangesAsync();
        }
    }
}
