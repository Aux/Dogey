using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.SQLite.Modules
{
    [Group("tag")]
    [Remarks("Create and manage tags.")]
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

        [Command, Priority(10)]
        [Remarks("Execute the specified tag.")]
        public async Task TagAsync([Remainder]string name)
        {
            var tag = await _db.GetTagAsync(Context.Guild.Id, name.ToLower());

            if (tag == null)
            {
                var tags = await _db.FindTagsAsync(Context.Guild.Id, name, 5);

                string reply = $"Could not find a tag like `{name}`.";
                if (tags.Count() > 0)
                {
                    string related = string.Join(", ", tags.Select(x => x.Aliases.First()));
                    reply += $"\nDid you mean: {related}";
                }

                await ReplyAsync(reply);
                return;
            }

            await ReplyAsync($"{tag.Aliases.First()}: {tag.Content}");
        }

        [Command("create"), Priority(0)]
        [Remarks("Create a new tag.")]
        public async Task CreateAsync(string name, [Remainder]string content)
        {
            await _db.CreateTagAsync(Context, name.ToLower(), content);
            await Context.Message.AddReactionAsync(":thumbsup:");
        }

        [Command("edit"), Priority(0)]
        [Remarks("Edit and existing tag you own.")]
        public async Task EditAsync(string name, [Remainder]string content)
        {
            await Context.Message.AddReactionAsync(":thumbsup:");
        }

        [Command("alias"), Priority(0)]
        [Remarks("Add aliases to an existing tag.")]
        public async Task AliasAsync(string name, params string[] aliases)
        {
            await _db.AddAliasAsync(Context, name, aliases);
            await Context.Message.AddReactionAsync(":thumbsup:");
        }

        [Command("unalias"), Priority(0)]
        [Remarks("Remove an alias from an existing tag.")]
        public async Task UnaliasAsync(string name, params string[] aliases)
        {
            await _db.RemoveAliasAsync(Context, name, aliases);
            await Context.Message.AddReactionAsync(":thumbsup:");
        }

        [Command("delete"), Priority(0)]
        [Remarks("Delete an existing tag you own.")]
        public async Task DeleteAsync([Remainder]string name)
        {
            await _db.DeleteTagAsync(Context, name.ToLower());
            await Context.Message.AddReactionAsync(":thumbsup:");
        }

        [Command("info"), Priority(0)]
        [Remarks("Get information about a tag.")]
        public async Task InfoAsync([Remainder]string name)
        {
            var tag = await _db.GetTagAsync(Context.Guild.Id, name.ToLower());
            var builder = new EmbedBuilder();

            var author = Context.Guild.GetUser(tag.OwnerId);
            builder.Author = new EmbedAuthorBuilder()
            {
                IconUrl = author.GetAvatarUrl(),
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
