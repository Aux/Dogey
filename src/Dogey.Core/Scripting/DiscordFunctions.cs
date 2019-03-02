using System;
using Discord;
using Dogey.Modules;
using Scriban.Runtime;

namespace Dogey.Scripting
{
    public class DiscordFunctions : ScriptObject
    {
        private readonly DogeyCommandContext _context;

        public DiscordFunctions(DogeyCommandContext context)
        {
            _context = context;

            var functions = new ScriptObject();
            functions.Import("react", new Action<string>((emote) =>
            {
                IEmote value;
                if (Emote.TryParse(emote, out Emote emoteValue))
                    value = emoteValue;
                else
                    value = new Emoji(emote);
                
                _context.Message.AddReactionAsync(value).GetAwaiter().GetResult();
            }));

            SetValue("discord", functions, true);
            SetValue("user", _context.User, true);
            SetValue("message", _context.Message, true);
            SetValue("channel", _context.Channel, true);
            SetValue("guild", _context.Guild, true);
        }
    }
}
