using Discord;
using Dogey.Enums;
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



        public static class Builder
        {
            private static CustomCommand cmd = new CustomCommand();

            public static CustomCommand Context(IUserMessage msg, bool ChannelLimited = false)
            {
                if (ChannelLimited) cmd.ChannelId = msg.Channel.Id;
                cmd.GuildId = (msg.Channel as IGuildChannel).Guild.Id;
                cmd.OwnerId = msg.Author.Id;
                return cmd;
            }

            public static CustomCommand Name(string name)
            {
                cmd.Name = name;
                return cmd;
            }

            public static CustomCommand Description(string desc)
            {
                cmd.Description = desc;
                return cmd;
            }

            public static CustomCommand Type(CommandType type)
            {
                cmd.Type = type;
                return cmd;
            }
            
            public static CustomCommand AddMessage(string tag, string content)
            {
                cmd.Messages.Add(tag, content);
                return cmd;
            }

            public static void Save()
            {
                using (var db = new DataContext())
                {
                    db.Commands.Add(cmd);
                    db.SaveChanges();
                }
            }
        }
    }
}
