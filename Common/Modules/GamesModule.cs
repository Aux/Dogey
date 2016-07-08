using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class GamesModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("stats", cmd =>
            {
                cmd.CreateCommand("")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("**Available Stats:**\n" +
                                                    "`stats` `overwatch` `{battletag}`");
                    });
                cmd.CreateCommand("overwatch")
                    .Alias(new string[] { "ow" })
                    .Description("Get overwatch stats for this user.")
                    .Parameter("user", ParameterType.Required)
                    .Do(async e =>
                    {
                        const string baseUrl = "http://masteroverwatch.com/profile/pc/us/";
                        string battletag = e.Args[0];

                        if (string.IsNullOrEmpty(battletag))
                        {
                            await e.Channel.SendMessage("`~stats` `overwatch` `{battletag}`");
                            return;
                        }

                        string userUrl = baseUrl + battletag.Replace("#", "-");

                        WebClient x = new WebClient();
                        string title = Regex.Match(x.DownloadString(userUrl), @"\<title\b[^>]*\>\s*(?<Title>[\s\S]*?)\</title\>", RegexOptions.IgnoreCase).Groups["Title"].Value;

                        if (title.Contains("Not found."))
                        {
                            await e.Channel.SendMessage($"The battletag `{battletag}` is not valid.");
                            return;
                        }

                        await e.Channel.SendMessage(userUrl);
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "GamesModule", "Loaded.");
        }
    }
}