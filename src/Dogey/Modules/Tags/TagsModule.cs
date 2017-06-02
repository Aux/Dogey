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

            if (!HasTags(Context.Guild, tags))
                return;

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

            if (!HasTags(user, tags))
                return;

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

            if (!HasTags(Context.Guild, tags))
                return;

            var selected = SelectRandom(tags);
            await ReplyAsync($"{selected.Aliases.First()}: {selected.Content}");
        }

        [Command("random")]
        [Summary("Show a random tag from the specified user")]
        public async Task RandomAsync([Remainder]SocketUser user)
        {
            var tags = await _manager.GetTagsAsync(Context.Guild, user);

            if (!HasTags(Context.Guild, tags))
                return;

            var selected = SelectRandom(tags);
            await ReplyAsync($"{selected.Aliases.First()}: {selected.Content}");
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
