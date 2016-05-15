using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Utility
{
    public class Messages
    {
        public static string Activity(User Before, User After)
        {
            if (Before.Name != After.Name)
            {
                return $"**RENAME:** *{Before.Name}* is now known as *{After.Name}*";
            }
            else
                if (Before.Status.Value != After.Status.Value)
            {
                if (After.Status.Value == "online")
                {
                    return $"**STATUS:** *{After.Name}* is now *{After.Status.Value}*";
                }
                else
                {
                    return $"**STATUS:** *{After.Name}* is now *{After.Status.Value}*";
                }
            }
            else
                if (Before.VoiceChannel != After.VoiceChannel)
            {
                if (After.VoiceChannel == null)
                {
                    return $"**VOICE:** *{After.Name}* has left {Before.VoiceChannel.Name}";
                }
                else
                {
                    return $"**VOICE:** *{After.Name}* has joined {After.VoiceChannel.Name}";
                }
            }
            return null;
        }
    }
}
