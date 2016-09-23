using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dogey.Types
{
    public static class CustomVariables
    {
        // Precompiled variables are wrapped %var%, compiled variables are wrapped %{1+1}%.
        public const string
            uname = "%user-name%",
            unick = "%user-nick%",
            udisc = "%user-discrim%",
            uid = "%user-id%",
            ument = "%user-mention%",
            cname = "%channel-name%",
            cid = "%channel-id%",
            cment = "%channel-mention%",
            sname = "%server-name%",
            sid  ="%server-id%";
        
        public static string Format(IUserMessage msg, string text)
        {
            Regex r = new Regex(@"%(.+?)%");
            MatchCollection tags = r.Matches(text);
            var output = new StringBuilder(text);

            var user = msg.Author as IGuildUser;
            var channel = msg.Channel as ITextChannel;
            var guild = channel.Guild;

            foreach(Match tag in tags)
            {
                switch (tag.Value.ToLower())
                {
                    case uname:
                        output = output.Replace(tag.Value, user.Username);
                        continue;
                    case unick:
                        output = output.Replace(tag.Value, user.Nickname ?? user.Username);
                        continue;
                    case udisc:
                        output = output.Replace(tag.Value, user.Discriminator);
                        continue;
                    case uid:
                        output = output.Replace(tag.Value, user.Id.ToString());
                        continue;
                    case ument:
                        output = output.Replace(tag.Value, user.Mention);
                        continue;
                    case cname:
                        output = output.Replace(tag.Value, channel.Name);
                        continue;
                    case cid:
                        output = output.Replace(tag.Value, channel.Id.ToString());
                        continue;
                    case cment:
                        output = output.Replace(tag.Value, channel.Mention);
                        continue;
                    case sname:
                        output = output.Replace(tag.Value, guild.Name);
                        continue;
                    case sid:
                        output = output.Replace(tag.Value, guild.Id.ToString());
                        continue;
                }
            }

            return output.ToString();
        }
    }
}
