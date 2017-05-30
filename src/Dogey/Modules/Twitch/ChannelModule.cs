using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NTwitch.Rest;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Twitch
{
    [Group("twitch"), Name("Twitch")]
    [Summary("")]
    public class ChannelModule : ModuleBase<DogeyCommandContext>
    {
        private readonly TwitchRestClient _client;

        public ChannelModule(IServiceProvider provider)
        {
            _client = provider.GetService<TwitchRestClient>();
        }

        [Command("channel")]
        public async Task ChannelAsync(string name)
        {
            var user = (await _client.GetUsersAsync(name)).FirstOrDefault();

            if (user == null)
            {
                await ReplyAsync($"I could not find a channel named `{name}`");
                return;
            }

            var channel = await user.GetChannelAsync();

            var builder = new EmbedBuilder();

            builder.Description = user.Bio;
            builder.ThumbnailUrl = user.LogoUrl;

            builder.WithAuthor(author =>
            {
                author.Name = $"Click here to view {user.DisplayName}'s profile";
                author.Url = channel.Url;
            });

            builder.AddInlineField("Created", channel.CreatedAt);
            builder.AddInlineField("Updated", channel.UpdatedAt);
            builder.AddField("Partner", channel.IsPartner);
            builder.AddInlineField("Mature", channel.IsMature);
            builder.AddField("Followers", channel.Followers);

            await ReplyAsync("", embed: builder);
        }
    }
}
