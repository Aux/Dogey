using Microsoft.EntityFrameworkCore;

namespace Dogey.MySQL
{
    public class RootDatabase : DbContext
    {
        public DbSet<MyGuildConfig> Guilds { get; set; }
        public DbSet<MyTag> Tags { get; set; }
        public DbSet<MyPat> Pats { get; set; }

        public DbSet<MyDiscordMessage> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var config = MyConfiguration.Load();
            string connection = $"server={config.Server};" +
                $"userid={config.User};" +
                $"pwd={config.Password};" +
                $"port={config.Port};" +
                $"database=root;";
            builder.UseMySql(connection);
        }
    }
}
