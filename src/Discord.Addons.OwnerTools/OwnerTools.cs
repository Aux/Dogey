using Discord.Commands;
using System.Threading.Tasks;

namespace Discord.Addons.OwnerTools
{
    public static class OwnerTools
    {
        public static async Task LoadAsync(CommandService service)
        {
            var avatar = new AvatarModule(service);
            await avatar.LoadAsync();

        }
    }
}
