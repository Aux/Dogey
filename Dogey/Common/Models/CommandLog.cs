using Dogey.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Models
{
    [Table("commandlogs")]
    public class CommandLog
    {
        [Key, Required, Column("Id")]
        public int Id { get; set; }

        [Required, Column("Timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Column("GuildId")]
        public ulong? GuildId { get; set; }

        [Column("ChannelId")]
        public ulong ChannelId { get; set; }

        [Column("UserId")]
        public ulong UserId { get; set; }

        [Column("Command")]
        public string Command { get; set; }

        [Column("Parameters")]
        public string Parameters { get; set; }

        [Column("Action")]
        public CommandAction Action { get; set; }

        public void CreateTable()
        {

        }
    }
}
