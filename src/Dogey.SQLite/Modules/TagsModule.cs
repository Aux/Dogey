using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.SQLite.Modules
{
    [Group("tags")]
    [Remarks("Search and view available tags.")]
    public class TagsModule : ModuleBase<SocketCommandContext>
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

        private EmbedBuilder GetEmbed(LiteTag[] tags, string name, string image)
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
