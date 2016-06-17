using Discord;
using Dogey.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey
{
    public class Events
    {
        internal static void OnMessageRecieved(object s, MessageEventArgs e)
        {
            if (e.Message.IsMentioningMe()) Console.BackgroundColor = ConsoleColor.DarkBlue;
            if (e.Channel.IsPrivate && e.Server == null)
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write("[PM]");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write($"[{e.Server.Name} - {e.Channel.Name}]");
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"[{e.User.Name}]");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" {e.Message.RawText}");
            if (e.Message.Attachments.Count() > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($" +{e.Message.Attachments.Count()}");
            }
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
