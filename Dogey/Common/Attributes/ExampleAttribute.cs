using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ExampleAttribute : SummaryAttribute
    {
        public new string Text;

        public ExampleAttribute(string text) : base(text)
        {
            Text = text;
        }
    }
}
