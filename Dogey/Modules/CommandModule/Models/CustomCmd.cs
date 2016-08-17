using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Commands
{
    public class CustomCommand
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ulong Owner { get; set; }
        public ulong Server { get; set; }
        public ulong Channel { get; set; }
        public List<string> Messages { get; set; }
    }
}
