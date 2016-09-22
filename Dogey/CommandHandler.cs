using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Discord;
using Dogey.Models;
using System.Linq;
using Dogey.Tools;
using Dogey.Extensions;
using Dogey.Enums;

namespace Dogey
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _cmds;

        public async Task Install(DiscordSocketClient c)
        {
            _client = c;
            _cmds = new CommandService();

            var map = new DependencyMap();
            map.Add(_client);
            
            await _cmds.LoadAssembly(Assembly.GetEntryAssembly(), map);
            Globals.CommandService = _cmds;
        }

        public async Task HandleCommand(IUserMessage msg)
        {
            if (msg.Author.IsBot || Globals.Config.Blocked.Contains(msg.Author.Id))
                return;

            var self = await (msg.Channel as IGuildChannel)?.Guild.GetCurrentUserAsync();
            if (self != null)
            {
                var t = msg.Channel as ITextChannel;
                var perms = t.PermissionOverwrites.Where(x => x.TargetId == self.Id || self.Roles.Select(r => r.Id).Contains(x.TargetId));

                if (perms.Any(x => x.Permissions.SendMessages == PermValue.Deny))
                    return;
            }

            if (msg != null)
            {
                var g = (msg.Channel as IGuildChannel)?.Guild;
                
                string prefix;
                if (g != null)
                    prefix = await g.GetCustomPrefixAsync();
                else
                    prefix = Globals.Config.DefaultPrefix;
                
                int argPos = 0;
                if (msg.HasStringPrefix(prefix, ref argPos))
                {
                    var result = await _cmds.Execute(msg, argPos);

                    if (!result.IsSuccess)
                    {
                        if (!result.ErrorReason.Contains("You can use this command again in "))
                        {
                            if (result.Error == CommandError.UnknownCommand)
                                await CustomCommand.Handle(msg);
                            else
                                await msg.Channel.SendMessageAsync(result.ErrorReason);
                        }
                        else
                        {
                            var dm = await (msg.Author as IGuildUser)?.CreateDMChannelAsync();

                            if (dm != null)
                            {
                                await dm.SendMessageAsync(result.ErrorReason);
                                DogeyConsole.Log(msg);
                            }
                        }
                    } else
                    {
                        string[] parts = msg.Content.Replace(prefix, "").Split(new[] { ' ' }, 2);
                        string parameters = null;
                        if (parts.Count() > 1)
                            parameters = parts[1];

                        using (var db = new DataContext())
                        {
                            db.CommandLogs.Add(new CommandLog()
                            {
                                Timestamp = DateTime.UtcNow,
                                GuildId = (msg.Channel as IGuildChannel)?.Guild.Id,
                                ChannelId = msg.Channel.Id,
                                UserId = msg.Author.Id,
                                Command = parts[0],
                                Parameters = parameters,
                                Action = CommandAction.Executed
                            });
                            await db.SaveChangesAsync();
                        }

                        DogeyConsole.Log(msg);
                    }
                } else
                {
                    await msg.SaveMessage();
                }
            }
        }


    }
}