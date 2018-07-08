using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.Owner
{
    [RequireOwner]
    public class PlonkModule : DogeyModuleBase
    {
        public PlonkModule(RootController root)
            : base(root) { }

        [Command("plonk")]
        public async Task PlonkAsync([Remainder]IGuild guild)
        {
            var config = await _root.GetOrCreateConfigAsync(guild);
            if (config.BannedAt == null)
            {
                config.BannedAt = DateTime.UtcNow;
                await _root.ModifyAsync(config);
                await ReplySuccessAsync();
            }
        }

        [Command("plonk")]
        public async Task PlonkAsync([Remainder]SocketUser user)
        {
            var config = await _root.GetOrCreateConfigAsync(user);
            if (config.BannedAt == null)
            {
                config.BannedAt = DateTime.UtcNow;
                await _root.ModifyAsync(config);
                await ReplySuccessAsync();
            }
        }

        [Command("unplonk")]
        public async Task UnplonkAsync([Remainder]IGuild guild)
        {
            var config = await _root.GetOrCreateConfigAsync(guild);
            if (config.BannedAt != null)
            {
                config.BannedAt = null;
                await _root.ModifyAsync(config);
                await ReplySuccessAsync();
            }
        }

        [Command("unplonk")]
        public async Task UnplonkAsync([Remainder]SocketUser user)
        {
            var config = await _root.GetOrCreateConfigAsync(user);
            if (config.BannedAt != null)
            {
                config.BannedAt = null;
                await _root.ModifyAsync(config);
                await ReplySuccessAsync();
            }
        }
    }
}
