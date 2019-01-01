using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Configs
{
    [RequireUserPermission(GuildPermission.ManageRoles), RequireBotPermission(GuildPermission.ManageRoles)]
    public class ReactionRoleModule : DogeyModuleBase
    {
        private readonly ReactionRoleService _reactionRoleService;

        public ReactionRoleModule(RootController root, ReactionRoleService reactionRoleService) : base(root)
        {
            _reactionRoleService = reactionRoleService;
        }

        private async Task<SocketUserMessage> GetMessageFromUrlAsync(Uri url)
        {
            var ids = url.AbsolutePath.Split('/').Skip(3);

            var channelId = ids.ElementAtOrDefault(0);
            var messageId = ids.ElementAtOrDefault(1);

            if (!ulong.TryParse(channelId, out ulong chId))
            {
                await ReplyAsync($"Invalid url: `{channelId}` is not a channel id");
                return null;
            }

            var channel = Context.Guild.GetTextChannel(chId);

            if (!ulong.TryParse(messageId, out ulong msgId))
            {
                await ReplyAsync($"Invalid url: `{messageId}` is not a message id");
                return null;
            }

            return (await channel.GetMessageAsync(msgId)) as SocketUserMessage;
        }
        
        [Command("addreactionrole"), Alias("addreactrole")]
        public async Task AddReactionRoleAsync(Uri messageLinkUrl, [Remainder]IRole role)
        {
            var message = await GetMessageFromUrlAsync(messageLinkUrl);
            if (message == null) return;

            var reactionRole = await _root.CreateAsync(new ReactionRole
            {
                GuildId = Context.Guild.Id,
                MessageId = message.Id,
                RoleId = role.Id
            });

            _reactionRoleService.ReactionRoles.Add(reactionRole);
            await ReplyAsync("Success");
        }

        [Command("removereactionrole"), Alias("removereactrole")]
        public async Task RemoveReactionRoleAsync(Uri messageLinkUrl)
        {
            var message = await GetMessageFromUrlAsync(messageLinkUrl);
            if (message == null) return;

            var reactionRole = await _root.GetReactionRoleAsync(message.Id);
            await _root.DeleteAsync(reactionRole);

            _reactionRoleService.ReactionRoles.Remove(reactionRole);
            await ReplyAsync("Success");
        }
    }
}
