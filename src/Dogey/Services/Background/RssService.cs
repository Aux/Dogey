using Discord.WebSocket;
using Dogey.Config;
using Dogey.Databases;
using Dogey.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dogey.Services
{
    public class RssArticle
    {
        public ulong FeedId { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public class RssService : BackgroundService
    {
        public List<RssFeed> Feeds;

        private CancellationTokenSource _cts, _delay;
        private Task _runTask;
        private int _iteration = 0;

        private readonly DiscordSocketClient _discord;
        private readonly ConfigController _controller;
        private readonly ILogger<RssService> _logger;
        private readonly HttpClient _http;

        private readonly TimeSpan _regexTimeout;

        public RssService(
            DiscordSocketClient discord,
            ConfigController controller,
            ILogger<RssService> logger,
            IConfiguration config,
            HttpClient http)
        {
            Feeds = new List<RssFeed>();

            var options = new AppOptions();
            config.Bind(options);
            _regexTimeout = TimeSpan.FromSeconds(options.RegexTimeoutSeconds);

            _discord = discord;
            _controller = controller;
            _logger = logger;
            _http = http;
        }

        public override void Start()
        {
            Feeds.AddRange(_controller.Database.RssFeeds);
            _cts = new CancellationTokenSource();
            _delay = new CancellationTokenSource();
            _runTask = RunAsync(_cts.Token);
            _logger.LogInformation("Started");
        }

        public override void Stop()
        {
            _cts.Cancel();
            _runTask = null;
            _logger.LogInformation("Stopped");
        }

        public void ForceUpdate()
        {
            _delay.Cancel();
            _delay = new CancellationTokenSource();
        }

        private IEnumerable<RssArticle> ParseGeneric(XElement node, RssFeed feed)
        {
            var articles = node
                .Element("channel")
                .Elements("item")
                .Select(x => new RssArticle
                {
                    FeedId = feed.Id,
                    Title = x.Element("title")?.Value,
                    Link = x.Element("link")?.Value,
                    PublishedAt = DateTime.Parse(x.Element("pubDate").Value)
                });
            return articles;
        }

        private IEnumerable<RssArticle> ParseAtom(XElement node, RssFeed feed)
        {
            var articles = node.Elements("{http://www.w3.org/2005/Atom}entry")
                .Select(x => new RssArticle
                {
                    FeedId = feed.Id,
                    Title = x.Element("{http://www.w3.org/2005/Atom}title")?.Value,
                    Link = x.Element("{http://www.w3.org/2005/Atom}link")?.Attribute("href")?.Value,
                    PublishedAt = DateTime.Parse(x.Element("{http://www.w3.org/2005/Atom}published").Value)
                });
            return articles;
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    _iteration++;
                    _logger.LogInformation($"Beginning poll #{_iteration}");

                    var queue = new Queue<RssFeed>(Feeds);
                    while (queue.Count > 0)
                    {
                        var feed = queue.Dequeue();
                        var response = await _http.SendAsync(new HttpRequestMessage(HttpMethod.Get, feed.Url));
                        if (!response.IsSuccessStatusCode)
                        {
                            _logger.LogError($"Unable to read `{feed.Url}`: {response.ReasonPhrase}");
                            continue;
                        }

                        var content = await response.Content.ReadAsStreamAsync();
                        var document = XDocument.Load(content);
                        var rssNode = document.Element("rss");
                        var atomNode = document.Element("{http://www.w3.org/2005/Atom}feed");

                        var articles = new List<RssArticle>();
                        if (rssNode != null)
                            articles.AddRange(ParseGeneric(rssNode, feed));
                        else
                        if (atomNode != null)
                            articles.AddRange(ParseAtom(atomNode, feed));
                        else
                            throw new InvalidOperationException("Unknown feed type found");
                        
                        articles = articles
                            .Where(x => x.PublishedAt > feed.UpdatedAt)
                            .Where(x => feed.Regex != null ? Regex.Match(x.Title, feed.Regex, RegexOptions.None, _regexTimeout).Success : true)
                            .OrderBy(x => x.PublishedAt)
                            .ToList();

                        if (articles.Count == 0)
                            continue;

                        foreach (var article in articles)
                        {
                            var channel = _discord.GetChannel(feed.ChannelId) as SocketTextChannel;
                            await channel.SendMessageAsync($"New post found: {article.Link}");
                        }
                        feed.UpdatedAt = DateTime.Now;
                    }

                    try
                    {
                        await Task.Delay(TimeSpan.FromMinutes(5), _delay.Token);
                    }
                    catch
                    {
                        _logger.LogWarning("Poll delay cancelled");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                Stop();
                Start();
            }
        }
    }
}
