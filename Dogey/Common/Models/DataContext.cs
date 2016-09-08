using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Dogey.Models
{
    public class DataContext : DbContext
    {
        public DbSet<CustomCommand> Commands { get; set; }
        //public DbSet<CommandLog> CommandLogs { get; set; }
        public DbSet<GuildSettings> Settings { get; set; }
        //public DbSet<GuildLog> GuildLogs { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Database.EnsureCreated();
            string datadir = Path.Combine(AppContext.BaseDirectory, @"data\commands.doge");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public void CreateDatabase()
        {
            
        }
    }
}
