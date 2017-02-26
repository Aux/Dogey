using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("info")]
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private CommandService _service;
        private Process _process;

        public InfoModule(CommandService service)
        {
            _service = service;
        }

        protected override void BeforeExecute()
        {
            _process = Process.GetCurrentProcess();

        }

        [Command]
        public Task BaseAsync()
            => new HelpModule(_service).HelpAsync(Context, "info");

        [Command("summary")]
        public Task SummaryAsync()
        {
            return Task.CompletedTask;
        }

        [Command("performance")]
        public Task PerformanceAsync()
        {
            var builder = new EmbedBuilder();
            builder.ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl();
            builder.Title = "Performance Information for Dogey";

            var uptime = (DateTime.Now - _process.StartTime);

            var desc = $"**Uptime:** {uptime.Days} day {uptime.Hours} hr {uptime.Minutes} min {uptime.Seconds} sec\n" +
                       $"**Library:** Discord.Net ({DiscordConfig.Version})\n" +
                       $"**OS:** {RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}\n" +
                       $"**Framework:** {RuntimeInformation.FrameworkDescription}\n" +
                       $"**Memory Usage:** {Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)}mb\n" +
                       $"**Latency:** {Context.Client.Latency}ms\n";

            builder.Description = desc;
            return ReplyAsync("", embed: builder);
        }

        [Command("activity")]
        public Task ActivityAsync()
        {
            return Task.CompletedTask;
        }
    }
}
