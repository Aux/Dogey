using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Name("LMGTFY")]
    [Summary("Let me google that for you")]
    public class LmgtfyModule : ModuleBase<DogeyCommandContext>
    {
        private const string _lmgtfyUrl = "http://lmgtfy.com/?q=";
        private const string _lmfgtfyUrl = "http://lmfgtfy.com/?q=";

        [Command("lmgtfy")]
        [Remarks("For when someone doesn't quite know how to use google.")]
        public Task LmgtfyAsync([Remainder]string query)
        {
            string cleanQuery = Uri.EscapeDataString(query);
            string url = _lmgtfyUrl + cleanQuery;
            return ReplyAsync(url);
        }

        [Command("lmfgtfy")]
        [Remarks("For when someone doesn't quite fucking know how to use google.")]
        public Task LmfgtfyAsync([Remainder]string query)
        {
            string cleanQuery = Uri.EscapeDataString(query);
            string url = _lmfgtfyUrl + cleanQuery;
            return ReplyAsync(url);
        }
    }
}
