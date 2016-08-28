using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Commands
{
    [Module, Name("Commands")]
    [RequireContext(ContextType.Guild)]
    public class CommandModule
    {
        private DiscordSocketClient _client;

        public CommandModule(DiscordSocketClient client)
        {
            _client = client;
        }
        
        [Command("commands")]
        public async Task Commands(IUserMessage msg)
        {
            var channel = (msg.Channel as IGuildChannel) ?? null;

            if (channel != null)
            {
                var infomsg = new List<string>();
                using (var c = new CommandContext())
                {
                    infomsg.AddRange(new string[]
                    {
                        "```xl",
                        $"Server: {string.Join(", ", c.Commands.Where(x => x.GuildId == channel.Guild.Id).Select(x => x.Name))}",
                        "```"
                    });
                }
                await msg.Channel.SendMessageAsync(string.Join("\n", infomsg));
            }
        }

        [Command("create")]
        public async Task Create(IUserMessage msg, string name)
        {
            try
            {
                var channel = (msg.Channel as IGuildChannel) ?? null;

                if (channel != null)
                {
                    var cmd = new CustomCommand()
                    {
                        OwnerId = msg.Author.Id,
                        GuildId = channel.Guild.Id,
                        Name = name,
                        Messages = new Dictionary<string, string>()
                    };

                    await cmd.CreateAsync(msg);
                    await msg.Channel.SendMessageAsync($"Created `{name}`.");
                }
            } catch (Exception ex)
            {
                DogeyConsole.Log(LogSeverity.Debug, "[]", ex.ToString());
            }
        }

        [Command("delete")]
        public async Task Delete(IUserMessage msg)
        {
            await Task.Delay(1);
        }

        [Command("*.add")]
        [Description("Add a new mesage to the command.")]
        public async Task Add(IUserMessage msg, [Remainder]string message)
        {
            await Task.Delay(1);
        }
        [Command("*.del")]
        [Description("Remove a message from the command.")]
        public async Task Del(IUserMessage msg, string tag)
        {
            await Task.Delay(1);
        }
        [Command("*.desc")]
        [Description("Set a description for this command.")]
        public async Task Desc(IUserMessage msg, [Remainder]string description)
        {
            await Task.Delay(1);
        }
        [Command("*.addtag")]
        [Description("Add a new message and specify a tag.")]
        public async Task Addtag(IUserMessage msg, string tag, [Remainder]string message)
        {
            await Task.Delay(1);
        }
        [Command("*.retag")]
        [Description("Change the tag of an existing message.")]
        public async Task Settag(IUserMessage msg, string before, string after)
        {
            await Task.Delay(1);
        }
        [Command("*.rename")]
        [Description("Rename this command.")]
        public async Task Rename(IUserMessage msg, string name)
        {
            await Task.Delay(1);
        }
        [Command("*.info")]
        [Description("Get information about this command or command tag.")]
        public async Task Info(IUserMessage msg, string tag = null)
        {
            await Task.Delay(1);
        }
        [Command("*.raw")]
        [Description("View a message without parsed variables.")]
        public async Task Raw(IUserMessage msg, string tag)
        {
            await Task.Delay(1);
        }
    }
}
