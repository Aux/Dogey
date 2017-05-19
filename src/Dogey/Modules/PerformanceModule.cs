using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("performance"), Name("Performance")]
    [Summary("Get technical information about Dogey")]
    public class PerformanceModule : ModuleBase<SocketCommandContext>
    {
        private Process _process;

        protected override void BeforeExecute()
        {
            _process = Process.GetCurrentProcess();
        }

        [Command]
        [Summary("Get a summary of performance information about Collie")]
        public Task PerformanceAsync()
        {
            var builder = new EmbedBuilder();
            builder.ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl();
            builder.Title = "Performance Summary";

            var uptime = (DateTime.Now - _process.StartTime);

            var desc = $"**Uptime:** {GetUptime()}\n" +
                       $"**Library:** {GetLibrary()}\n" +
                       $"**OS:** {GetOperatingSystem()}\n" +
                       $"**Framework:** {GetFramework()}\n" +
                       $"**Memory Usage:** {GetMemoryUsage()}\n" +
                       $"**Latency:** {GetLatency()}\n";

            builder.Description = desc;
            return ReplyAsync("", embed: builder);
        }

        [Command("uptime")]
        [Summary("Get how long this session has been online")]
        public Task UptimeAsync()
            => ReplyAsync(GetUptime());

        [Command("library"), Alias("lib")]
        [Summary("Get information about the discord library collie is using")]
        public Task LibraryAsync()
            => ReplyAsync(GetLibrary());

        [Command("operatingsystem"), Alias("os")]
        [Summary("Get information about the operating system collie is running")]
        public Task OperatingSystemAsync()
            => ReplyAsync(GetOperatingSystem());

        [Command("framework")]
        [Summary("Get information about the framework collie is running")]
        public Task FrameworkAsync()
            => ReplyAsync(GetFramework());

        [Command("memoryusage"), Alias("memory", "mem")]
        [Summary("Get information about collie's current memory usage")]
        public Task MemoryUsageAsync()
            => ReplyAsync(GetMemoryUsage());

        [Command("latency"), Alias("lag", "ping")]
        [Summary("Get information about collie's current ping")]
        public Task LatencyAsync()
            => ReplyAsync(GetLatency());

        public string GetUptime()
        {
            var uptime = (DateTime.Now - _process.StartTime);
            return $"{uptime.Days} day {uptime.Hours} hr {uptime.Minutes} min {uptime.Seconds} sec";
        }

        public string GetLibrary()
            => $"Discord.Net ({DiscordConfig.Version})";

        public string GetOperatingSystem()
            => $"{RuntimeInformation.OSDescription} {RuntimeInformation.OSArchitecture}";

        public string GetFramework()
            => RuntimeInformation.FrameworkDescription;

        public string GetMemoryUsage()
            => $"{Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)}mb";

        public string GetLatency()
            => $"{Context.Client.Latency}ms";
    }
}