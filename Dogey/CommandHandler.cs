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
            var channel = (msg.Channel as IGuildChannel) ?? null;

            int index = msg.Content.IndexOf(Globals.Config.Prefix);
            string cmdtext = (index < 0)
                ? msg.Content
                : msg.Content.Remove(index, Globals.Config.Prefix.Length);

            var cmd = cmdtext.Split(' ')[0].Split('.')[0];

            CustomCommand custom;
            using (var c = new CommandContext())
            {
                var check = c.Commands.Where(x => x.Name == cmd && x.GuildId == channel.Guild.Id);
                if (check.Count() > 1)
                    return new CustomResult()
                    {
                        IsSuccess = false,
                        Error = CommandError.MultipleMatches,
                        ErrorReason = $"The command `{cmd}` returned multiple matches."
                    };
                else
                    custom = check.FirstOrDefault();
            }

            if (cmd == custom.Name)
            {

            } else
            if (cmd.EndsWith(".add"))
            {

            } else
            if (cmd.EndsWith(".del"))
            {

            } else
            if (cmd.EndsWith(".info"))
            {

            } else
            if (cmd.EndsWith(".raw"))
            {

            }

                await Task.Delay(1);
            return new CustomResult()
            {
                IsSuccess = false,
                ErrorReason = "This is a custom command."
            };
        }
    }
}