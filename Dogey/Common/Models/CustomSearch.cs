using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Common.Models
{
    [Table("customsearch")]
    public class CustomSearch
    {
        [Column("Id"), Required, Key]
        public int Id { get; set; }
        [Column("UserId")]
        public ulong UserId { get; set; }

        [Column("Name")]
        public string Name { get; set; }
        [Column("Description")]
        public string Description { get; set; }
        [Column("SearchId")]
        public string SearchId { get; set; }
    }
}
