using Discord.WebSocket;
using Dogey.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        private readonly DiscordSocketClient _discord;
        private readonly ILogger<RssService> _logger;
        private readonly HttpClient _http;

        public RssService(DiscordSocketClient discord, ILogger<RssService> logger, HttpClient http)
        {
            Feeds = new List<RssFeed>();

            _discord = discord;
            _logger = logger;
            _http = http;
        }

        public override void Start()
        {
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

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
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
                        {

                        } else
                        if (atomNode != null)
                        {
                            articles.AddRange(atomNode
                                .Elements("{http://www.w3.org/2005/Atom}entry")
                                .Select(x => new RssArticle
                                {
                                    FeedId = feed.Id,
                                    Title = x.Element("{http://www.w3.org/2005/Atom}title")?.Value,
                                    Link = x.Element("{http://www.w3.org/2005/Atom}link")?.Attribute("href")?.Value,
                                    PublishedAt = DateTime.Parse(x.Element("{http://www.w3.org/2005/Atom}published").Value)
                                }));
                        } else
                        {
                            throw new InvalidOperationException("Unknown feed type found");
                        }

                        articles = articles
                            .Where(x => x.PublishedAt > feed.UpdatedAt)
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
                    catch { }
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
