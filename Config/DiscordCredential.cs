using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Config
{
    public class DiscordCredential
    {
        /// <summary>
        /// Your Discord bot connection Token, has priority over Email/Password combination.
        /// </summary>
        public string Token { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// Change Dogey's avatar on startup, can be either a URL or a FileLocation.
        /// </summary>
        public string AvatarURL { get; set; }
    }
}
