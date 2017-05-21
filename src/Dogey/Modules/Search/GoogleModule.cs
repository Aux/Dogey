using Discord.Commands;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("google"), Alias("g"), Name("Google")]
    [Summary("Search google for the specified query")]
    public class GoogleModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;
        private readonly Configuration _config;
        private readonly CustomsearchService _search;
        
        public GoogleModule(IServiceProvider provider)
        {
            _commands = provider.GetService<CommandService>();
            _config = provider.GetService<Configuration>();
        }
        
        [Command]
        public Task BaseAsync()
            => new HelpModule(_commands).HelpAsync(Context, "google");

        [Command, Priority(10)]
        public async Task SearchAsync([Remainder]string query)
        {
            var links = await SearchGoogleAsync(query);

            if (links.Count() == 0)
            {
                await ReplyAsync("No results found.");
                return;
            }

            await ReplyAsync(GetMessage(links));
        }

        [Command, Priority(0)]
        public async Task SearchSiteAsync(Uri site, [Remainder]string query)
        {
            var links = await SearchGoogleAsync(query, site);

            if (links.Count() == 0)
            {
                await ReplyAsync("No results found.");
                return;
            }

            await ReplyAsync(GetMessage(links));
        }

        private async Task<IEnumerable<Result>> SearchGoogleAsync(string query, Uri site = null)
        {
            var request = _search.Cse.List(query);
            request.Cx = _config.CustomSearch.EngineId;
            if (site != null)
                request.SiteSearch = site.ToString();

            var result = await request.ExecuteAsync();
            return result.Items.Take(_config.CustomSearch.ResultCount);
        }

        private string GetMessage(IEnumerable<Result> links)
        {
            StringBuilder reply = new StringBuilder();
            reply.AppendLine(links.First().Link);
            reply.AppendLine();
            reply.AppendLine("**Related:**");

            foreach (var site in links.Skip(1))
                reply.AppendLine($"<{site.Link}>");

            return reply.ToString();
        }
    }
}
