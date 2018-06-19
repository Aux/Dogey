using Discord;
using Discord.Commands;
using Octokit;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [RequireEnabled]
    public class AboutModule : DogeyModuleBase
    {
        public string Library => $"Discord.Net ({DiscordConfig.Version})";
        public string MemoryUsage => $"{Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)}mb";

        private readonly GitHubClient _github;
        private readonly Process _process;

        public AboutModule(GitHubClient github)
        {
            _github = github;
            _process = Process.GetCurrentProcess();
        }

        public string GetUptime()
        {
            var uptime = (DateTime.Now - _process.StartTime);
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }

        [Command("about")]
        public async Task AboutAsync()
        {
            var app = await Context.Client.GetApplicationInfoAsync();
            var commits = await _github.Repository.Commit.GetAll("Aux", "Dogey", new ApiOptions()
            {
                PageSize = 3,
                PageCount = 1
            });

            var builder = new StringBuilder()
                .AppendLine("**Recent Commits:**");

            foreach (var commit in commits)
                builder.AppendLine($"[`{commit.Sha.Substring(0, 7)}`]({commit.HtmlUrl}) {commit.Commit.Message}");
            
            var embed = new EmbedBuilder()
                .WithAuthor(app.Owner)
                .WithDescription(builder.ToString())
                .AddInlineField("Memory", MemoryUsage)
                .AddInlineField("Latency", Context.Client.Latency + "ms")
                .AddInlineField("Uptime", GetUptime())
                .WithFooter(x => x.Text = Library);
            await ReplyEmbedAsync(embed);
        }
    }
}
