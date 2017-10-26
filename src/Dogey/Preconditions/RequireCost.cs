using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class RequireCost : PreconditionAttribute
    {
        private int _cost;

        public RequireCost(int defaultCost)
        {
            _cost = defaultCost;
        }
        
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            throw new NotImplementedException();
        }
    }
}
