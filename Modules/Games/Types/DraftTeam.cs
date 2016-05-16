using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Games.Types
{
    public class DraftTeam
    {
        public string Name { get; set; }
        public DraftColor Color { get; set; }
        public List<Discord.User> Players { get; set; }

    }
}
