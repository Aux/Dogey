using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Dogey.Commands
{
    public class RequireNodeAttribute : PreconditionAttribute
    {
        public Node Node { get; }

        public RequireNodeAttribute(string value)
        {
            Node = new Node(value);
        }

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
