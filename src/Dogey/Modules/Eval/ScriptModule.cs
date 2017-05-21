using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.Eval
{
    [Group("script"), Name("Script")]
    [Summary("Create and manage scripts.")]
    public class ScriptModule : ModuleBase<SocketCommandContext>, IDisposable
    {
        private readonly RoslynManager _roslyn;
        private readonly ScriptDatabase _db;

        public ScriptModule(IServiceProvider provider)
        {
            _roslyn = provider.GetService<RoslynManager>();
            _db = provider.GetService<ScriptDatabase>();
        }

        [Command, Priority(0)]
        [Summary("Execute the specified script")]
        public async Task ScriptAsync([Remainder]string name)
        {
            await ReplyAsync(":thumbsup:");
        }

        [Command("create"), Priority(10)]
        [Summary("Create a new script")]
        public async Task CreateAsync(string name, [Remainder]string content)
        {
            await ReplyAsync(":thumbsup:");
        }

        [Command("edit"), Priority(10)]
        [Summary("Edit an existing script you own")]
        public async Task EditAsync(string name, [Remainder]string content)
        {
            await ReplyAsync(":thumbsup:");
        }

        [Command("alias"), Priority(10)]
        [Summary("Add aliases to an existing script")]
        public async Task AliasAsync(string name, params string[] aliases)
        {
            await ReplyAsync(":thumbsup:");
        }

        [Command("unalias"), Priority(10)]
        [Summary("Remove aliases from an existing script")]
        public async Task UnaliasAsync(string name, params string[] aliases)
        {
            await ReplyAsync(":thumbsup:");
        }

        [Command("delete"), Priority(10)]
        [Summary("Delete an existing script you own")]
        public async Task DeleteAsync([Remainder]string name)
        {
            await ReplyAsync(":thumbsup:");
        }

        [Command("gist"), Priority(10)]
        [Summary("Get a link to this script on Github Gist")]
        public async Task GistAsync([Remainder]string name)
        {
            await ReplyAsync(":thumbsup:");
        }

        [Command("info"), Priority(10)]
        [Summary("Get information about a script")]
        public async Task InfoAsync([Remainder]string name)
        {
            await ReplyAsync(":thumbsup:");
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
