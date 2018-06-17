using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.Owner
{
    public class PlonkModule : DogeyModuleBase
    {
        private readonly RootController _root;

        public PlonkModule(RootController root)
        {
            _root = root;
        }

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
        public async Task PlonkAsync([Remainder]IUser user)
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
        public async Task UnplonkAsync([Remainder]IUser user)
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
