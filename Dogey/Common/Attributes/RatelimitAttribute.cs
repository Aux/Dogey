using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Dogey.Enums;
using Dogey.Models;

namespace Dogey.Attributes
{
    /// <summary>
    /// Set the minimum permission required to use a module or command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RatelimitAttribute : PreconditionAttribute
    {
        public double Time;
        public RateMeasure Mode;
        
        public override Task<PreconditionResult> CheckPermissions(IUserMessage msg, Command cmd, object mod)
        {
            using (var db = new DataContext())
            {
                var limit = db.LimitedUsers.Where(x =>
                            x.UserId == msg.Author.Id &&
                            x.Module == cmd.Module.Name &&
                            x.Command == cmd.Name).FirstOrDefault();

                if (limit == null)
                {
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
                else
                {
                    var remaining = DateTime.Now - limit.End;

                    string m = $"{remaining.Days}d {remaining.Hours}h {remaining.Minutes}m {remaining.Seconds}s";
                    return Task.FromResult(PreconditionResult.FromError($"You are ratelimited for another {m}."));
                }
            }
        }
    }
}
