using Discord;
using Discord.Commands;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [RequireOwner]
    public class OwnerModule : DogeyModuleBase
    {
        public OwnerModule(RootController root)
            : base(root) { }

        [Command("reactwith")]
        public async Task ReactWithAsync(Emote emote)
        {
            await Context.Message.AddReactionAsync(emote);
        }

        [Command("setstreaming")]
        public async Task SetActivityAsync(Uri url, [Remainder]string name)
        {
            await Context.Client.SetActivityAsync(new StreamingGame(name, url.ToString()));
            await ReplySuccessAsync();
        }

        [Command("setgame")]
        public async Task SetActivityAsync([Remainder]string activity)
        {
            await Context.Client.SetGameAsync(activity);
            await ReplySuccessAsync();
        }

        [Command("setstatus")]
        public async Task SetStatusAsync(UserStatus userStatus)
        {
            await Context.Client.SetStatusAsync(userStatus);
            await ReplySuccessAsync();
        }

        [Command("setavatar")]
        public async Task SetAvatarAsync(Uri url)
        {
            try
            {
                using (var client = new HttpClient())
                using (var request = new HttpRequestMessage(HttpMethod.Get, url))
                {
                    var response = await client.SendAsync(request);
                    if (!response.Content.Headers.ContentType.ToString().StartsWith("image"))
                    {
                        await ReplyAsync("Invalid image link provided");
                        return;
                    }

                    var stream = await response.Content.ReadAsStreamAsync();
                    await Context.Client.CurrentUser.ModifyAsync(x => x.Avatar = new Image(stream));
                }
                await ReplySuccessAsync();
            } catch (HttpRequestException)
            {
                await ReplyAsync("Invalid image link provided");
            }
        }
    }
}
