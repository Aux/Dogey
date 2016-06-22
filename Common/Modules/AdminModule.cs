using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Common.Models;
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
    class AdminModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("erase")
                    .Description("Clear a specific user's messages from the current channel.")
                    .Parameter("user", ParameterType.Required)
                    .Parameter("messages", ParameterType.Optional)
                    .Do(async e =>
                    {
                        int MaxDeletion = 25;
                        User user = e.Server.FindUsers(e.Args[0]).FirstOrDefault();

                        int deletion = MaxDeletion++;
                        if (!string.IsNullOrEmpty(e.Args[1]))
                        {
                            bool isNumeric = Int32.TryParse(e.Args[0], out deletion);
                        }
                        if (deletion > MaxDeletion) deletion = MaxDeletion;

                        IEnumerable<Message> msgs;
                        if (e.Channel.Messages.Count() < deletion)
                            msgs = await e.Channel.DownloadMessages(deletion);
                        else
                            msgs = e.Channel.Messages.OrderByDescending(x => x.Timestamp).Take(deletion);

                        int deletedMessages = 0;
                        foreach (Message msg in msgs)
                        {
                            if (msg.User.Id == user.Id)
                            {
                                await msg.Delete();
                                deletedMessages++;
                            }
                        }

                        var message = await e.Channel.SendMessage($"Deleted **{deletedMessages}** message(s) by **{user.Name}**");
                        await Task.Delay(10000);
                        await e.Message.Delete();
                    });
                cmd.CreateCommand("bans")
                    .Description("Shows the number of Dogey servers this user is banned from.")
                    .Parameter("user", ParameterType.Required)
                    .Do(async e =>
                    {
                        string banFile = $@"bans\{e.User.Id}.ban";
                        var user = e.Server.FindUsers(e.Args[0]).FirstOrDefault();

                        if (!File.Exists(banFile))
                        {
                            await e.Channel.SendMessage($"`{e.User.Name}` has not been banned from any servers.");
                            return;
                        } else
                        {
                            var ban = JsonConvert.DeserializeObject<UserBans>(File.ReadAllText(banFile));
                            
                            await e.Channel.SendMessage($"`{e.User.Name}` has been banned from {ban.Servers.Count()} server(s).");
                            return;
                        }
                    });
            });

            DogeyConsole.Log(LogSeverity.Info, "AdminModule", "Loaded.");
        }
    }
}