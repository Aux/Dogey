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
            else if (Before.Status.Value != After.Status.Value)
            {
                if (After.Status.Value == "idle") return null;
                if (After.Status.Value == "online")
                {
                    return $"**STATUS:** *{After.Name}* is now *{After.Status.Value}*";
                }
                else
                {
                    return $"**STATUS:** *{After.Name}* is now *{After.Status.Value}*";
                }
            }
            else if (Before.VoiceChannel != After.VoiceChannel)
            {
                if (After.VoiceChannel == null)
                {
                    return $"**VOICE:** *{After.Name}* has left {Before.VoiceChannel.Name}";
                }
                else if (Before.VoiceChannel == null)
                {
                    return $"**VOICE:** *{After.Name}* has joined {After.VoiceChannel.Name}";
                }
                else
                {
                    return $"**VOICE:** *{After.Name}* has switched from {Before.VoiceChannel.Name} to {After.VoiceChannel.Name}";
                }
            }
            else if (Before.IsServerMuted != After.IsServerSuppressed)
            {
                if (After.IsServerMuted) return $"**VOICE:** *{After.Name}* is now muted.";
                if (!After.IsServerMuted) return $"**VOICE:** *{After.Name}* is no longer muted.";
            }
            else if (Before.IsServerDeafened != After.IsServerDeafened)
            {
                if (After.IsServerDeafened) return $"**VOICE:** *{After.Name}* is now deafened.";
                if (!After.IsServerDeafened) return $"**VOICE:** *{After.Name}* is no longer deafened.";
            }
            return null;
        }
    }
}
