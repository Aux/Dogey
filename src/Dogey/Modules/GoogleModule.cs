using Discord.Commands;
using Google.Apis.Customsearch.v1;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("google"), Alias("g")]
    public class GoogleModule : ModuleBase<SocketCommandContext>
    {
        private CustomsearchService _search;
        private CommandService _service;
        private CustomSearchConfig _config;
        private string _token;

        public GoogleModule(CommandService service)
        {
            _service = service;
            _config = Configuration.Load().CustomSearch;
            _token = string.IsNullOrWhiteSpace(_config.Token) 
                ? Configuration.Load().Token.Google 
                : _config.Token;
        }

        protected override void BeforeExecute()
        {
            if (string.IsNullOrWhiteSpace(_token))
                throw new InvalidOperationException("The Google module has not yet been configured. Please add the google token to the configuration file.");

            _search = new CustomsearchService(new BaseClientService.Initializer()
            {
                ApiKey =_token,
                MaxUrlLength = 256
            });
        }

        [Command]
        public Task BaseAsync()
            => new HelpModule(_service).HelpAsync(Context, "google");

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
            request.Cx = _config.EngineId;
            if (site != null)
                request.SiteSearch = site.ToString();

            var result = await request.ExecuteAsync();
            return result.Items.Take(_config.ResultCount);
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
