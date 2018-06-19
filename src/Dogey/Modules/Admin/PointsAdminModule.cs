using Discord;
using Discord.Commands;

namespace Dogey.Modules.Admin
{
    [RequireUserPermission(GuildPermission.Administrator)]
    public class PointsAdminModule : DogeyModuleBase
    {
    }
}

