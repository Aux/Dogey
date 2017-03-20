using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dogey.MySQL
{
    public class MyTag : MyEntity<long>, ITag<long, JsonObject<List<string>>>
    {
        [Required, Timestamp]
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [Required]
        public JsonObject<List<string>> Aliases { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public long OwnerId { get; set; }
        [Required]
        public long GuildId { get; set; }
    }
}
