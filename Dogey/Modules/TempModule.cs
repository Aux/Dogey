using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module]
    [Name("General")]
    public class TempModule
    {
        private DiscordSocketClient _client;

        public TempModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("tempvoice")]
        public async Task TempVoice(IUserMessage msg)
        {
            var user = msg.Author as IGuildUser ?? null;

            if (user.VoiceChannel == null)
            {
                await msg.Channel.SendMessageAsync("You must be in a voice channel before you can create a temporary one.");
            } else
            {

            }
        }

    }
}
