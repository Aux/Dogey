using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class UsageModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("usage", cmd =>
            {
                cmd.CreateCommand("")
                    .Description("Provide the user with information about the usage command.")
                    .Do(async e =>
                    {
                        await e.User.SendMessage("Don't");
                    });
                cmd.CreateCommand("server")
                    .Description("The total number of times a command has been executed on this server.")
                    .Parameter("command", ParameterType.Optional)
                    .Do(async e =>
                    {

                        await e.Channel.SendMessage($"The command `._.` has been executed **0** times in this server.");
                    });
                cmd.CreateCommand("all")
                    .Description("The total number of times a command has been executed across all servers.")
                    .Parameter("command", ParameterType.Optional)
                    .Do(async e =>
                    {
                        
                        await e.Channel.SendMessage($"The command `._.` has been executed **0** times total.");
                    });
                cmd.CreateCommand("total.executed")
                    .Description("The total number of times a command has been executed.")
                    .Parameter("command", ParameterType.Optional)
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("");
                    });
                cmd.CreateCommand("total")
                    .Description("The total number of commands that exist across all servers.")
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("");
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "UsageModule", "Loaded.");
        }
    }
}