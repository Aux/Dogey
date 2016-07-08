using Discord;
using Dogey.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Utility
{
    public class Background
    {
        public static bool _cancelRotateAvatars = false;

        public static async Task RotateAvatars(DiscordClient s, Configuration c)
        {
            const string fileLoc = "config\\avatars";
            DateTime lastCheck = DateTime.Now;
            var _config = c;
            var _dogey = s;

            while (!_cancelRotateAvatars)
            {
                if (!Directory.Exists(fileLoc) || Directory.GetFiles(fileLoc).Length < 2)
                {
                    _cancelRotateAvatars = true;
                    return;
                }

                if (lastCheck.DayOfWeek != DateTime.Now.DayOfWeek)
                {
                    var avatars = Directory.GetFiles(fileLoc);
                    string chosen = _config.Avatar;

                    while (chosen == _config.Avatar)
                    {
                        var r = new Random();
                        chosen = avatars[r.Next(0, avatars.Length)];
                    }

                    await _dogey.CurrentUser.Edit(avatar: File.Open(chosen, FileMode.Open));
                    _config.Avatar = Path.GetFileName(chosen);
                    _config.ToFile("config\\configuration.json");
                    DogeyConsole.Log(LogSeverity.Info, "Settings", $"Updated avatar to {Path.GetFileName(chosen)}");
                }

                lastCheck = DateTime.Now;
                await Task.Delay(36000);
            }
        }
    }
}
