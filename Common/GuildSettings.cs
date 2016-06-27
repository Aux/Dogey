using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common
{
    public class GuildSettings
    {
        //General Settings
        public List<ulong> UserWhitelist { get; set; }
        public List<ulong> RoleWhitelist { get; set; }
        public List<ulong> UserBlacklist { get; set; }
        public List<ulong> RoleBlacklist { get; set; }

        //Custom Module Settings
        public bool CustomShowIndex { get; set; }

        //Admin Module Settings
        public int DefaultDuration { get; set; }
    }
}
