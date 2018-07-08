using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.Admin
{
    [RequireUserPermission(GuildPermission.Administrator)]
    public class PointsAdminModule : DogeyModuleBase
    {
        private readonly PointEarningService _points;

        public PointsAdminModule(PointEarningService points, RootController root)
            : base(root)
        {
            _points = points;
        }
        
        [Command("mirrorearnings")]
        public async Task MirrorPointEarningsAsync(SocketTextChannel channel)
        {
            var guild = Context.Guild;
            _points.TryAddAction(Context.Guild.Id.ToString(), async log =>
            {
                await channel.SendMessageAsync($"{guild.GetUser(log.UserId)} has earned **{log.Amount}** point(s)");
            });
            await ReplySuccessAsync();
        }

        [Command("mirrorearnings")]
        public async Task MirrorPointEarningsAsync()
        {
            if (_points.TryRemoveAction(Context.Guild.Id.ToString(), out Func<PointLog, Task> value))
            {
                await ReplySuccessAsync();
                return;
            }
            await ReplyAsync("No mirror is currently set");
        }
    }
}

