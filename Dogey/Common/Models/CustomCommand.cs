using Discord;
using Dogey.Enums;
using Dogey.Extensions;
using Dogey.Tools;
using Dogey.Types;
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
        [Key, Required, Column("Id")]
        public int Id { get; set; }

        [Required, Column("Name")]
        public string Name { get; set; }

        [Required, Column("Description")]
        public string Description { get; set; }

        [Required, Column("GuildId")]
        public ulong GuildId { get; set; }

        [Column("ChannelId")]
        public ulong? ChannelId { get; set; }

        [Required, Column("OwnerId")]
        public ulong OwnerId { get; set; }

        [Column("Type")]
        public CommandType Type { get; set; } = CommandType.Single;

        #region :T
        [NotMapped, JsonIgnore]
        private List<string> _tags { get; set; }

        [Column("Tags")]
        private string tags
        {
            get { return JsonConvert.SerializeObject(_tags); }
            set { _tags = JsonConvert.DeserializeObject <List<string>>(value); }
        }

        [NotMapped, JsonIgnore]
        private List<string> _msgs { get; set; }

        [Column("Messages")]
        private string msgs
        {
            get { return JsonConvert.SerializeObject(_msgs); }
            set { _msgs = JsonConvert.DeserializeObject<List<string>>(value); }
        }
        #endregion

        [NotMapped]
        public List<string> Tags { get; set; } = new List<string>();

        [NotMapped]
        public List<string> Messages { get; set; } = new List<string>();

        public CustomCommand() { }
        public CustomCommand(IUserMessage msg, string name, bool channelLocked = false)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            
            Name = name;
            GuildId = guild.Id;
            if (channelLocked)
                ChannelId = msg.Channel.Id;
            OwnerId = msg.Author.Id;
        }

        public static async Task Handle(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            var parts = msg.Content.Split(new[] { ' ' }, 2);

            string prefix = await guild.GetCustomPrefixAsync();
            string name = parts[0].Substring(prefix.Count());
            string sub = null;
            if (name.Contains("."))
            {
                sub = name.Split('.')[1];
                name = name.Split('.')[0];
            }
            string parameters = null;
            if (parts.Count() > 1)
                parameters = parts[1];
            
            CustomCommand cmd;
            using (var db = new DataContext())
                cmd = db.Commands.Where(x => x.GuildId == guild.Id && x.Name == name).FirstOrDefault();

            Console.WriteLine($"{prefix} / {name} / {sub} / {parameters}");

            if (cmd != null)
            {
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

        // !cmd <tag>
        private static async Task Base(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            string prefix = await guild.GetCustomPrefixAsync();
            string message = null;

            using (var db = new DataContext())
            {
                db.CommandLogs.Add(new CommandLog()
                {
                    Timestamp = DateTime.UtcNow,
                    GuildId = guild.Id,
                    ChannelId = msg.Channel.Id,
                    UserId = msg.Author.Id,
                    Command = cmd.Name,
                    Action = CommandAction.Executed
                });
                await db.SaveChangesAsync();
            }
            
            if (!string.IsNullOrWhiteSpace(parameters))
            {
                int selected = cmd.Tags.IndexOf(parameters);
                if (selected >= 0)
                    message = $"{cmd.Tags[selected]}: {cmd.Messages[selected]}";
            } else
            {
                switch (cmd.Type)
                {
                    case CommandType.List:
                        message = $"These are the tags for **{cmd.Name}**:\n```{string.Join(", ", cmd.Tags)}```";
                        break;
                    case CommandType.Random:
                        int r = new Random().Next(0, cmd.Tags.Count());
                        message = $"{cmd.Tags[r]}: {cmd.Messages[r]}";
                        break;
                    default:
                        if (cmd.Tags.Count() > 0)
                            message = cmd.Messages.FirstOrDefault();
                        else
                            message = $"This command does not have any tags, add some with `{prefix}{cmd.Name}.add <tag> <message>`.";
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(message))
                await msg.Channel.SendMessageAsync(message);
        }

        // !cmd.add <tag> <remainder>
        private static async Task Add(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            try
            {
                if (msg.Author.Id == cmd.OwnerId)
                {
                    var arr = parameters.Split(new[] { ' ' }, 2);
                    string tag = arr[0];
                    string message = arr[1];

                    var tags = cmd.Tags;
                    tags.Add(tag);
                    var msgs = cmd.Messages;
                    msgs.Add(message);

                    using (var db = new DataContext())
                    {
                        cmd.Tags = tags;
                        cmd.Messages = msgs;

                        db.Commands.Update(cmd);
                        await db.SaveChangesAsync();
                    }

                    await msg.Channel.SendMessageAsync(":thumbsup:");
                }
            }
            catch (Exception ex)
            {
                DogeyConsole.Log(LogSeverity.Info, "[Add]", ex.Message);
            }
        }

        // !cmd.del <tag>
        private static async Task Del(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            try
            {
                if (msg.Author.Id == cmd.OwnerId)
                {
                    int selected = cmd.Tags.IndexOf(parameters);
                    cmd.Tags.RemoveAt(selected);
                    cmd.Messages.RemoveAt(selected);

                    using (var db = new DataContext())
                    {
                        db.Commands.Update(cmd);
                        await db.SaveChangesAsync();
                    }

                    await msg.Channel.SendMessageAsync(":thumbsup:");
                }
            }
            catch (KeyNotFoundException)
            {
                await msg.Channel.SendMessageAsync($"The tag `{parameters}` does not exist in `{cmd.Name}`.");
            }
            catch (Exception ex)
            {
                DogeyConsole.Log(LogSeverity.Info, "[Del]", ex.Message);
            }
        }

        // !cmd.raw <tag>
        private static async Task Raw(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            await Task.Delay(1);
        }

        // !cmd.retag <oldtag> <newtag>
        private static async Task Retag(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            try
            {
                if (msg.Author.Id == cmd.OwnerId)
                {
                    var arr = parameters.Split(' ');
                    string oldtag = arr[0];
                    string newtag = arr[1];

                    int selected = cmd.Tags.IndexOf(oldtag);
                    cmd.Tags[selected] = newtag;

                    using (var db = new DataContext())
                    {
                        db.Commands.Update(cmd);
                        await db.SaveChangesAsync();
                    }

                    await msg.Channel.SendMessageAsync(":thumbsup:");
                }
            }
            catch (IndexOutOfRangeException)
            {
                await msg.Channel.SendMessageAsync($"Invalid parameters.");
            }
            catch (Exception ex)
            {
                DogeyConsole.Log(LogSeverity.Info, "[Retag]", ex.Message);
            }
        }

        // !cmd.mode <mode>
        // Single, List, Random
        private static async Task Mode(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            await Task.Delay(1);
        }
    }
}
