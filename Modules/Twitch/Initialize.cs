using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Twitch
{
    class Initialize : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("twitch", cmd =>
            {
                cmd.CreateCommand("online")
                    .Description("Check if this channel is streaming.")
                    .Parameter("channel", ParameterType.Multiple)
                    .Do(async e =>
                    {
                        string user = e.Args.Aggregate((i, j) => i + " " + j);

                        using(var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate}))
                        {
                            client.BaseAddress = new Uri($"https://api.twitch.tv/kraken/streams/{user}");

                        }


                        await e.Channel.SendMessage("");
                    });
            });

            DogeyConsole.Write("Twitch Module loaded.");
        }
    }
}
