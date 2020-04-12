using System.Linq;

namespace Dogey.Commands
{
    public struct Node
    {
        public string RawValue { get; }
        public string[] Items { get; }
        public bool IsWildcard { get; }
        public bool IsNegator { get; }
        public bool IsGlobal { get; }

        public Node(string value)
        {
            RawValue = value;
            Items = RawValue.Split('.');
            IsWildcard = value.EndsWith("*");
            IsNegator = value.StartsWith("-");
            IsGlobal = IsNegator ? value.Remove(1) == "*" : value == "*";
        }

        public override string ToString() => RawValue;
    }
}
