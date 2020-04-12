using System.Collections.Generic;

namespace Dogey.Commands
{
    public class NodeGroup
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Node> Children { get; set; }
    }
}
