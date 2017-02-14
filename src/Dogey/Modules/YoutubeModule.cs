using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("youtube"), Alias("yt")]
    public class YoutubeModule : ModuleBase<SocketCommandContext>
    {
        [Command]
        public Task BaseAsync()
            => new HelpModule().HelpAsync(Context, "youtube");

        [Command("search")]
        public Task SearchAsync([Remainder]string query)
        {
            return Task.CompletedTask;
        }

        [Command("channel")]
        public Task ChannelAsync([Remainder]string query)
        {
            return Task.CompletedTask;
        }

        [Command("playlist")]
        public Task PlaylistAsync([Remainder]string query)
        {
            return Task.CompletedTask;
        }
    }
}
