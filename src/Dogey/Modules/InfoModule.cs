using Discord;
using Discord.Commands;
using Octokit;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("info"), Name("Info")]
    [Summary("Get information about Dogey")]
    public class InfoModule : DogeyModuleBase
    {
        private readonly GitHubClient _github;
        private Process _process;

        public InfoModule(GitHubClient github)
        {
            _github = github;
            _process = Process.GetCurrentProcess();
        }
        
        [Command]
        public async Task InfoAsync()
        {
            var app = await Context.Client.GetApplicationInfoAsync();
            var commits = await _github.Repository.Commit.GetAll("Aux", "Dogey", new ApiOptions()
            {
                PageSize = 3,
                PageCount = 1
            });

            var sbuilder = new StringBuilder()
                .AppendLine("Recent Commits:");

            foreach (var commit in commits)
                sbuilder.AppendLine($"[`{commit.Sha.Substring(0, 7)}`]({commit.HtmlUrl}) {commit.Commit.Message}");

            var builder = new EmbedBuilder()
                .WithAuthor(x => 
                {
                    x.Name = app.Owner.ToString();
                    x.IconUrl = app.Owner.GetAvatarUrl();
                    x.Url = "https://discord.gg/B4BwQ8r";
                })
                .WithDescription(sbuilder.ToString())
                .AddInlineField("Memory", GetMemoryUsage())
                .AddInlineField("Latency", GetLatency())
                .AddInlineField("Uptime", GetUptime())
                .WithFooter(x => x.Text = GetLibrary());

            await ReplyAsync(builder);
        }

        public string GetUptime()
        {
            var uptime = (DateTime.Now - _process.StartTime);
            return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        }

        public string GetLibrary()
            => $"Discord.Net ({DiscordConfig.Version})";
        public string GetMemoryUsage()
            => $"{Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)}mb";
        public string GetLatency()
            => $"{Context.Client.Latency}ms";
    }
}
