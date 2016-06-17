using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Models
{
    public class CustomCommand
    {
        public string Name { get; set; }
        public List<string> Messages { get; set; }

        public ulong CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public CustomCommand()
        {
            Messages = new List<string>();
        }
    }
}
