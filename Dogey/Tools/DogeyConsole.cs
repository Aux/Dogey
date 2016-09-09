using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Tools
{
    public static class DogeyConsole
    {
        public static void TitleCard(string title, string version = null, ConsoleColor? color = null)
        {
            if (color == null)
                color = ConsoleColor.Yellow;

            var card = new List<string>();
            card.Add($"┌{new string('─', 12)}{new string('─', title.Count())}{new string('─', 12)}┐");
            card.Add($"│{new string(' ', 12)}{title}{new string(' ', 12)}│");
            if (version != null)
            {
                int diff = title.Count() - version.Count() / 2;

                if (diff > 0)
                    card.Add($"│{new string(' ', 12 + diff)}{version}{new string(' ', 12 + diff)}│");
            }
            card.Add($"└{new string('─', 12)}{new string('─', title.Count())}{new string('─', 12)}┘");

            Console.Title = title;
            DogeyConsole.NewLine(string.Join(Environment.NewLine, card));
        }

        /// <summary> Write a string to the console on an existing line. </summary>
        /// <param name="text">String written to the console.</param>
        /// <param name="foreground">The text color in the console.</param>
        /// <param name="background">The background color in the console.</param>
        public static void Append(string text, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            Console.Write(text);
        }

        /// <summary> Write a string to the console on an new line. </summary>
        /// <param name="text">String written to the console.</param>
        /// <param name="foreground">The text color in the console.</param>
        /// <param name="background">The background color in the console.</param>
        public static void NewLine(string text = "", ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground == null)
                foreground = ConsoleColor.White;
            if (background == null)
                background = ConsoleColor.Black;

            Console.ForegroundColor = (ConsoleColor)foreground;
            Console.BackgroundColor = (ConsoleColor)background;
            Console.Write(Environment.NewLine + text);
        }

        public static void Log(LogSeverity severity, string source, string message)
        {
            DogeyConsole.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            DogeyConsole.Append($"[{severity}] ", ConsoleColor.Red);
            DogeyConsole.Append($"{source}: ", ConsoleColor.DarkGreen);
            DogeyConsole.Append(message, ConsoleColor.White);
        }

        public static void Log(IUserMessage msg)
        {
            var channel = (msg.Channel as IGuildChannel);
            DogeyConsole.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.Gray);

            if (channel?.Guild == null)
                DogeyConsole.Append($"[PM] ", ConsoleColor.Magenta);
            else
                DogeyConsole.Append($"[{channel.Guild.Name} #{channel.Name}] ", ConsoleColor.DarkGreen);

            DogeyConsole.Append($"{msg.Author}: ", ConsoleColor.Green);
            DogeyConsole.Append(msg.Content, ConsoleColor.White);
        }

        /// <summary> Reset the console's colors to default. </summary>
        public static void ResetColors()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
