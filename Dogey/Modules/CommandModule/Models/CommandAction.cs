using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Commands
{
    public enum CommandAction
    {
        Executed,
        Created,
        Modified,
        Deleted,
        Restored
    }
}
