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

        public CustomCommand FromMsg(IMessage msg)
        {
            var channel = (msg.Channel as IGuildChannel) ?? null;

            int index = msg.Content.IndexOf(Globals.Config.Prefix);
            string cmdtext = (index < 0)
                ? msg.Content
                : msg.Content.Remove(index, Globals.Config.Prefix.Length);

            var cmd = cmdtext.Split(' ')[0];

            using (var c = new CommandContext())
            {
                var check = c.Commands.Where(x => x.Name == cmd.Split('.')[0] && x.GuildId == channel.Guild.Id);
                if (check.Count() > 1)
                    throw new Exception("Multile results found.");
                else
                    return check.FirstOrDefault();
            }
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

        public async Task SendMessageAsync(IMessage msg, int? index = null, bool parseTags = true)
        {
            string message;
            if (index != null)
                message = Messages[(int)index];
            else
                message = Messages[new Random().Next(0, Messages.Count() - 1)];

            if (parseTags)
                await msg.Channel.SendMessageAsync(message); // Create tag parser
            else
                await msg.Channel.SendMessageAsync(message);
        }

        public async Task AddMessageAsync(IMessage msg, string content)
        {
            using (var c = new CommandContext())
            {
                var cmd = c.Commands.FirstOrDefault(x => x.Id == Id);
                cmd.Messages.Add(content);

                await c.SaveChangesAsync();
                await msg.Channel.SendMessageAsync($"Added message number **{cmd.Messages.Count()}** to `{Name}`.");
            }
        }

        public async Task DelMessageAsync(IMessage msg, int index)
        {
            using (var c = new CommandContext())
            {
                var cmd = c.Commands.FirstOrDefault(x => x.Id == Id);
                cmd.Messages.RemoveAt(index);

                await c.SaveChangesAsync();
                await msg.Channel.SendMessageAsync($"Removed message number **{index}** from `{Name}`.");
            }
        }

        public async Task SendInfoAsync(IMessage msg, int? index = null)
        {
            var guild = (msg.Channel as IGuildChannel).Guild ?? null;
            var infomsg = new List<string>();

            using (var c = new CommandContext())
            {
                infomsg.AddRange(new string[]
                {
                    "```xl",
                    $"    Command: {Name}",
                    $"   Messages: {Messages.Count()}",
                    $"    Created: {c.Logs.Where(x => x.CommandId == Id && x.Action == CommandAction.Created).FirstOrDefault()?.Timestamp}",
                    $"    Channel: ",
                    $"      Owner: {await guild.GetUserAsync(OwnerId)}",
                    $"       Uses: {c.Logs.Where(x => x.CommandId == Id && x.Action == CommandAction.Executed).Count()}",
                    $"Description: {Description}",
                    "```"
                });
            }

            await msg.Channel.SendMessageAsync(string.Join(Environment.NewLine, infomsg));
        }
    }
}
