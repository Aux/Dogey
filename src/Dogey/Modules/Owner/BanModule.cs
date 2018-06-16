using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.Modules.Owner
{
    public class BanModule : DogeyModuleBase
    {
        private readonly RootController _root;

        public BanModule(RootController root)
        {
            _root = root;
        }

        [Command("banguild")]
        public async Task BanAsync([Remainder]IGuild guild)
        {
            await Task.Delay(0);
        }

        [Command("unbanguild")]
        public async Task UnbanAsync([Remainder]IGuild guild)
        {
            await Task.Delay(0);
        }
    }
}
