using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Enums
{
    /// <summary>
    /// Define the action taken when root command is executed.
    /// </summary>
    public enum CommandType
    {
        Single,     // Execute the only subcommand
        List,       // List all subcommands
        Random      // Execute a random subcommand
    }
}
