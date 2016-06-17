using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class DogeyModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("evaluate")
                    .Description("Do some math.")
                    .Parameter("Math", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        var express = new Expression(e.Args[0]);

                        await e.Channel.SendMessage($"The solution for **{e.Args[0]}** is **{express.Evaluate()}**");
                    });
            });

            DogeyConsole.Write("Dogey Module loaded.");
        }
    }
}