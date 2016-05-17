using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Utility
{
    public static class Extensions
    {
        public static Task Reply(this DiscordClient client, CommandEventArgs e, string text)
            => Reply(client, e.User, e.Channel, text);
        public async static Task Reply(this DiscordClient client, User user, Channel channel, string text)
        {
            if (text != null)
            {
                if (!channel.IsPrivate)
                    await channel.SendMessage($"{user.Name}: {text}");
                else
                    await channel.SendMessage(text);
            }
        }
        public static Task Reply<T>(this DiscordClient client, CommandEventArgs e, string prefix, T obj)
            => Reply(client, e.User, e.Channel, prefix, obj != null ? JsonConvert.SerializeObject(obj, Formatting.Indented) : "null");
        public static Task Reply<T>(this DiscordClient client, User user, Channel channel, string prefix, T obj)
            => Reply(client, user, channel, prefix, obj != null ? JsonConvert.SerializeObject(obj, Formatting.Indented) : "null");
        public static Task Reply(this DiscordClient client, CommandEventArgs e, string prefix, string text)
            => Reply(client, e.User, e.Channel, (prefix != null ? $"{Format.Bold(prefix)}:\n" : "\n") + text);
        public static Task Reply(this DiscordClient client, User user, Channel channel, string prefix, string text)
            => Reply(client, user, channel, (prefix != null ? $"{Format.Bold(prefix)}:\n" : "\n") + text);

        public static Task ReplyError(this DiscordClient client, CommandEventArgs e, string text)
            => Reply(client, e.User, e.Channel, "Error: " + text);
        public static Task ReplyError(this DiscordClient client, User user, Channel channel, string text)
            => Reply(client, user, channel, "Error: " + text);
        public static Task ReplyError(this DiscordClient client, CommandEventArgs e, Exception ex)
            => Reply(client, e.User, e.Channel, "Error: " + ex.GetBaseException().Message);
        public static Task ReplyError(this DiscordClient client, User user, Channel channel, Exception ex)
            => Reply(client, user, channel, "Error: " + ex.GetBaseException().Message);
    }

    internal static class InternalExtensions
    {
        public static Task<User[]> FindUsers(this DiscordClient client, CommandEventArgs e, string username, string discriminator)
            => FindUsers(client, e, username, discriminator, false);
        public static async Task<User> FindUser(this DiscordClient client, CommandEventArgs e, string username, string discriminator)
            => (await FindUsers(client, e, username, discriminator, true))?[0];
        public static async Task<User[]> FindUsers(this DiscordClient client, CommandEventArgs e, string username, string discriminator, bool singleTarget)
        {
            IEnumerable<User> users;
            if (discriminator == "")
                users = e.Server.FindUsers(username);
            else
            {
                var user = e.Server.GetUser(username, ushort.Parse(discriminator));
                if (user == null)
                    users = Enumerable.Empty<User>();
                else
                    users = new User[] { user };
            }

            int count = users.Count();
            if (singleTarget)
            {
                if (count == 0)
                {
                    await client.ReplyError(e, "User was not found.");
                    return null;
                }
                else if (count > 1)
                {
                    await client.ReplyError(e, "Multiple users were found with that username.");
                    return null;
                }
            }
            else
            {
                if (count == 0)
                {
                    await client.ReplyError(e, "No user was found.");
                    return null;
                }
            }
            return users.ToArray();
        }
        public static async Task<Channel> FindChannel(this DiscordClient client, CommandEventArgs e, string name, ChannelType type = null)
        {
            var channels = e.Server.FindChannels(name, type);

            int count = channels.Count();
            if (count == 0)
            {
                await client.ReplyError(e, "Channel was not found.");
                return null;
            }
            else if (count > 1)
            {
                await client.ReplyError(e, "Multiple channels were found with that name.");
                return null;
            }
            return channels.FirstOrDefault();
        }

        public static async Task<User> GetUser(this DiscordClient client, CommandEventArgs e, ulong userId)
        {
            var user = e.Server.GetUser(userId);

            if (user == null)
            {
                await client.ReplyError(e, "No user was not found.");
                return null;
            }
            return user;
        }
    }
}