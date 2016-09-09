using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Models
{
    public class Ratelimit
    {
        public ulong UserId { get; set; }
        public string Module { get; set; }
        public string Command { get; set; }
        public DateTime End { get; set; }
    }
}
