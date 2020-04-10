using System.Threading.Tasks;

namespace Dogey
{
    class Program
    {
        static Task Main(string[] args)
            => new Startup().StartAsync();
    }
}
