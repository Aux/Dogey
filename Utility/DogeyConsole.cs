using Discord;
using Discord.Commands;
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
        /// <param name="fore">The color of the message text.</param>
        public static void Append(string text, ConsoleColor? fore = null, ConsoleColor? back = null)
        {
            if (back == null) back = ConsoleColor.Black;
            if (fore == null) fore = ConsoleColor.White;
            Console.ForegroundColor = (ConsoleColor)fore;
            Console.BackgroundColor = (ConsoleColor)back;
            Console.Write(text, fore);
        }

        /// <summary>
        /// Write text to a new line in the console, with optional color.
        /// </summary>
        /// <param name="text">The message text to be displayed.</param>
        /// <param name="fore">The color of the message text.</param>
        public static void Write(string text, ConsoleColor? fore = null, ConsoleColor? back = null)
        {
            if (back == null) back = ConsoleColor.Black;
            if (fore == null) fore = ConsoleColor.White;
            Console.ForegroundColor = (ConsoleColor)fore;
            Console.BackgroundColor = (ConsoleColor)back;
            Console.WriteLine(text);
        }

        public static void NewLine()
        {
            Console.WriteLine();
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

        public static void Log(string error, string source, string message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write($"[{error}] ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write($"{source}: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{message}\n");
        }
    }
}
