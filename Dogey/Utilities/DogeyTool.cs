using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Utilities
{
    public static class DogeyTool
    {
        public static async Task AutoDeleteMsg(IUserMessage msg, int delay)
        {
            await Task.Delay(delay);
            await msg.DeleteAsync();
        }
    }
}
