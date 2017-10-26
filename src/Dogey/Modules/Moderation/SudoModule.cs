using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.Moderation
{
    [RequireOwner]
    [Group("sudo"), Name("Sudo")]
    [Summary("Execute a command as another user")]
    public class SudoModule : DogeyModuleBase
    {
        private readonly CommandHandler _manager;
        private readonly IServiceProvider _provider;

        public SudoModule(IServiceProvider provider)
        {
            _manager = provider.GetService<CommandHandler>();
            _provider = provider;
        }

        [Command]
        [Summary("Execute a command as the specified user")]
        public async Task SudoAsync(SocketUser user, [Remainder]string command)
        {
            var context = new DogeyCommandContext(Context.Client, Context.Message, user);
            await _manager.ExecuteAsync(context, _provider, command);
        }
    }
}
