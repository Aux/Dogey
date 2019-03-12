using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Databases;
using Dogey.Models;
using Dogey.Services;
using Google.Apis.YouTube.v3;
using System;
using System.Linq;
using System.Text;
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

        public RssModule(ConfigController controller, YouTubeService youtube, RssService rss)
        {
            _controller = controller;
            _youtube = youtube;
            _rss = rss;
        }

        [Command("add")]
        public async Task AddAsync(Uri url, [Remainder]SocketTextChannel channel = null)
        {
            AddFeed(url.ToString(), channel);
            await ReplyAsync("Added feed");
        }

        private void AddFeed(string url, IChannel channel)
        {
            channel = channel ?? Context.Channel;
            var feed = new RssFeed
            {
                ChannelId = channel.Id,
                Url = url
            };
            _controller.Add(feed);
            _rss.Feeds.Add(feed);
        }

        [Command("remove")]
        public async Task RemoveAsync(Uri url, [Remainder]SocketTextChannel channel)
        {
            var feed = _controller.Database.RssFeeds.SingleOrDefault(x => x.Url == url.ToString());
            if (feed == null)
            {
                await ReplyAsync("No feed was found for the specified url");
                return;
            }

            _controller.Database.Remove(feed);
            _rss.Feeds.Remove(feed);
            await ReplyAsync("Removed feed");
        }

        [Command("list")]
        public async Task ListAsync([Remainder]SocketTextChannel channel = null)
        {
            var feeds = channel == null 
                ? _rss.Feeds 
                : _rss.Feeds.Where(x => x.ChannelId == channel.Id);

            var builder = new StringBuilder();
            foreach (var feed in feeds)
                builder.AppendLine($"{((SocketTextChannel)Context.Client.GetChannel(feed.ChannelId)).Mention} {feed.Url}");

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
