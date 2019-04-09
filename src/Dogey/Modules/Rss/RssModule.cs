using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Config;
using Dogey.Databases;
using Dogey.Models;
using Dogey.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dogey.Modules.Rss
{
    [Name("RSS"), Group("rss")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public partial class RssModule : DogeyModuleBase
    {
        private readonly ConfigController _controller;
        private readonly YouTubeService _youtube;
        private readonly RssService _rss;

        private readonly TimeSpan _regexTimeout;

        public RssModule(
            ConfigController controller, 
            YouTubeService youtube,
            IConfiguration config,
            RssService rss)
        {
            _controller = controller;
            _youtube = youtube;
            _rss = rss;

            var options = new AppOptions();
            config.Bind(options);
            _regexTimeout = TimeSpan.FromSeconds(options.RegexTimeoutSeconds);
        }

        [Command("add")]
        public async Task AddAsync(Uri url, SocketTextChannel channel = null, Regex regex = null)
        {
            AddFeed(url.ToString(), channel, regex.ToString());
            await ReplyAsync("Added feed");
        }

        private bool AddFeed(string url, IChannel channel, string regex)
        {
            channel = channel ?? Context.Channel;
            var feed = new RssFeed
            {
                ChannelId = channel.Id,
                Url = url,
                Regex = string.IsNullOrWhiteSpace(regex) ? null : regex
            };
            _controller.Add(feed);
            _rss.Feeds.Add(feed);
            return true;
        }

        [Command("remove")]
        public async Task RemoveAsync(ulong id)
        {
            var feed = _controller.Database.RssFeeds.SingleOrDefault(x => x.Id == id);
            if (feed == null)
            {
                await ReplyAsync("No feed was found for the specified url");
                return;
            }

            _controller.Database.RssFeeds.Remove(feed);
            _rss.Feeds.Remove(feed);
            await ReplyAsync($"Removed feed `{feed.Id}` from {MentionUtils.MentionChannel(feed.ChannelId)}");
        }

        [Command("list")]
        public async Task ListAsync([Remainder]SocketTextChannel channel = null)
        {
            var feeds = channel == null 
                ? _rss.Feeds 
                : _rss.Feeds.Where(x => x.ChannelId == channel.Id);

            var builder = new StringBuilder();
            foreach (var feed in feeds)
            {
                var regex = feed.Regex == null ? "" : $"`{feed.Regex}` ";
                builder.AppendLine($"{feed.Id}. {((SocketTextChannel)Context.Client.GetChannel(feed.ChannelId)).Mention} {regex}{feed.Url}");
            }

            await ReplyEmbedAsync(new EmbedBuilder()
                .WithTitle($"Available Feeds ({feeds.Count()})")
                .WithDescription(builder.ToString()));
        }

        [Command("force")]
        public Task ForceAsync()
        {
            _rss.ForceUpdate();
            return Task.CompletedTask;
        }
    }
}
