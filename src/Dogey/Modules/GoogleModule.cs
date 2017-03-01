using Discord.Commands;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Search = Google.Apis.Customsearch.v1.Data;

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
                ApiKey = _config.Token,
                MaxUrlLength = 256
            });
        }

        [Command]
        public Task BaseAsync()
            => new HelpModule(_service).HelpAsync(Context, "google");

        [Command]
        public async Task GoogleAsync([Remainder]string query)
        {
            var sites = await SearchGoogleAsync(query);

            if (sites.Count() == 0)
            {
                await ReplyAsync("No results found.");
                return;
            }

            StringBuilder reply = new StringBuilder();
            reply.AppendLine(sites.First().Link);
            reply.AppendLine();
            reply.AppendLine("**See Also:**");

            foreach (var site in sites.Skip(1))
                reply.AppendLine($"<{site.Link}>");

            await ReplyAsync(reply.ToString());
        }

        private async Task<IEnumerable<Search.Result>> SearchGoogleAsync(string query)
        {
            var request = _search.Cse.List(query);
            request.Cx = _config.EngineId;

            var result = await request.ExecuteAsync();
            return result.Items.Take(_config.ResultCount);
        }
    }
}
