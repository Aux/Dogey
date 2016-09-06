using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.InfoModule
{
    [Module, Name("Info")]
    [RequireContext(ContextType.Guild)]
    public class RoleGroup
    {
        private DiscordSocketClient _client;

        public RoleGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("roleinfo")]
        [Description("Get information about this server.")]
        public async Task Roleinfo(IUserMessage msg, IRole role)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;

            var infomsg = new List<string>
            {
                "```xl",
                $"     Id: {role.Id}",
                $"   Name: {role.Name}",
                $"Created: {role.CreatedAt.ToString("ddddd, MMM dd yyyy, hh:mm:ss tt")}",
                $"  Color: {role.Color}",
                $"Hoisted: {role.IsHoisted}",
                $"Managed: {role.IsManaged}",
                $"  Users: {(await guild.GetUsersAsync()).Where(x => x.Roles.Contains(role)).Count()}",
                "```"
            };

            if (Globals.Config.IsSelfbot)
                await msg.ModifyAsync((e) => e.Content = string.Join("\n", infomsg));
            else
                await msg.Channel.SendMessageAsync(string.Join("\n", infomsg));
        }
        
        [Module("roleinfo"), Name("Info")]
        public class SubCommands
        {

        }
    }
}
