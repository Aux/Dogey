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
    public class StreamModule : ModuleBase<SocketCommandContext>
    {
        private readonly TwitchRestClient _client;

        public StreamModule(IServiceProvider provider)
        {
            _client = provider.GetService<TwitchRestClient>();
        }

        [Command("stream")]
        public async Task StreamAsync(string channel)
        {
            var user = (await _client.GetUsersAsync(channel)).FirstOrDefault();

            if (user == null)
            {
                await ReplyAsync($"I could not find a user named `{channel}`");
                return;
            }

            var stream = await _client.GetStreamAsync(user.Id);

            if (stream == null)
            {
                await ReplyAsync($"The channel `{channel}` is not currently streaming.");
                return;
            }

            var builder = new EmbedBuilder();
            
            builder.Description = stream.Channel.Status;
            builder.ImageUrl = stream.Previews["large"];
            builder.ThumbnailUrl = user.LogoUrl;

            builder.WithAuthor(author =>
            {
                author.Name = $"Click here to watch {user.DisplayName} on Twitch!";
                author.Url = stream.Channel.Url;
            });

            builder.AddInlineField("Viewers", stream.Viewers);
            builder.AddInlineField("Playing", stream.Game);

            await ReplyAsync("", embed: builder);
        }
    }
}
