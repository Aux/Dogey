using Discord;
using Dogey.Common.Models;
using Dogey.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;

namespace Dogey
{
    public class Events
    {
        internal static void OnMessageRecieved(object s, MessageEventArgs e)
        {
            if (e.Message.IsMentioningMe()) Console.BackgroundColor = ConsoleColor.DarkBlue;
            if (e.Channel.IsPrivate && e.Server == null)
            {
                DogeyConsole.Append("[PM]", ConsoleColor.DarkMagenta);
            }
            else
            {
                DogeyConsole.Append($"[{e.Server.Name} - {e.Channel.Name}]", ConsoleColor.DarkYellow);
            }

            DogeyConsole.Append($"[{e.User.Name}]", ConsoleColor.Yellow);
            DogeyConsole.Append($" {e.Message.RawText}", ConsoleColor.White);
            
            if (e.Message.Attachments.Count() > 0)
            {
                DogeyConsole.Append($" +{e.Message.Attachments.Count()}", ConsoleColor.Green);
            }

            DogeyConsole.NewLine();
            Console.BackgroundColor = ConsoleColor.Black;
        }

        internal static void JoinedServer(object sender, ServerEventArgs e)
        {
            string serverFolder = $@"servers\{e.Server.Id}";
            Directory.CreateDirectory(Path.Combine(serverFolder, "commands"));
            Directory.CreateDirectory(Path.Combine(serverFolder, "logs"));

            DogeyConsole.Log(LogSeverity.Info, e.Server.Name, "Joined new server.");
        }

        internal static void CommandError(object sender, CommandErrorEventArgs e)
        {
            DogeyConsole.Log(Enum.GetName(typeof(CommandErrorType), e.ErrorType), e.Command.Text, e.Exception.Message);
        }

        internal static void UserJoined(object sender, UserEventArgs e)
        {
            string banFile = $@"bans\{e.User.Id}.ban";
            if (File.Exists(banFile))
            {
                var ban = JsonConvert.DeserializeObject<UserBans>(File.ReadAllText(banFile));
                int totalBans = ban.Bans.Sum(x => x.Value);

                if (totalBans > 1)
                {
                    e.Server.DefaultChannel.SendMessage($"Welcome {e.User.Mention} to the server! They have {totalBans} bans on their account.");
                }
            } else
            {
                e.Server.DefaultChannel.SendMessage($"Welcome {e.User.Mention} to the server!");
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"has joined the server.");
        }
        
        internal static void UserLeft(object sender, UserEventArgs e)
        {

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"has left the server.");
        }

        internal static void UserBannned(object sender, UserEventArgs e)
        {
            UserBans ban;
            string banFile = $@"bans\{e.User.Id}.ban";

            if (!File.Exists(banFile))
            {
                ban = new UserBans()
                {
                    ID = e.User.Id
                };
                ban.Servers.Add(e.Server.Id);

                string json = JsonConvert.SerializeObject(ban);
                File.Create(banFile).Close();
                File.WriteAllText(banFile, json);
            } else
            {
                ban = JsonConvert.DeserializeObject<UserBans>(File.ReadAllText(banFile));

                if (!ban.Servers.Contains(e.Server.Id))
                {
                    ban.Servers.Add(e.Server.Id);
                } else
                {
                    ban.Bans[e.Server.Id]++;
                }
            }

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"[{e.Server.Name}]");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{e.User.Name} ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"was banned from the server. ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($" Ban #{ban.Bans.Sum(x => x.Value)}");
        }
    }
}
