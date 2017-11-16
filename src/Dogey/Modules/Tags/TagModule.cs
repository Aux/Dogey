using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Tags
{
    [Name("Tag"), Group("tag"), Alias("tags")]
    [Summary("Create and manage tags for this guild")]
    public partial class TagModule : DogeyModuleBase
    {
        private readonly TagManager _manager;
        private readonly TagService _service;

        public TagModule(TagManager manager, TagService service)
        {
            _manager = manager;
            _service = service;
        }

        [Command]
        public async Task TagsAsync()
        {
            var tags = await _manager.GetPublicTagsAsync(Context.Guild.Id);

            var embed = new EmbedBuilder()
                .WithTitle("Public tags for this guild")
                .WithDescription(string.Join(", ", tags.Select(x => x.Name)));

            await ReplyAsync(embed);
        }

        [Priority(0)]
        [Command]
        public Task TagAsync(Tag tag)
            => ReplyAsync(tag.Content);
        
        [Priority(10)]
        [Command("create")]
        public async Task CreateAsync(string name, [Remainder]string content)
        {
            var embed = new EmbedBuilder();

            var tag = await _manager.FindTagAsync(Context.Guild.Id, name);
            if (tag != null)
            {
                embed.WithDescription($"A tag named `{name}` already exists.");
                await ReplyAsync(embed);
            }

            var defaults = await _manager.GetOrCreateDefaultsAsync(Context.User.Id);

            await _manager.CreateAsync(new Tag
            {
                OwnerId = Context.User.Id,
                GuildId = Context.Guild.Id,
                Name = name,
                Content = content,
                IsPublic = (Context.User as SocketGuildUser).GuildPermissions.Administrator,
                IsCommand = defaults.IsCommand
            });

            embed.WithDescription($"The tag `{name}` has been created.");
            await ReplyAsync(embed);

            if (defaults.IsCommand)
                _ = LoadTagAsync(name);
        }

        [Priority(10)]
        [Command("delete")]
        public Task DeleteAsync()
            => Task.CompletedTask;

        [Priority(10)]
        [Command("edit"), Alias("update", "modify", "change")]
        public Task EditAsync()
            => Task.CompletedTask;

        [Priority(10)]
        [Command("claim")]
        public Task ClaimAsync()
            => Task.CompletedTask;

        [Priority(10)]
        [Command("addalias")]
        public Task AddAliasAsync()
            => Task.CompletedTask;

        [Priority(10)]
        [Command("removealias")]
        public Task RemoveAliasAsync()
            => Task.CompletedTask;

        private async Task LoadTagAsync(string name)
        {
            var tag = await _manager.FindTagAsync(Context.Guild.Id, name);
            if (tag == null) return;

            await _service.AddCommandAsync(tag);
        }
    }
}
