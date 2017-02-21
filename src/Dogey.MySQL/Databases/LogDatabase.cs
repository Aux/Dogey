using Microsoft.EntityFrameworkCore;

namespace Dogey.MySQL
{
    public class LogDatabase : DbContext
    {
        public DbSet<MyDiscordMessage> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            var config = MyConfiguration.Load();
            string connection = $"server={config.Server};userid={config.User};pwd={config.Password};port={config.Port};database=root;sslmode=none;";
            builder.UseMySql(connection);
        }
    }
}
