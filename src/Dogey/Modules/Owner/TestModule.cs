using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Dogey.Modules.Owner
{
    [RequireOwner]
    public class TestModule : DogeyModuleBase
    {
        private readonly ResponsiveService _responsive;

        public TestModule(ResponsiveService responsive, RootController root)
            : base(root)
        {
            _responsive = responsive;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            _responsive.SetContext(Context);
        }

        [Command("test")]
        public async Task TestResponsiveAsync()
        {
            await ReplyAsync("Say something pls");
            var msg = await _responsive.WaitForReplyAsync(Context.Channel, Context.User);
            if (msg == null)
            {
                await ReplyAsync("You didn't say anything in time and I got bored");
                return;
            }

            await ReplyAsync("You said: " + msg.Content);
        }

        [Command("success")]
        public async Task SuccessAsync()
        {
            await ReplySuccessAsync();
        }

        [Command("reacttest")]
        public async Task TestReactionAsync()
        {
            var reactMsg = await ReplyAsync("What emote should I react with?");
            var reaction = await _responsive.WaitForReactionAsync(reactMsg, Context.User);
            await ReplyReactionAsync(reaction.Emote);
        }

        [Command("testreply")]
        public async Task TestReplyAsync()
        {
            var msg = await ReplyAsync("Should I do something neat?");
            var reply = await _responsive.WaitForReplyAsync(Context.Channel, Context.User);
            if (reply == null)
            {
                await ReplyAsync("You didn't say anything in time and I got bored");
                return;
            }

            if (_responsive.IsSuccessReply(reply))
            {
                await ReplyAsync("Yay! But I don't have any neat things right now :'(");
                return;
            }
            await ReplyAsync("aw darn...");
        }
    }
}
