using Discord;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Extensions
{
    public static class IUserEx
    {
        public static bool IsRateLimited(this IUser user)
        {
            using (var db = new DataContext())
            {
                
            }


            return false;
        }
    }
}
