using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using System;
using System.Collections.Generic;
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
                        int MaxDeletion = 100;
                        User user = e.Message.MentionedUsers.FirstOrDefault();

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
                            if (e.Message.User == user)
                            {
                                await e.Message.Delete();
                                deletedMessages++;
                            }
                        }

                        await e.Channel.SendMessage($"Deleted **{deletedMessages}** message(s) by **{user.Name}**");
                    });
            });

            DogeyConsole.Write("Admin Module loaded.");
        }
    }
}