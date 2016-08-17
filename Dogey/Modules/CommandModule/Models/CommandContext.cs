using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Commands
{
    public class CommandContext : DbContext
    {
        public DbSet<CustomCommand> Commands { get; set; }
        public DbSet<CustomInfo> CommandInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string datadir = Path.Combine(AppContext.BaseDirectory, @"data\commands.doge");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }
    }
}
