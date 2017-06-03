using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Tags
{
    [Group("tag"), Name("Tag")]
    [Summary("Create and manage tags for this guild")]
    public class TagModule : ModuleBase<DogeyCommandContext>
    {
        private readonly TagManager _manager;

        public TagModule(TagManager manager)
        {
            _manager = manager;
        }
        
        [Command, Priority(0)]
        [Summary("Show the tag associated with the specified name")]
        public async Task TagAsync([Remainder]string name)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);

            if (await _manager.IsDupeExecutionAsync(tag.Id)) return;
            if (NotExists(tag, name)) return;

            var _ = _manager.AddLogAsync(tag, Context);
            await ReplyAsync($"{tag.Aliases.First()}: {tag.Content}");
        }

        [Priority(10)]
        [Command("create"), Alias("new", "add")]
        [Summary("Create a new tag for this guild")]
        public async Task CreateAsync(string name, [Remainder]string content)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);

            if (Exists(tag, name)) return;

            await _manager.CreateTagAsync(name, content, Context);
            await ReplyAsync(":thumbsup:");
        }

        [Priority(10)]
        [Command("delete"), Alias("remove")]
        [Summary("Delete an existing tag from this guild")]
        public async Task DeleteAsync(string name)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);

            if (NotExists(tag, name)) return;

            await _manager.DeleteTagAsync(tag);
            await ReplyAsync(":thumbsup:");
        }

        [Priority(10)]
        [Command("modify"), Alias("edit", "change")]
        [Summary("Modify an existing tag from this guild")]
        public async Task ModifyAsync(string name, [Remainder]string content)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);

            if (NotExists(tag, name) || !IsOwner(tag)) return;

            await _manager.ModifyTagAsync(tag, content);
            await ReplyAsync(":thumbsup:");
        }

        [Priority(10)]
        [Command("setowner"), Alias("donate", "give")]
        [Summary("Change the owner of a tag in this guild")]
        public async Task SetOwnerAsync(string name, [Remainder]SocketUser user)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);

            if (NotExists(tag, name) || !IsOwner(tag)) return;

            await _manager.SetOwnerAsync(tag, user);
            await ReplyAsync(":thumbsup:");
        }

        [Priority(10)]
        [Command("alias"), Alias("addalias")]
        [Summary("Add a new alias to the specified tag")]
        public async Task AddAliasAsync(string name, params string[] aliases)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);

            if (NotExists(tag, name)) return;

            await _manager.AddAliasesAsync(tag, aliases);
            await ReplyAsync(":thumbsup:");
        }

        [Priority(10)]
        [Command("unalias"), Alias("removealias")]
        [Summary("Remove an existing alias from the specified tag")]
        public async Task RemoveAliasAsync(string name, params string[] aliases)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);

            if (NotExists(tag, name) || !IsOwner(tag)) return;

            await _manager.RemoveAliasesAsync(tag, aliases);
            await ReplyAsync(":thumbsup:");
        }

        private bool IsOwner(Tag tag)
        {
            if (tag.OwnerId != Context.User.Id)
            {
                var _ = ReplyAsync("You are not the owner of this tag");
                return false;
            }
            return true;
        }

        private bool Exists(Tag tag, string name)
        {
            if (tag != null)
            {
                var _ = ReplyAsync($"The tag `{name}` already exists");
                return true;
            }
            return false;
        }

        private bool NotExists(Tag tag, string name)
        {
            if (tag == null)
            {
                var _ = ReplySuggestionsAsync(name);
                return true;
            }
            return false;
        }

        private async Task ReplySuggestionsAsync(string name)
        {
            string msg = $"The tag `{name}` does not exist";
            var tags = await _manager.FindTagsAsync(name, Context.Guild);

            if (tags.Count() != 0)
                msg += $"\nDid you mean: {string.Join(", ", tags.Select(x => x.Aliases.First()))}";
            
            await ReplyAsync(msg);
        }
    }
}
