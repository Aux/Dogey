using System;
using Discord;
using Dogey.Modules;
using Scriban.Runtime;

namespace Dogey.Scripting
{
    public class DiscordFunctions : ScriptObject
    {
        public DiscordFunctions(DogeyCommandContext context)
        {
            var discordFuncs = new ScriptObject();
            discordFuncs.Import("sendreaction", new Action<string>((emote) =>
            {
                IEmote value;
                if (Emote.TryParse(emote, out Emote emoteValue))
                    value = emoteValue;
                else
                    value = new Emoji(emote);

                context.Message.AddReactionAsync(value).GetAwaiter().GetResult();
            }));
            discordFuncs.SetValue("user", context.User, true);
            discordFuncs.SetValue("message", context.Message, true);
            discordFuncs.SetValue("channel", context.Channel, true);
            discordFuncs.SetValue("guild", context.Guild, true);

            var embedBuilder = new EmbedBuilder();
            var embedFuncs = new ScriptObject();
            embedFuncs.Import("author", new Action<string>((value) => embedBuilder.WithAuthor(value)));
            embedFuncs.Import("description", new Action<string>((value) => embedBuilder.Description = value));
            embedFuncs.Import("footer", new Action<string>((value) => embedBuilder.WithFooter(value)));
            embedFuncs.Import("image", new Action<string>((value) => embedBuilder.ImageUrl = value));
            embedFuncs.Import("thumbnail", new Action<string>((value) => embedBuilder.ThumbnailUrl = value));
            embedFuncs.Import("image", new Action<DateTime>((value) => embedBuilder.Timestamp = value));
            embedFuncs.Import("title", new Action<string>((value) => embedBuilder.Title = value));
            embedFuncs.Import("url", new Action<string>((value) => embedBuilder.Url = value));
            embedFuncs.Import("color", new Action<int, int, int>((r, g, b) => embedBuilder.Color = new Color(r, g, b)));
            embedFuncs.Import("addfield", new Action<string, string>((name, value) => embedBuilder.AddField(name, value)));
            embedFuncs.Import("send", new Action(() =>
            {
                context.Channel.SendMessageAsync(embed: embedBuilder.Build());
            }));

            SetValue("discord", discordFuncs, true);
            SetValue("embed", embedFuncs, false);
        }
    }
}
