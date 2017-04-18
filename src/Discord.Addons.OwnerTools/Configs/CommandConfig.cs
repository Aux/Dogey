using Discord.Commands;
using System.Collections.Generic;

namespace Discord.Addons.OwnerTools
{
    public abstract class CommandConfig
    {
        public List<PreconditionAttribute> Preconditions { get; set; } = new List<PreconditionAttribute>();
        public virtual List<string> Aliases { get; set; } = new List<string>();
        public virtual string Summary { get; set; }
        public virtual string Remarks { get; set; }

        public CommandConfig()
        {
            Preconditions.Add(new RequireOwnerAttribute());
        }
    }
}
