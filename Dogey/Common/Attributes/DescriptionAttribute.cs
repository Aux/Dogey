using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Commands
{
    /// <summary>
    /// Because SOMEONE felt like it was necessary to remove Descriptions >:(
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class DescriptionAttribute : RemarksAttribute
    {
        public new string Text;

        public DescriptionAttribute(string text) : base(text)
        {
            Text = text;
        }
    }
}
