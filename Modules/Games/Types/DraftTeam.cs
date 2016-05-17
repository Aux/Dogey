using Discord;
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
        public Color Color { get; set; }
        public User Captain { get; set; }
        public List<User> Players { get; set; }
        public Role Role { get; set; }

        public DraftTeam()
        {
            
        }
    }
}
