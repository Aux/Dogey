using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class ScriptManager : DbManager<ScriptDatabase>
    {
        public ScriptManager(ScriptDatabase db)
            : base(db) { }
        
        public Task<Script> GetScriptAsync(string name)
            => _db.Scripts.FirstOrDefaultAsync(x => x.Names.Contains(name.ToLower()));

        public Task<Script[]> GetScriptsAsync(ulong userId)
            => _db.Scripts.Where(x => x.OwnerId == userId).ToArrayAsync();

        public Task<Script[]> FindScriptsAsync(string name, int stop)
        {
            int tolerance = Configuration.Load().RelatedTagsLimit;
            var scripts = _db.Scripts.Where(x => x.Aliases.Any(y => MathHelper.GetStringDistance(name, y) <= tolerance));
            var selected = scripts.OrderBy(x => x.Aliases.Sum(y => MathHelper.GetStringDistance(name, y))).Take(stop);
            return selected.ToArrayAsync();
        }

        public async Task CreateScriptAsync(ulong ownerId, string name, string content)
        {
            var duplicate = await _db.Scripts.AnyAsync(x => x.Aliases.Any(y => y == name.ToLower()));

            if (duplicate)
                throw new ArgumentException($"A script named `{name}` already exists.");

            var script = new Script
            {
                OwnerId = ownerId,
                Aliases = new List<string>() { name },
                Content = content
            };

            await _db.Scripts.AddAsync(script);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteScriptAsync(string name)
        {
            var script = await _db.Scripts.FirstOrDefaultAsync(x => x.Aliases.Any(y => y == name.ToLower()));

            if (script == null)
                throw new ArgumentException($"A script named `{name}` does not exist.");

            _db.Scripts.Remove(script);
            await _db.SaveChangesAsync();
        }
    }
}
