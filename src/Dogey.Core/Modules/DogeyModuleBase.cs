using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Rest;

namespace Dogey.Modules
{
    public class DogeyModuleBase : ModuleBase<DogeyCommandContext>
    {
        public Task<IUserMessage> ReplyEmbedAsync(Embed embed)
            => ReplyAsync("", false, embed, null);
        public Task<IUserMessage> ReplyEmbedAsync(EmbedBuilder builder)
            => ReplyAsync("", false, builder.Build(), null);

        public Task ReplyReactionAsync(IEmote emote)
            => Context.Message.AddReactionAsync(emote);

        public Task<RestUserMessage> ReplyFileAsync(Stream stream, string fileName, string message = null)
            => Context.Channel.SendFileAsync(stream, fileName, message, false, null);
    }
}
