using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Chatlog
{
    public class Initialize : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("chat", cmd =>
            {
                cmd.CreateCommand("totalmsgs")
                    .Description("Get the number of messages posted on this server, optionally specify a user and/or a span of time.")
                    .Parameter("user", ParameterType.Optional)
                    .Parameter("begintime", ParameterType.Optional)
                    .Parameter("endtime", ParameterType.Optional)
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("`!chat totalmsgs [user] [begintime] [endtime]");
                    });
            });

            DogeyConsole.Write("Chatlog Module loaded.");
        }
    }
}
