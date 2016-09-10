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

        public RatelimitAttribute(double time, RateMeasure mode)
        {
            Time = time;
            Mode = mode;
        }

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
                    limit = new Ratelimit()
                    {
                        UserId = msg.Author.Id,
                        Module = cmd.Module.Name,
                        Command = cmd.Name,
                    };

                    switch (Mode)
                    {
                        case RateMeasure.Seconds:
                            limit.End = DateTime.UtcNow + TimeSpan.FromSeconds(Time);
                            break;
                        case RateMeasure.Minutes:
                            limit.End = DateTime.UtcNow + TimeSpan.FromMinutes(Time);
                            break;
                        case RateMeasure.Hours:
                            limit.End = DateTime.UtcNow + TimeSpan.FromHours(Time);
                            break;
                    }

                    db.LimitedUsers.Add(limit);
                    db.SaveChanges();
                    return Task.FromResult(PreconditionResult.FromSuccess());
                }
                else
                {
                    var remaining = limit.End - DateTime.UtcNow;
                    
                    if (remaining < TimeSpan.FromSeconds(0))
                    {
                        db.LimitedUsers.Remove(limit);
                        return Task.FromResult(PreconditionResult.FromSuccess());
                    } else
                    {
                        string m = $"{remaining.Days}d {remaining.Hours}h {remaining.Minutes}m {remaining.Seconds}s";
                        return Task.FromResult(PreconditionResult.FromError($"You can use this command again in {m}."));
                    }
                }
            }
        }
    }
}
