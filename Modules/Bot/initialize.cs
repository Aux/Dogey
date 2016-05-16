using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Bot
{
    public class Initialize : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("doge")
                    .Description("Get a doge.")
                    .Parameter("phrase", ParameterType.Multiple)
                    .Do(async e =>
                    {
                        var r = new Random();
                        string dogeFile = $"servers\\{e.Server.Id}\\{r.Next(10000, 99999)}.png";

                        string dogeText = null;
                        foreach(string arg in e.Args)
                        {
                            if (dogeText == null)
                            {
                                dogeText += arg;
                            } else
                            {
                                dogeText += "/" + arg;
                            }
                        }

                        using (WebClient client = new WebClient())
                        {
                            client.DownloadFile($"http://dogr.io/wow/{Uri.UnescapeDataString(dogeText)}.png", dogeFile);
                        }

                        await e.Channel.SendFile(dogeFile);
                        System.IO.File.Delete(dogeFile);
                    });
            });

            DogeyConsole.Write("Bot Module loaded.");
        }
    }
}
