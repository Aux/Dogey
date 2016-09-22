using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Tools
{
    public static class Utility
    {
        public static async Task AutoDeleteMsg(IUserMessage msg, int delay = 5000)
        {
            await Task.Delay(5000);
            await msg.DeleteAsync();
        }
    }
}
