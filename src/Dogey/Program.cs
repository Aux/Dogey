using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Dogey
{
    public class Program
    {
        public static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        private Startup _startup = new Startup();

        public async Task StartAsync()
        {
            PrettyConsole.NewLine($"Dogey v{AppHelper.Version}");
            PrettyConsole.NewLine();

            Configuration.EnsureExists();

            var services = await _startup.ConfigureServices();

            var manager = services.GetService<CommandManager>();
            await manager.StartAsync();

            await Task.Delay(-1);
        }
    }
}