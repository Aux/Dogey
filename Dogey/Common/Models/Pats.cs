using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Models
{
    [Table("pats")]
    public class Pats
    {
        [Key, Required, Column("UserId")]
        public ulong UserId { get; set; }

        [Required, Column("Count")]
        public int Count { get; set; } = 1;
    }
}
