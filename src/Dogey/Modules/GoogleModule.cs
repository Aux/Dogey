using Discord.Commands;
using Google.Apis.Customsearch.v1;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("google"), Alias("g")]
    public class GoogleModule : DogeyModuleBase
    {
        private readonly CustomsearchService _cse;
        private readonly IConfiguration _config;

        public GoogleModule(CustomsearchService cse, IConfiguration config)
        {
            _cse = cse;
            _config = config;
        }

        [Command, Priority(1)]
        public Task SearchAsync([Remainder]string query)
            => SearchAsync(null, query);

        [Command, Priority(0)]
        public async Task SearchAsync(Uri url, [Remainder]string query)
        {
            var request = _cse.Cse.List(query);
            request.Cx = _config["google:cse_id"];
            request.SiteSearch = url?.ToString();
            
            var result = await request.ExecuteAsync();
            if (result.Items.Count() == 0)
            {
                await ReplyAsync("No results found.");
                return;
            }

            var links = result.Items.Take(4);

            var reply = new StringBuilder();
            reply.AppendLine(links.First().Link);
            reply.AppendLine();
            reply.AppendLine("**Related:**");

            foreach (var site in links.Skip(1))
                reply.AppendLine($"<{site.Link}>");

            await ReplyAsync(reply.ToString());
        }
    }
}
