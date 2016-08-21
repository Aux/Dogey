using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Commands
{
    [Table("commandlogs")]
    public class CustomInfo
    {
        /// <summary> The log's unique identifier. </summary>
        [Key, Required, Column("Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Timestamp, Required, Column("Timestamp")]
        public DateTime Timestamp { get; set; }
        
        [Column("GuildId")]
        public ulong? GuildId { get; set; }
        
        [Column("ChannelId")]
        public ulong? ChannelId { get; set; }
        
        [Required, Column("UserId")]
        public ulong UserId { get; set; }
        
        [Column("CommandId")]
        public int CommandId { get; set; }

        [Column("CommandText")]
        public string CommandText { get; set; }
        
        [Required, Column("Action")]
        public CommandAction Action { get; set; }
        
        [Column("Changes")]
        public string Changes { get; set; }
    }
}
