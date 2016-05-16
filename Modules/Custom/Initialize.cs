using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Custom
{
    public class Initialize : IModule
    {
        private ModuleManager _manager;
        private Discord.DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                //cmd.CreateCommand("doge")
                //    .Description("Get a doge.")
                //    .Parameter("phrase", ParameterType.Multiple)
                //    .Do(async e =>
                //    {
                //        await e.Channel.SendMessage("Not implemented.");
                //    });
            });

            DogeyConsole.Write("Custom Module loaded.");
        }
    }
}
