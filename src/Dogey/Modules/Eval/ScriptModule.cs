using Discord;
using Discord.Commands;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Eval
{
    [RequireOwner]
    [Group("script"), Name("Script")]
    public class ScriptModule : DogeyModuleBase
    {
        private readonly ScriptManager _manager;
        private readonly RoslynManager _roslyn;

        public ScriptModule(ScriptManager manager, RoslynManager roslyn)
        {
            _manager = manager;
            _roslyn = roslyn;
        }

        [Command(RunMode = RunMode.Async), Priority(0)]
        [Summary("Execute the specified script")]
        public async Task ScriptAsync([Remainder]string name)
        {
            var script = await _manager.GetScriptAsync(name);

            if (script == null)
            {
                await SuggestScriptAsync(name);
                return;
            }

            var result = await _roslyn.EvalAsync(Context, script);
            await ReplyAsync(result);
        }

        [Command("create"), Priority(10)]
        [Summary("Create a new script")]
        public async Task CreateAsync(string name, [Remainder]string content)
        {
            string cleancode = _roslyn.GetFormattedCode("cs", content);
            await _manager.CreateScriptAsync(Context.User.Id, name, cleancode);
            await ReplyAsync(":thumbsup:");
        }

        //[Command("edit"), Priority(10)]
        //[Summary("Edit an existing script you own")]
        //public async Task EditAsync(string name, [Remainder]string content)
        //{
        //    await ReplyAsync(":thumbsup:");
        //}

        //[Command("alias"), Priority(10)]
        //[Summary("Add aliases to an existing script")]
        //public async Task AliasAsync(string name, params string[] aliases)
        //{
        //    await ReplyAsync(":thumbsup:");
        //}

        //[Command("unalias"), Priority(10)]
        //[Summary("Remove aliases from an existing script")]
        //public async Task UnaliasAsync(string name, params string[] aliases)
        //{
        //    await ReplyAsync(":thumbsup:");
        //}

        [Command("delete"), Priority(10)]
        [Summary("Delete an existing script you own")]
        public async Task DeleteAsync([Remainder]string name)
        {
            await _manager.DeleteScriptAsync(name);
            await ReplyAsync(":thumbsup:");
        }

        //[Command("gist"), Priority(10)]
        //[Summary("Get a link to this script on Github Gist")]
        //public async Task GistAsync([Remainder]string name)
        //{
        //    await ReplyAsync(":thumbsup:");
        //}

        [Command("info"), Priority(10)]
        [Summary("Get information about a script")]
        public async Task InfoAsync([Remainder]string name)
        {
            var script = await _manager.GetScriptAsync(name.ToLower());
            var builder = new EmbedBuilder();

            var author = Context.Guild.GetUser(script.OwnerId);
            builder.Author = new EmbedAuthorBuilder()
            {
                IconUrl = author.GetAvatarUrl(),
                Name = author.ToString()
            };

            builder.AddField("Code", $"```cs\n{script.Content}```");
            builder.AddInlineField("Owner", author.Mention);
            builder.AddInlineField("Aliases", string.Join(", ", script.Aliases));
            builder.Timestamp = script.CreatedAt;

            await ReplyAsync("", embed: builder);
        }

        private async Task SuggestScriptAsync(string name)
        {
            var scripts = await _manager.FindScriptsAsync(name, 3);

            var reply = new StringBuilder();

            reply.Append($"Could not find a script like `{name}`");
            if (scripts.Count() > 0)
            {
                reply.AppendLine("Did you mean:");
                foreach (var script in scripts)
                    reply.AppendLine(script.Aliases.First());
            }

            await ReplyAsync(reply.ToString());
        }
    }
}
