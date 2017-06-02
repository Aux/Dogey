using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Tags
{
    [Group("tag"), Name("Tag")]
    [Summary("View informations and stats for a tag")]
    public class TagInfoModule : ModuleBase<DogeyCommandContext>
    {
        private readonly TagManager _manager;

        public TagInfoModule(TagManager manager)
        {
            _manager = manager;
        }

        [Command("info"), Priority(1)]
        [Summary("")]
        public async Task InfoAsync([Remainder]string name)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);
            var author = Context.Guild.GetUser(tag.OwnerId);
            var count = await _manager.CountLogsAsync(tag.Id);

            var builder = new EmbedBuilder()
                .WithFooter(x => x.Text = "Created")
                .WithTimestamp(tag.CreatedAt)
                .AddInlineField("Owner", author.Mention)
                .AddInlineField("Uses", count)
                .AddInlineField("Aliases", string.Join(", ", tag.Aliases));

            builder.WithAuthor(x =>
            {
                x.Name = author.ToString();
                x.IconUrl = author.GetAvatarUrl();
            });

            await ReplyAsync("", embed: builder);
        }

        [Command("info"), Priority(10)]
        [Summary("")]
        public async Task InfoAsync(string name, [Remainder]SocketUser user)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);
            var count = await _manager.CountLogsAsync(tag.Id, user);

            await ReplyAsync($"{tag.Aliases.First()} has been used {count} time(s)");
        }

        [Command("info"), Priority(10)]
        [Summary("")]
        public async Task InfoAsync(string name, [Remainder]SocketChannel channel)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);
            var count = await _manager.CountLogsAsync(tag.Id, channel);

            await ReplyAsync($"{tag.Aliases.First()} has been used {count} time(s)");
        }

        [Command("info"), Priority(5)]
        [Summary("")]
        public async Task InfoAsync([Remainder]SocketUser user)
        {
            var count = await _manager.CountLogsAsync(user);

            await ReplyAsync($"{user} executed tags {count} time(s)");
        }

        [Command("info"), Priority(5)]
        [Summary("")]
        public async Task InfoAsync([Remainder]SocketChannel channel)
        {
            var count = await _manager.CountLogsAsync(channel);

            await ReplyAsync($"{channel} executed tags {count} time(s)");
        }
    }
}
