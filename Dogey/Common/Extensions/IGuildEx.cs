using Discord;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Extensions
{
    public static class IGuildEx
    {
        public static async Task EnsureSettingsExist(this IGuild guild)
        {
            using (var db = new DataContext())
            {
                var settings = db.Settings.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                if (settings == null)
                {
                    settings = new GuildSettings(guild.Id);
                    db.Settings.Add(settings);
                    await db.SaveChangesAsync();
                }
            }
        }

        public static async Task<string> GetCustomPrefixAsync(this IGuild guild)
        {
            await EnsureSettingsExist(guild);
            using (var db = new DataContext())
            {
                var settings = db.Settings.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                return settings.Prefix;
            }
        }

        public static async Task<ITextChannel> GetLogChannelAsync(this IGuild guild)
        {
            await EnsureSettingsExist(guild);
            using (var db = new DataContext())
            {
                var settings = db.Settings.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                var log = await guild.GetTextChannelAsync(settings.LogChannelId);
                return log;
            }
        }

        public static async Task<ITextChannel> GetStarChannelAsync(this IGuild guild)
        {
            await EnsureSettingsExist(guild);
            using (var db = new DataContext())
            {
                var settings = db.Settings.Where(x => x.GuildId == guild.Id).FirstOrDefault();
                var star = await guild.GetTextChannelAsync(settings.StarChannelId);
                return star;
            }
        }

        public static IEnumerable<CustomCommand> GetCustomCommands(this IGuild guild)
        {
            using (var db = new DataContext())
                return db.Commands.Where(x => x.GuildId == guild.Id);
        }

        public static async Task AddGuildLog(this IGuild guild, GuildLog log)
        {
            using (var db = new DataContext())
            {
                db.GuildLogs.Add(log);
                await db.SaveChangesAsync();
            }
        }
    }
}
