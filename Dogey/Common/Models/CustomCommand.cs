using Discord;
using Dogey.Enums;
using Dogey.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Models
{
    [Table("commands")]
    public class CustomCommand
    {
        [Column("Id"), Key, Required]
        public int Id { get; set; }
        [Column("Name")]
        public string Name { get; set; }
        [Column("Description")]
        public string Description { get; set; }

        [Column("GuildId")]
        public ulong GuildId { get; set; }
        [Column("ChannelId")]
        public ulong? ChannelId { get; set; }
        [Column("OwnerId")]
        public ulong OwnerId { get; set; }

        [Column("Type")]
        public CommandType Type { get; set; } = CommandType.Single;
        [Column("Messages")]
        public string Messages { get; set; } = "{}";

        public CustomCommand() { }
        public CustomCommand(IUserMessage msg, string name, string desc = null, bool channelled = false)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;

            Name = name;
            Description = desc;
            GuildId = guild.Id;
            if (channelled) ChannelId = msg.Channel.Id;
            OwnerId = msg.Author.Id;
        }

        #region Commands
        public static async Task Handle(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            string prefix = await guild.GetCustomPrefixAsync();

            string name = null,
                sub = null,
                parameters = null;

            string[] parts = msg.Content.Replace(prefix, "").Split(new[] { ' ' }, 2);
            if (parts[0].Contains('.'))
            {
                var text = parts[0].ToLower().Split('.');
                name = text[0];
                sub = text[1];
            } else
            {
                name = parts[0].ToLower();
            }
            if (parts.Count() > 1)
                parameters = parts[1];

            CustomCommand cmd;
            using (var db = new DataContext())
                cmd = db.Commands.Where(x => x.GuildId == guild.Id && x.Name == name).FirstOrDefault();

            if (cmd != null)
            {
                using (var db = new DataContext())
                {
                    db.CommandLogs.Add(new CommandLog()
                    {
                        Timestamp = DateTime.UtcNow,
                        GuildId = guild.Id,
                        ChannelId = msg.Channel.Id,
                        UserId = msg.Author.Id,
                        Command = parts[0],
                        Parameters = parameters,
                        Action = CommandAction.Executed
                    });
                    await db.SaveChangesAsync();
                }

                switch (sub)
                {
                    case null:
                        await Base(msg, cmd, parameters);
                        break;
                    case "":
                        await Base(msg, cmd, parameters);
                        break;
                    case "add":
                        await Add(msg, cmd, parameters);
                        break;
                    case "del":
                        await Del(msg, cmd, parameters);
                        break;
                    case "raw":
                        await Raw(msg, cmd, parameters);
                        break;
                    case "retag":
                        await Retag(msg, cmd, parameters);
                        break;
                    case "mode":
                        await Mode(msg, cmd, parameters);
                        break;
                }
            }
        }
        
        public static async Task Base(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            string prefix = await guild.GetCustomPrefixAsync();
            var msgs = cmd.GetMessages();

            if (string.IsNullOrWhiteSpace(parameters))
            {
                switch(cmd.Type)
                {
                    default:
                        if (msgs.Count() > 0)
                            await msg.Channel.SendMessageAsync(msgs.FirstOrDefault().Value);
                        else
                            await msg.Channel.SendMessageAsync($"This command does not have any tags, add some with `{prefix}{cmd.Name}.add <tag> <message>`.");
                        break;
                    case CommandType.List:
                        await msg.Channel.SendMessageAsync($"These are the tags for **{cmd.Name}**:\n```{string.Join(", ", msgs.Select(x => x.Key))}```");
                        break;
                    case CommandType.Random:
                        int r = new Random().Next(0, msgs.Count());
                        await msg.Channel.SendMessageAsync($"{msgs.ElementAt(r).Key}: {msgs.ElementAt(r).Value}");
                        break;
                }
            }
            else
            {
                string tag = parameters.ToLower();
                if (msgs.ContainsKey(tag))
                    await msg.Channel.SendMessageAsync($"{tag}: {msgs[tag]}");
                else
                    await msg.Channel.SendMessageAsync($"The command `{cmd.Name}` does not contain the tag `{tag}`.");
            }
        }

        public static async Task Raw(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            await Task.Delay(1);
        }

        public static async Task Add(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            string prefix = await guild.GetCustomPrefixAsync();
            var msgs = cmd.GetMessages();

            var arr = parameters.Split(new[] { ' ' }, 2);
            string tag = arr[0];
            string message = arr[1];

            msgs.Add(tag, message);
            cmd.SetMessages(msgs);

            using (var db = new DataContext())
            {
                db.Commands.Update(cmd);
                await db.SaveChangesAsync();
            }

            await msg.Channel.SendMessageAsync($":thumbsup:");
        }

        public static async Task Del(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            string prefix = await guild.GetCustomPrefixAsync();
            var msgs = cmd.GetMessages();

            msgs.Remove(parameters);
            cmd.SetMessages(msgs);

            using (var db = new DataContext())
            {
                db.Commands.Update(cmd);
                await db.SaveChangesAsync();
            }

            await msg.Channel.SendMessageAsync($":thumbsup:");
        }

        public static async Task Retag(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            string prefix = await guild.GetCustomPrefixAsync();
            var msgs = cmd.GetMessages();

            var arr = parameters.Split(' ');
            string oldtag = arr[0];
            string newtag = arr[1];

            string message = msgs[oldtag];
            msgs.Remove(oldtag);
            msgs.Add(newtag, message);
            cmd.SetMessages(msgs);

            using (var db = new DataContext())
            {
                db.Commands.Update(cmd);
                await db.SaveChangesAsync();
            }

            await msg.Channel.SendMessageAsync($":thumbsup:");
        }

        public static async Task Mode(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            string prefix = await guild.GetCustomPrefixAsync();

            CommandType t = cmd.Type;
            if (parameters == "0" ||
                parameters == "single" ||
                parameters == "s")
                t = CommandType.Single;
            else
            if (parameters == "1" ||
                parameters == "list" ||
                parameters == "l")
                t = CommandType.List;
            else
            if (parameters == "2" ||
                parameters == "random" ||
                parameters == "r")
                t = CommandType.Random;
            else
                return;

            using (var db = new DataContext())
            {
                cmd.Type = t;
                db.Commands.Update(cmd);
                await db.SaveChangesAsync();
            }

            await msg.Channel.SendMessageAsync($"`{cmd.Name}` is now `{Enum.GetName(typeof(CommandType), t)}`");
        }
        #endregion

        #region MessageHandler
        public Dictionary<string, string> GetMessages()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, string>>(Messages);
        }
        public void SetMessages(Dictionary<string, string> msgs)
        {
            Messages = JsonConvert.SerializeObject(msgs);
        }
        public void AddMessage(string tag, string message)
        {
            var msgs = GetMessages();
            msgs.Add(tag, message);
            SetMessages(msgs);
        }
        public void DelMessage(string tag)
        {
            var msgs = GetMessages();
            msgs.Remove(tag);
            SetMessages(msgs);
        }
        #endregion
    }
}
