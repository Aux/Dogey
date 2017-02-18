using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public static class CommandServiceExtensions
    {
        public static Task LoadSqliteModulesAsync(this CommandService service)
        {
            return Task.CompletedTask;
        }
    }
}
