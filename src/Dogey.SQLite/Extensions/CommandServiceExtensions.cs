using Discord.Commands;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public static class CommandServiceExtensions
    {
        public static async Task LoadSqliteModulesAsync(this CommandService service)
        {
            await service.AddModulesAsync(Assembly.GetEntryAssembly());
        }
    }
}
