using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Dogey.Modules.Owner
{
    [RequireOwner]
    public class TestModule : DogeyModuleBase
    {
        private readonly ResponsiveService _responsive;

        public TestModule(ResponsiveService responsive, RootController root)
            : base(root)
        {
            _responsive = responsive;
        }
    }
}
