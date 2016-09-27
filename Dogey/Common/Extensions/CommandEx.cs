using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Extensions
{
    public static class CommandEx
    {
        public static void Markdownify(this Command cmd, string parameters)
        {
            string dir = $@"C:\Users\justin.gentry\Desktop\New folder\{cmd.Module.Name}_.md";

            string module = "---\n" +
                            $"title: {cmd.Module.Name}\n" +
                            $"excerpt: {cmd.Module.Remarks}\n" +
                            $"permalink: /dogey/{cmd.Module.Name}/\n" +
                            "---\n\n{% include toc title=\"Contents\" icon=\"file-text\" %}\n\n";
            string command = $"\n\n### {cmd.Name}\n" + 
                            $"**Aliases:** {string.Join(", ", cmd.Aliases)}\n" +      
                            $"**Parameters:** `{parameters}\n" +     
                            $"**Example:** `{cmd.Summary}`\n" +    
                            $"{cmd.Remarks}";       

            if (File.Exists(dir))
            {
                File.AppendAllText(dir, command);
            } else
            {
                File.WriteAllText(dir, module + command);
            }
        }
    }
}
