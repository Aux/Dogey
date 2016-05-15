using Discord;
using Discord.Audio;
using Discord.Commands;
using Discord.Modules;
using Dogey.Config;
using Dogey.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey
{
    class Program
    {
        public static DiscordClient _dogey { get; set; }

        public static Configuration config = null;


        static void Main(string[] args)
        {
            Console.Title = "Dogey";
            Startup.DogeyBanner();

            if (!Startup.ConfigExists())
            {
                Startup.CreateConfig();
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                return;
            }

            config = new Configuration().FromFile("config\\configuration.json");
            _dogey = new DiscordClient(x =>
            {
                x.AppName = config.Discord.Username;
                x.AppUrl = "https://github.com/Auxes/Dogey";
                x.MessageCacheSize = 0;
                x.UsePermissionsCache = true;
                x.EnablePreUpdateEvents = true;
                x.LogLevel = LogSeverity.Debug;
            })
            .UsingCommands(x =>
            {
                x.AllowMentionPrefix = config.AllowMentionCommands;
                x.HelpMode = HelpMode.Public;
                x.PrefixChar = config.DefaultPrefix;
            })
            .UsingAudio(x =>
            {
                x.Mode = AudioMode.Outgoing;
                x.EnableMultiserver = true;
                x.EnableEncryption = true;
                x.Bitrate = AudioServiceConfig.MaxBitrate;
                x.BufferLength = 10000;
            })
            .UsingModules();

            _dogey.MessageReceived += Events.OnMessageRecieved;
            _dogey.ProfileUpdated += Events.OnProfileUpdated;
            _dogey.UserUpdated += Events.OnUserUpdated;
            _dogey.JoinedServer += Events.OnJoinedServer;
            
            _dogey.AddModule<Modules.Chatlog.Initialize>("Chatlog", ModuleFilter.None);
            _dogey.AddModule<Modules.Bot.Initialize>("Bot", ModuleFilter.None);

            _dogey.ExecuteAndWait(async () =>
            {
                while (true)
                {
                    try {
                        if (string.IsNullOrEmpty(config.Discord.Token))
                        {
                            await _dogey.Connect(config.Discord.Email, config.Discord.Password);
                            DogeyConsole.Write($"Connected to Discord using {config.Discord.Email}\n");
                            break;
                        } else
                        {
                            await _dogey.Connect(config.Discord.Token);
                            DogeyConsole.Write("Connected to Discord using bot token.\n");
                            break;
                        }
                    } catch (Exception ex)
                    {
                        _dogey.Log.Error($"Login Failed", ex);
                        DogeyConsole.Write(ex.ToString());
                        await Task.Delay(_dogey.Config.FailedReconnectDelay);
                    }
                }
            });
        }

    }
}
