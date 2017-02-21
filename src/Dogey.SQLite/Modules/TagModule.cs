using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.SQLite.Modules
{
    [Group("tag")]
    public class TagModule : ModuleBase<SocketCommandContext>
    {
        private TagDatabase _db;

        protected override void BeforeExecute()
        {
            _db = new TagDatabase();
        }

        protected override void AfterExecute()
        {
            _db.Dispose();
        }

        [Command, Priority(0)]
        [Remarks("Execute the specified tag.")]
        public async Task TagAsync([Remainder]string name)
        {
            var tag = await _db.GetTagAsync(Context, name.ToLower());

            if (tag == null)
            {
                var tags = await _db.FindTagsAsync(Context, name, 5);
                string related = string.Join(", ", tags.Select(x => x.Aliases.First()));
                await ReplyAsync($"Could not find a tag like `{name}`. Did you mean:\n{related}");
                return;
            }

            await ReplyAsync($"{tag.Aliases.First()}: {tag.Content}");
        }

        [Command("create"), Priority(10)]
        [Remarks("Create a new tag.")]
        public async Task CreateAsync(string name, [Remainder]string content)
        {
            await _db.CreateTagAsync(Context, name.ToLower(), content);
            await ReplyAsync(":thumbsup:");
        }

        [Command("edit"), Priority(10)]
        [Remarks("Edit and existing tag you own.")]
        public Task EditAsync(string name, [Remainder]string content)
        {
            return Task.CompletedTask;
        }

        [Command("alias"), Priority(10)]
        [Remarks("Add aliases to an existing tag.")]
        public Task AliasAsync(string name, params string[] aliases)
        {
            return Task.CompletedTask;
        }

        [Command("unalias"), Priority(10)]
        [Remarks("Remove an alias to an existing tag.")]
        public Task UnaliasAsync(string name, string alias)
        {
            return Task.CompletedTask;
        }

        [Command("delete"), Priority(10)]
        [Remarks("Delete an existing tag you own.")]
        public async Task DeleteAsync([Remainder]string name)
        {
            await _db.DeleteTagAsync(Context, name.ToLower());
            await ReplyAsync(":thumbsup:");
        }

        [Command("info"), Priority(10)]
        [Remarks("Get information about a tag.")]
        public async Task InfoAsync([Remainder]string name)
        {
            var tag = await _db.GetTagAsync(Context, name.ToLower());
            var builder = new EmbedBuilder();

            var author = Context.Guild.GetUser(tag.OwnerId);
            builder.Author = new EmbedAuthorBuilder()
            {
                IconUrl = author.AvatarUrl,
                Name = $"{author.ToString()} ({author.Id})"
            };

            builder.Title = "Tag Info";
            builder.Description = $"Owner: {author.Mention}\nCreated: {tag.CreatedAt}\nUpdated: {tag.UpdatedAt}";

            builder.AddField(x =>
            {
                x.Name = "Aliases";
                x.Value = string.Join(", ", tag.Aliases);
            });

            await ReplyAsync("", embed: builder);
        }
    }
}
