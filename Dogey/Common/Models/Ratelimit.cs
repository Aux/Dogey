using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Models
{
    [Table("ratelimits")]
    public class Ratelimit
    {
        [Key, Required, Column("UserId")]
        public ulong UserId { get; set; }

        [Required, Column("Module")]
        public string Module { get; set; }

        [Required, Column("Command")]
        public string Command { get; set; }

        [Required, Column("End")]
        public DateTime End { get; set; }
    }
}
