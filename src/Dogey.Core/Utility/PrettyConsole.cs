using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public static class PrettyConsole
    {
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

        public static void Log(object severity, string source, string message)
        {
            PrettyConsole.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            PrettyConsole.Append($"[{severity}] ", ConsoleColor.Red);
            PrettyConsole.Append($"{source}: ", ConsoleColor.DarkGreen);
            PrettyConsole.Append(message, ConsoleColor.White);
        }

        public static Task LogAsync(object severity, string source, string message)
        {
            PrettyConsole.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.DarkGray);
            PrettyConsole.Append($"[{severity}] ", ConsoleColor.Red);
            PrettyConsole.Append($"{source}: ", ConsoleColor.DarkGreen);
            PrettyConsole.Append(message, ConsoleColor.White);
            return Task.CompletedTask;
        }

        public static void Log(IUserMessage msg)
        {
            var channel = (msg.Channel as IGuildChannel);
            PrettyConsole.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.Gray);

            if (channel?.Guild == null)
                PrettyConsole.Append($"[PM] ", ConsoleColor.Magenta);
            else
                PrettyConsole.Append($"[{channel.Guild.Name} #{channel.Name}] ", ConsoleColor.DarkGreen);

            PrettyConsole.Append($"{msg.Author}: ", ConsoleColor.Green);
            PrettyConsole.Append(msg.Content, ConsoleColor.White);
        }

        public static void Log(CommandContext c)
        {
            var channel = (c.Channel as SocketGuildChannel);
            PrettyConsole.NewLine($"{DateTime.Now.ToString("hh:mm:ss")} ", ConsoleColor.Gray);

            if (channel == null)
                PrettyConsole.Append($"[PM] ", ConsoleColor.Magenta);
            else
                PrettyConsole.Append($"[{c.Guild.Name} #{channel.Name}] ", ConsoleColor.DarkGreen);

            PrettyConsole.Append($"{c.User}: ", ConsoleColor.Green);
            PrettyConsole.Append(c.Message.Content, ConsoleColor.White);
        }
    }
}