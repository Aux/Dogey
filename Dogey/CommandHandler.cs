using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Modules.Commands;
using Dogey.Utilities;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class CommandHandler
    {
        private CommandService _commands;
        private DiscordSocketClient _client;
        private ISelfUser _self;

        public async Task Install(DiscordSocketClient c)
        {
            _client = c;
            _commands = new CommandService();
            _self = await _client.GetCurrentUserAsync();
            
            var map = new DependencyMap();
            map.Add(_client);
            map.Add(_self);
            
            await _commands.LoadAssembly(Assembly.GetEntryAssembly(), map);
        }

        public async Task HandleCommand(IMessage msg)
        {
            if (msg.Author.IsBot)
                return;

            int argPos = 0;
            var channel = (msg.Channel as IGuildChannel) ?? null;

            if (msg.HasStringPrefix(Globals.Config.Prefix, ref argPos))
            {
                IResult result = null;
                if (IsCustomCommand(msg.Content))
                    result = await CustomExecute(msg);
                else
                    result = await _commands.Execute(msg, argPos);

                if (result.IsSuccess)
                {
                    using (var c = new CommandContext())
                    {
                        c.Logs.Add(new CustomInfo()
                        {
                            Timestamp = DateTime.UtcNow,
                            GuildId = channel?.Guild?.Id,
                            ChannelId = channel?.Id,
                            UserId = msg.Author.Id,
                            CommandId = (new CustomCommand().FromMsg(msg))?.Id,
                            CommandText = msg.Content,
                            Action = CommandAction.Executed
                        });

                        await c.SaveChangesAsync();
                    }
                }
                else
                {
                    await msg.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }
        
        public bool IsCustomCommand(string msgContent)
        {
            int index = msgContent.IndexOf(Globals.Config.Prefix);
            string cmdtext = (index < 0)
                ? msgContent
                : msgContent.Remove(index, Globals.Config.Prefix.Length);
            
            using (var c = new CommandContext())
                return c.Commands.Any(x => cmdtext.StartsWith(x.Name));
        }

        public async Task<IResult> CustomExecute(IMessage msg)
        {
            try
            {
                var channel = (msg.Channel as IGuildChannel) ?? null;

                int index = msg.Content.IndexOf(Globals.Config.Prefix);
                string cmdtext = (index < 0)
                    ? msg.Content
                    : msg.Content.Remove(index, Globals.Config.Prefix.Length);

                var cmd = cmdtext.Split(' ')[0];

                var custom = new CustomCommand().FromMsg(msg);

                if (cmd.EndsWith(".add"))
                    await custom.AddMessageAsync(msg, cmdtext.Replace(cmd, ""));
                else if (cmd.EndsWith(".del"))
                    await custom.DelMessageAsync(msg, int.Parse(cmdtext.Replace(cmd, "")));
                else if (cmd.EndsWith(".info"))
                    await custom.SendInfoAsync(msg);
                else if (cmd.EndsWith(".raw"))
                    await custom.SendMessageAsync(msg, parseTags: false);
                else if (cmd == custom.Name)
                    await custom.SendMessageAsync(msg);
                
                return new CustomResult()
                {
                    IsSuccess = true
                };
            } catch (Exception ex)
            {
                return new CustomResult()
                {
                    IsSuccess = false,
                    ErrorReason = ex.ToString()
                };
            }
        }
    }
}