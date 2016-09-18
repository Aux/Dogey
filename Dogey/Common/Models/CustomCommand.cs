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
        public ulong ChannelId { get; set; }

        [Required, Column("OwnerId")]
        public ulong OwnerId { get; set; }

        [Column("Type")]
        public CommandType Type { get; set; }

        [JsonIgnore]
        [Column("Messages")]
        private string _messages
        {
            get { return JsonConvert.SerializeObject(Messages); }
            set { Messages = JsonConvert.DeserializeObject<Dictionary<string, string>>(value); }
        }

        [NotMapped]
        public Dictionary<string, string> Messages
        {
            get { return JsonConvert.DeserializeObject<Dictionary<string, string>>(_messages); }
            set { _messages = JsonConvert.SerializeObject(Messages); }
        }
        
        public CustomCommand()
        {
            Type = CommandType.Single;
            Messages = new Dictionary<string, string>();
        }

        public static async Task Handle(IUserMessage msg)
        {
            await Task.Delay(1);
        }

        // !cmd <tag>
        private static async Task Base(IUserMessage msg, CustomCommand cmd, string parameters)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
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
                string selected = cmd.Messages[parameters] ?? null;
                if (string.IsNullOrWhiteSpace(selected))
                    message = $"{parameters}: {selected}";
            } else
            {
                switch (cmd.Type)
                {
                    case CommandType.List:
                        message = $"These are the tags for **{cmd.Name}**:\n```{string.Join(", ", cmd.Messages.Select(x => x.Key))}```";
                        break;
                    case CommandType.Random:
                        int r = new Random().Next(0, cmd.Messages.Count());
                        var selected = cmd.Messages.ElementAt(r);
                        message = $"{selected.Key}: {selected.Value}";
                        break;
                    default:
                        message = cmd.Messages.ElementAt(0).Value;
                        if (string.IsNullOrWhiteSpace(message))
                            message = $"This command does not have any tags, add some with `{guild.GetCustomPrefixAsync()}{cmd.Name}.add <tag> <remainder>`.";
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
                    var arr = parameters.Split(' ');
                    string tag = arr[0];
                    string message = parameters.Substring(tag.Count() + 1);

                    cmd.Messages.Add(tag, message);
                    using (var db = new DataContext())
                    {
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
                    cmd.Messages.Remove(parameters);
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

                    string message = cmd.Messages[oldtag];
                    cmd.Messages.Remove(oldtag);
                    cmd.Messages.Add(newtag, message);

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
