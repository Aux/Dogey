using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Dogey.Modules.Configs
{
    public class ReactionRoleModule : DogeyModuleBase
    {
        private readonly ReactionRoleService _reactionRoleService;

        public ReactionRoleModule(RootController root, ReactionRoleService reactionRoleService) : base(root)
        {
            _reactionRoleService = reactionRoleService;
        }

        [Priority(1)]
        [RequireOwner]
        [Command("createreactionrole"), Alias("createreactrole", "addreactionrole", "addreactrole")]
        public async Task AddReactionRoleAsync(SocketTextChannel channel, ulong messageId, [Remainder]IRole role)
        {
            var message = (await channel.GetMessageAsync(messageId)) as SocketUserMessage;

            var reactionRole = await _root.CreateAsync(new ReactionRole
            {
                GuildId = Context.Guild.Id,
                MessageId = message.Id,
                RoleId = role.Id
            });

            _reactionRoleService.ReactionRoles.Add(reactionRole);
            await ReplyAsync("Success");
        }
    }
}
