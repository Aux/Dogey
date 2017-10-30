using Discord.Commands;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Dogey
{
    public class RequireCost : PreconditionAttribute
    {
        private long _cost;

        public RequireCost(long defaultCost)
        {
            _cost = defaultCost;
        }
        
        public override async Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var points = services.GetService<PointsManager>();

            var userProfile = await points.GetProfileAsync(context.User.Id);

            var difference = userProfile.TotalPoints - _cost;
            if (difference < 0)
                return PreconditionResult.FromError($"You need **{Math.Abs(difference)}** more point(s) to use **{command.Name}**");

            await points.UpdateTotalPointsAsync(context.User.Id, _cost * -1);
            return PreconditionResult.FromSuccess();
        }
    }
}
