using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [RequireOwner]
    [RequireEnabled]
    public class SudoModule : DogeyModuleBase
    {
        private readonly CommandHandlingService _handler;
        private readonly IServiceProvider _provider;

        public SudoModule(CommandHandlingService handler, IServiceProvider provider)
        {
            _handler = handler;
            _provider = provider;
        }

        [Command("sudo")]
        public async Task SudoAsync(SocketUser user, [Remainder]string command)
        {
            var context = new DogeyCommandContext(Context.Client, Context.Message, user);
            await _handler.ExecuteAsync(context, _provider, command);
        }
    }
}
