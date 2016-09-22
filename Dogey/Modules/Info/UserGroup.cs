using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Dogey.Models;
using Dogey.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.InfoModule
{
    [Module, Name("Info")]
    [RequireContext(ContextType.Guild)]
    public class UserGroup
    {
        private DiscordSocketClient _client;

        public UserGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("favwords")]
        [Description("Get information about this user.")]
        public async Task favwords(IUserMessage msg, IUser user = null, int page = 1)
        {
            int perpage = 5;
            int pagenum = page * perpage - perpage;
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            var u = user as IGuildUser ?? msg.Author as IGuildUser;
            using (var db = new DataContext())
            {
                var words = db.MessageLogs.Where(x => x.AuthorId == u.Id)
                              .SelectMany(x => x.Content.ToLower().Split(' '))
                              .GroupBy(x => x).OrderByDescending(x => x.Count())
                              .Skip(pagenum).Take(perpage)
                              .Select(x => $"{x.Key}: {x.Count()}");
                
                var message = new List<string>
                {
                    $"Favorite Words pg{page}",
                    "```xl",
                    string.Join("\n", words),
                    "```"
                };

                await msg.Channel.SendMessageAsync(string.Join("\n", message));
            }
        }

        [Command("userinfo")]
        [Description("Get information about this user.")]
        public async Task Userinfo(IUserMessage msg, [Remainder]IUser user = null)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            var u = user as IGuildUser ?? msg.Author as IGuildUser;

            int msgcount, cmdcount;
            using (var db = new DataContext())
            {
                msgcount = db.MessageLogs.Where(x => x.AuthorId == u.Id && x.GuildId == guild.Id).Count();
                cmdcount = db.Commands.Where(x => x.OwnerId == u.Id).Count();
            }
            
            var infomsg = new List<string>
            {
                "```xl",
                $"   Name: {u} ({u.Id})",
                $" Status: {Enum.GetName(typeof(UserStatus), u.Status)}",
                $"Created: {u.CreatedAt.ToString("ddddd, MMM dd yyyy, hh:mm:ss tt")}",
                $" Joined: {u.JoinedAt.Value.ToString("ddddd, MMM dd yyyy, hh:mm:ss tt")}",
                $"  Roles: {u.Roles.Count()}",
                $"   Msgs: {msgcount}",
                $"   Cmds: {cmdcount}",
                $" Avatar: {u.AvatarUrl}",
                "```"
            };

            await msg.Channel.SendMessageAsync(string.Join("\n", infomsg));
        }

        [Module("userinfo"), Name("Info")]
        public class SubCommands
        {
            [Command("permissions")]
            [Description("Get this user's permissions.")]
            public async Task Permissions(IUserMessage msg, [Remainder]IUser user = null)
            {
                var u = user as IGuildUser ?? msg.Author as IGuildUser;
                var g = u.GuildPermissions;

                var infomsg = new List<string>
                {
                    "```xl",
                    $"  Administrator: {Convert.ToInt32(g.Administrator)}",
                    "```"
                };

                await msg.Channel.SendMessageAsync(string.Join("\n", infomsg));
            }
        }
    }
}
