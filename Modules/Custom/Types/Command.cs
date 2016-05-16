using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Custom.Types
{
    public class Command
    {
        public string Text { get; set; }
        public string Type { get; set; }
        public Returnable Returns { get; set; }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedOn { get; set; }

        public Command()
        {
            CreatedOn = DateTime.Now;
        }
    }
}
