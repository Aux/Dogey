using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("lmgtfy")]
    public class LmgtfyModule : ModuleBase<SocketCommandContext>
    {
        private const string _lmgtfyUrl = "http://lmgtfy.com/?q=";

        [Command]
        [Remarks("For when someone does not quite know how to use google.")]
        public Task LmgtfyAsync([Remainder]string query)
        {
            string cleanQuery = Uri.EscapeDataString(query);
            string url = _lmgtfyUrl + cleanQuery;
            return ReplyAsync(url);
        }
    }
}
