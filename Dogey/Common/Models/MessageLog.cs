using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Models
{
    [Table("messagelogs")]
    public class MessageLog
    {
        [Key, Column("Id")]
        public int Id { get; set; }

        [Column("Timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [Column("GuildId")]
        public ulong GuildId { get; set; }

        [Column("ChannelId")]
        public ulong ChannelId { get; set; }

        [Column("AuthorId")]
        public ulong AuthorId { get; set; }

        [Column("Content")]
        public string Content { get; set; }

        [Column("Attachment")]
        public string Attachment { get; set; }

        public MessageLog() { }
        public MessageLog(IUserMessage msg)
        {
            Timestamp = msg.CreatedAt.DateTime;
            GuildId = (msg.Channel as IGuildChannel).Guild.Id;
            ChannelId = msg.Channel.Id;
            AuthorId = msg.Author.Id;
            Content = msg.Content;
            Attachment = msg.Attachments.FirstOrDefault()?.Url;
        }
    }
}
