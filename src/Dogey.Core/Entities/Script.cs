using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Dogey
{
    public class Script
    {
        public ulong Id { get; private set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string Content { get; set; }
        public ulong OwnerId { get; set; }
        public string Names
        {
            get { return JsonConvert.SerializeObject(Aliases); }
            set { Aliases = JsonConvert.DeserializeObject<List<string>>(value); }
        }
        
        public List<string> Aliases { get; set; }
    }
}
