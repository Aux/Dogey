using Discord.Commands;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public static class CommandServiceExtensions
    {
        public static Task LoadSqliteModulesAsync(this CommandService service)
        {
            using (var db = new TagDatabase())
                db.Database.EnsureCreated();

            return service.AddModulesAsync(typeof(LiteEntity<>).GetTypeInfo().Assembly);
        }
    }
}
