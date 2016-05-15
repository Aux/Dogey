using Discord;
using Dogey.Modules.Chatlog;
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
        internal static void OnJoinedServer(object sender, ServerEventArgs e)
        {
            if (Program.config.ChatlogModule)
            {
                Log.Exists(e.Server);
            }
            if (Program.config.CustomModule)
            {

            }
        }

        internal static void OnUserUpdated(object sender, UserUpdatedEventArgs e)
        {
            Channel activity = null;
            foreach (var channel in e.Server.TextChannels)
            {
                if (channel.Name == "activity")
                {
                    activity = channel;
                    continue;
                }
            }

            if (activity != null)
            {
                string message = Messages.Activity(e.Before, e.After);
                if (!string.IsNullOrEmpty(message)) activity.SendMessage(message);
            }
        }

        internal static void OnProfileUpdated(object sender, ProfileUpdatedEventArgs e)
        {

        }

        internal static void OnMessageRecieved(object s, MessageEventArgs e)
        {
            if (e.Server != null) DogeyConsole.Append($"[{e.Server.Name}]", ConsoleColor.Gray);
            DogeyConsole.Append($"[{e.Channel.Name}]", ConsoleColor.Gray);
            DogeyConsole.Append($" {e.User.Name}: ", ConsoleColor.Yellow);

            if (e.Message.Attachments.Count() != 0)
            {
                DogeyConsole.Append("[attachment]\n", ConsoleColor.White);
                return;
            }
            if (e.Message.IsTTS)
            {
                DogeyConsole.Append($"TTS:= {e.Message.RawText}\n", ConsoleColor.White);
                return;
            }
            DogeyConsole.Append(e.Message.RawText + "\n", ConsoleColor.White);
        }
    }
}
