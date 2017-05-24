using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("tags"), Name("Tags")]
    [Summary("Search and view available tags.")]
    public class TagsModule : ModuleBase<SocketCommandContext>
    {
        private readonly TagDatabase _db;

        public TagsModule(IServiceProvider provider)
        {
            _db = provider.GetService<TagDatabase>();
        }

        [Command]
        public async Task TagsAsync()
        {
            var tags = await _db.GetTagsAsync(Context.Guild.Id);

            if (tags.Count() == 0)
            {
                await ReplyAsync("This guild has no tags yet!");
                return;
            }

            var builder = GetEmbed(tags, Context.Guild.Name, Context.Guild.IconUrl);
            await ReplyAsync("", embed: builder);
        }

        [Command]
        public async Task TagsAsync([Remainder]SocketUser user)
        {
            var tags = await _db.GetTagsAsync(Context.Guild.Id, user.Id);

            if (tags.Count() == 0)
            {
                await ReplyAsync("This user has no tags yet!");
                return;
            }

            var builder = GetEmbed(tags, user.ToString(), user.GetAvatarUrl());
            await ReplyAsync("", embed: builder);
        }

        [Command("random")]
        public async Task RandomAsync()
        {
            var tags = await _db.GetTagsAsync(Context.Guild.Id);
            
            if (tags.Count() == 0)
            {
                await ReplyAsync("This guild does not have any tags!");
                return;
            }

            var selectedIndex = new Random().Next(0, tags.Count());
            var tag = tags.ElementAt(selectedIndex);

            await ReplyAsync($"{tag.Aliases.First()}: {tag.Content}");
        }
        
        private EmbedBuilder GetEmbed(Tag[] tags, string name, string image)
        {
            string tagMessage = string.Join(", ", tags.Select(x => x.Aliases.First()));

            var builder = new EmbedBuilder();

            builder.ThumbnailUrl = image;
            builder.Title = $"Tags for {name}";
            builder.Description = tagMessage;

            return builder;
        }
    }
}
