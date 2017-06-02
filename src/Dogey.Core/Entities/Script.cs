using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dogey
{
    public class Script : IEntity<ulong>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public ulong Id { get; private set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        [NotMapped]
        public List<string> Aliases { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public ulong OwnerId { get; set; }

        [Required]
        public string Names
        {
            get
            {
                return JsonConvert.SerializeObject(Aliases);
            }
            set
            {
                Aliases = JsonConvert.DeserializeObject<List<string>>(value);
            }
        }
    }
}
