using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Commands
{
    [Table("commands")]
    public class CustomCommand
    {
        /// <summary> The command's unique identifier. </summary>
        [Key, Required, Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary> The command owner's user id. </summary>
        [Required, Column("OwnerId")]
        public ulong OwnerId { get; set; }

        /// <summary> The command server's id. </summary>
        [Required, Column("GuildId")]
        public ulong GuildId { get; set; }

        /// <summary> The command channel's id, null if not bound to channel. </summary>
        [Column("ChannelId")]
        public ulong? ChannelId { get; set; }

        /// <summary> This command's name. </summary>
        [Required, Column("Name")]
        [MaxLength(16, ErrorMessage = "Command name must be less than 16 characters")]
        public string Name { get; set; }

        /// <summary> A description of what this command does. </summary>
        [Column("Description")]
        [MaxLength(128, ErrorMessage = "Command description cannot be longer than 128 characters.")]
        public string Description { get; set; }

        /// <summary> The command's unique identifier. </summary>
        [JsonIgnore]
        [Column("Messages")]
        public string MessageString
        {
            get { return JsonConvert.SerializeObject(_messages); }
            set { _messages = JsonConvert.DeserializeObject<List<string>>(value); }
        }

        [NotMapped]
        private List<string> _messages { get; set; }

        /// <summary> The command's unique identifier. </summary>
        [NotMapped]
        public List<string> Messages
        {
            get { return _messages; }
            set { _messages = value; }
        }
        
        public CustomCommand()
        {
            Messages = new List<string>();
        }

        /// <summary> Save this command to the database. </summary>
        public async Task CreateAsync(IMessage msg)
        {
            var channel = (msg.Channel as IGuildChannel) ?? null;

            using (var c = new CommandContext())
            {
                c.Commands.Add(this);

                c.Logs.Add(new CustomInfo()
                {
                    Timestamp = DateTime.UtcNow,
                    GuildId = channel?.Guild?.Id,
                    ChannelId = channel?.Id,
                    UserId = msg.Author.Id,
                    CommandId = c.Commands.LastOrDefault().Id + 1,
                    Action = CommandAction.Created
                });

                await c.SaveChangesAsync();
            }
        }

        /// <summary> Remove this command from the database. </summary>
        public async Task DeleteAsync(IMessage msg)
        {
            var channel = (msg.Channel as IGuildChannel) ?? null;

            using (var c = new CommandContext())
            {
                c.Commands.Remove(this);

                c.Logs.Add(new CustomInfo()
                {
                    Timestamp = DateTime.UtcNow,
                    GuildId = channel?.Guild?.Id,
                    ChannelId = channel?.Id,
                    UserId = msg.Author.Id,
                    CommandId = this.Id,
                    Action = CommandAction.Created
                });

                await c.SaveChangesAsync();
            }
        }

        public string GetMessage(int? index = null)
        {
            if (index != null)
            {
                return Messages[(int)index];
            } else
            {
                var r = new Random();
                return Messages[r.Next(0, Messages.Count() - 1)];
            }
        }

        public async Task<string> GetMessageRaw(int index)
        {
            await Task.Delay(1);
            return null;
        }

        public async Task<string> GetInfo(int? index = null)
        {
            await Task.Delay(1);
            return null;
        }
    }
}
