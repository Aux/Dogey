using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Utility
{
    public class DogeyConsole
    {
        /// <summary>
        /// Append text to an existing line in the console, with optional color.
        /// </summary>
        /// <param name="text">The message text to be displayed.</param>
        /// <param name="color">The color of the message text.</param>
        public static void Append(string text, ConsoleColor? color = null)
        {
            if (color == null) color = ConsoleColor.White;
            Console.ForegroundColor = (ConsoleColor)color;
            Console.Write(text, color);
        }

        /// <summary>
        /// Write text to a new line in the console, with optional color.
        /// </summary>
        /// <param name="text">The message text to be displayed.</param>
        /// <param name="color">The color of the message text.</param>
        public static void Write(string text, ConsoleColor? color = null)
        {
            if (color == null) color = ConsoleColor.White;
            Console.ForegroundColor = (ConsoleColor)color;
            Console.WriteLine(text);
        }

        public static void Log(LogSeverity severity, string source, string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write($"[{severity}] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"{source}: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{message}\n");
        }
    }
}
