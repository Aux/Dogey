using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.Moderation
{
    [Group("sudo"), Name("Sudo")]
    [RequireOwner]
    public class SudoModule : ModuleBase<DogeyCommandContext>
    {
        private readonly CommandHandler _manager;
        private readonly IServiceProvider _provider;

        public SudoModule(IServiceProvider provider)
        {
            _manager = provider.GetService<CommandHandler>();
            _provider = provider;
        }

        [Command]
        public async Task SudoAsync(SocketUser user, [Remainder]string command)
        {
            var context = new DogeyCommandContext(Context.Client, Context.Message, user);
            await _manager.ExecuteAsync(context, _provider, command);
        }
    }
}
