using Discord;
using Dogey.Enums;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Types
{
    public static class CommandBuilder
    {
        private static CustomCommand cmd = new CustomCommand();

        public static CustomCommand Context(IUserMessage msg, bool ChannelLimited = false)
        {
            if (ChannelLimited) cmd.ChannelId = msg.Channel.Id;
            cmd.GuildId = (msg.Channel as IGuildChannel).Guild.Id;
            cmd.OwnerId = msg.Author.Id;
            return cmd;
        }

        public static CustomCommand Name(string name)
        {
            cmd.Name = name;
            return cmd;
        }

        public static CustomCommand Description(string desc)
        {
            cmd.Description = desc;
            return cmd;
        }

        public static CustomCommand Type(CommandType type)
        {
            cmd.Type = type;
            return cmd;
        }

        public static CustomCommand AddMessage(string tag, string content)
        {
            cmd.Messages.Add(tag, content);
            return cmd;
        }

        public static void Save()
        {
            using (var db = new DataContext())
            {
                db.Commands.Add(cmd);
                db.SaveChanges();
            }
        }
    }
}
