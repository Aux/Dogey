using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Tags
{
    [Group("tags"), Name("Tags")]
    [Summary("Search and view information about available tags.")]
    public class TagsModule : ModuleBase<DogeyCommandContext>
    {
        private readonly TagManager _manager;
        private readonly Random _random;

        public TagsModule(TagManager manager, Random random)
        {
            _manager = manager;
            _random = random;
        }

        [Command]
        [Summary("View all available tags for this guild")]
        public async Task TagsAsync()
        {
            var tags = await _manager.GetTagsAsync(Context.Guild);

            if (!HasTags(Context.Guild, tags)) return;

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithTitle($"Tags for {Context.Guild}")
                .WithDescription(string.Join(", ", tags.Select(x => x.Aliases.First())));

            await ReplyAsync("", embed: builder);
        }

        [Command]
        [Summary("View all available tags for the specified user")]
        public async Task TagsAsync([Remainder]SocketUser user)
        {
            var tags = await _manager.GetTagsAsync(Context.Guild, user);

            if (!HasTags(user, tags)) return;

            var builder = new EmbedBuilder()
                .WithThumbnailUrl(user.GetAvatarUrl())
                .WithTitle($"Tags for {user}")
                .WithDescription(string.Join(", ", tags.Select(x => x.Aliases.First())));

            await ReplyAsync("", embed: builder);
        }

        [Command("random")]
        [Summary("Show a random tag from this guild")]
        public async Task RandomAsync()
        {
            var tags = await _manager.GetTagsAsync(Context.Guild);

            if (!HasTags(Context.Guild, tags)) return;

            var selected = SelectRandom(tags);
            var _ = _manager.AddLogAsync(selected, Context);
            await ReplyAsync($"{selected.Aliases.First()}: {selected.Content}");
        }

        [Command("random")]
        [Summary("Show a random tag from the specified user")]
        public async Task RandomAsync([Remainder]SocketUser user)
        {
            var tags = await _manager.GetTagsAsync(Context.Guild, user);

            if (!HasTags(Context.Guild, tags)) return;

            var selected = SelectRandom(tags);
            var _ = _manager.AddLogAsync(selected, Context);
            await ReplyAsync($"{selected.Aliases.First()}: {selected.Content}");
        }

        [Command("info"), Priority(0)]
        [Summary("Get information about the specified tag")]
        public async Task InfoAsync([Remainder]string name)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);
            var author = Context.Guild.GetUser(tag.OwnerId);
            var count = await _manager.CountLogsAsync(tag.Id);

            var builder = new EmbedBuilder()
                .WithFooter(x => x.Text = "Last Updated")
                .WithTimestamp(tag.UpdatedAt)
                .AddInlineField("Owner", author.Mention)
                .AddInlineField("Aliases", string.Join(", ", tag.Aliases))
                .AddInlineField("Uses", count);

            builder.WithAuthor(x =>
            {
                x.Name = author.ToString();
                x.IconUrl = author.GetAvatarUrl();
            });

            await ReplyAsync("", embed: builder);
        }

        [Command("info"), Priority(10)]
        [Summary("Get information about the specified tag in relation to the specified user")]
        public async Task InfoAsync(string name, [Remainder]SocketUser user)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);
            var count = await _manager.CountLogsAsync(tag.Id, user);

            await ReplyAsync($"{tag.Aliases.First()} has been used {count} time(s)");
        }

        [Command("info"), Priority(10)]
        [Summary("Get information about the specified tag in relation to the specified channel")]
        public async Task InfoAsync(string name, [Remainder]SocketChannel channel)
        {
            var tag = await _manager.GetTagAsync(name, Context.Guild);
            var count = await _manager.CountLogsAsync(tag.Id, channel);

            await ReplyAsync($"{tag.Aliases.First()} has been used {count} time(s)");
        }

        [Command("info"), Priority(5)]
        [Summary("Get information about tags for the specified user")]
        public async Task InfoAsync([Remainder]SocketUser user)
        {
            var count = await _manager.CountLogsAsync(user);
            await ReplyAsync($"{user} executed tags {count} time(s)");
        }

        [Command("info"), Priority(5)]
        [Summary("Get information about tags for the specified channel")]
        public async Task InfoAsync([Remainder]SocketChannel channel)
        {
            var count = await _manager.CountLogsAsync(channel);
            await ReplyAsync($"{channel} executed tags {count} time(s)");
        }

        private bool HasTags(object obj, IEnumerable<Tag> tags)
        {
            if (tags.Count() == 0)
            {
                var _ = ReplyAsync($"{obj} currently has no tags.");
                return false;
            }
            return true;
        }

        private Tag SelectRandom(IEnumerable<Tag> tags)
        {
            var index = _random.Next(0, tags.Count());
            var selected = tags.ElementAt(index);
            return selected;
        }
    }
}
