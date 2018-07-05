using Discord;
using Discord.Commands;
using System.IO;
using System.Threading.Tasks;

namespace Dogey
{
    public class DogeyModuleBase : ModuleBase<DogeyCommandContext>
    {
        public Task ReplySuccessAsync()
            => ReplyAsync("Success");

        public Task ReplyEmbedAsync(Embed embed = null)
            => ReplyAsync("", false, embed, null);

        public Task ReplyReactionAsync(IEmote emote)
            => Context.Message.AddReactionAsync(emote);

        public Task ReplyFileAsync(Stream stream, string fileName, string message = null)
            => Context.Channel.SendFileAsync(stream, fileName, message, false, null);
        
    }
}
