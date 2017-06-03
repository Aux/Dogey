using Discord.Commands;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("google"), Alias("g"), Name("Google")]
    [Summary("Search google for sites relating to a query")]
    public class GoogleModule : ModuleBase<DogeyCommandContext>
    {
        private readonly CustomsearchService _search;
        private readonly Configuration _config;
        
        public GoogleModule(CustomsearchService search, Configuration config)
        {
            _search = search;
            _config = config;
        }
        
        [Command, Priority(10)]
        [Summary("Get some links relating to the specified query")]
        public async Task SearchAsync([Remainder]string query)
        {
            var links = await SearchGoogleAsync(query);

            if (!HasResults(links)) return;
            await ReplyAsync(GetMessage(links));
        }

        [Command, Priority(0)]
        [Summary("Search a specific website for the specified query")]
        public async Task SearchSiteAsync(Uri site, [Remainder]string query)
        {
            var links = await SearchGoogleAsync(query, site);

            if (!HasResults(links)) return;
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

        private bool HasResults(IEnumerable<Result> links)
        {
            if (links.Count() == 0)
            {
                var _ = ReplyAsync("No results found.");
                return false;
            }
            return true;
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
