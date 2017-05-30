using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("tag"), Name("Tag")]
    [Summary("Create and manage tags.")]
    public class TagModule : ModuleBase<DogeyCommandContext>
    {
        private readonly TagDatabase _db;

        public TagModule(IServiceProvider provider)
        {
            _db = provider.GetService<TagDatabase>();
        }
        
        [Command, Priority(0)]
        [Summary("Execute the specified tag.")]
        public async Task TagAsync([Remainder]string name)
        {
            var tag = await _db.GetTagAsync(Context.Guild.Id, name.ToLower());

            if (tag == null)
            {
                await SuggestTagsAsync(name);
                return;
            }

            await ReplyAsync($"{tag.Aliases.First()}: {tag.Content}");
        }

        [Command("create"), Priority(10)]
        [Summary("Create a new tag.")]
        public async Task CreateAsync(string name, [Remainder]string content)
        {
            await _db.CreateTagAsync(Context, name.ToLower(), content);
            await ReplyAsync(":thumbsup:");
        }

        //[Command("edit"), Priority(10)]
        //[Summary("Edit and existing tag you own.")]
        //public async Task EditAsync(string name, [Remainder]string content)
        //{
        //    await ReplyAsync(":thumbsup:");
        //}

        [Command("alias"), Priority(10)]
        [Summary("Add aliases to an existing tag.")]
        public async Task AliasAsync(string name, params string[] aliases)
        {
            await _db.AddAliasAsync(Context, name, aliases);
            await ReplyAsync(":thumbsup:");
        }

        [Command("unalias"), Priority(10)]
        [Summary("Remove aliases from an existing tag.")]
        public async Task UnaliasAsync(string name, params string[] aliases)
        {
            await _db.RemoveAliasAsync(Context, name, aliases);
            await ReplyAsync(":thumbsup:");
        }

        [Command("delete"), Priority(10)]
        [Summary("Delete an existing tag you own.")]
        public async Task DeleteAsync([Remainder]string name)
        {
            await _db.DeleteTagAsync(Context, name.ToLower());
            await ReplyAsync(":thumbsup:");
        }

        [Command("info"), Priority(10)]
        [Summary("Get information about a tag.")]
        public async Task InfoAsync([Remainder]string name)
        {
            var tag = await _db.GetTagAsync(Context.Guild.Id, name.ToLower());
            var builder = new EmbedBuilder();

            var author = Context.Guild.GetUser(tag.OwnerId);
            builder.Author = new EmbedAuthorBuilder()
            {
                IconUrl = author.GetAvatarUrl(),
                Name = author.ToString()
            };
            
            builder.AddInlineField("Owner", author.Mention);
            builder.AddInlineField("Aliases", string.Join(", ", tag.Aliases));
            builder.Timestamp = tag.CreatedAt;
            
            await ReplyAsync("", embed: builder);
        }
        
        private async Task SuggestTagsAsync(string name)
        {
            var tags = await _db.FindTagsAsync(Context.Guild.Id, name, 5);

            var reply = new StringBuilder();

            reply.Append($"Could not find a tag like `{name}`");
            if (tags.Count() > 0)
            {
                reply.AppendLine("Did you mean:");
                foreach (var script in tags)
                    reply.AppendLine(script.Aliases.First());
            }
            
            await ReplyAsync(reply.ToString());
        }
    }
}
