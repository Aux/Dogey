using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Utility;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
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
                cmd.CreateCommand("count")
                    .Description("Get the number of messages posted on this server, optionally specify a user and/or a span of time.")
                    .Parameter("phrase", ParameterType.Unparsed)
                    //.Parameter("begintime", ParameterType.Optional)
                    //.Parameter("endtime", ParameterType.Optional)
                    .Do(async e =>
                    {
                        string serverFolder = $"servers\\{e.Server.Id}";
                        string chatFile = $"{serverFolder}\\chatlog.doge";

                        string user = e.Args[0];
                        int count = 0;

                        string msgWrite;
                        if (string.IsNullOrEmpty(e.Args[0]))
                        {
                            msgWrite = $"SELECT COUNT(*) FROM msgs WHERE UserID = {e.User.Id};";
                        } else
                        {
                            msgWrite = $"SELECT COUNT(*) FROM msgs WHERE UserID = {e.User.Id} AND INSTR(Message, '@1') > 0";
                        }

                        using (var sql = SQLite.Connect(chatFile))
                        using (var sqlcmd = new SQLiteCommand(msgWrite, sql))
                        {
                            if (!string.IsNullOrEmpty(e.Args[0])) sqlcmd.Parameters.AddWithValue("@1", e.Args[0]);
                            count = Convert.ToInt32(sqlcmd.ExecuteScalar());
                        }
                        
                        await e.Channel.SendMessage($"{e.User.Mention}, you have posted {count} messages.");
                    });
                //cmd.CreateCommand("download")
                //    .Description("Download all chat messages in the current server.")
                //    .Do(async e =>
                //    {
                //        await e.Channel.SendMessage($"{e.User.Mention}, starting message download...");

                //        IEnumerable<Message> msgs;
                //        msgs = (await e.Channel.DownloadMessages(100000));

                //        foreach(Message msg in msgs)
                //        {
                //            Log.Message(e.Server, msg);
                //        }
                        
                //        await e.Channel.SendMessage($"{e.User.Mention}, {msgs.Count()} messages have been downloaded.");
                //    });
            });

            DogeyConsole.Write("Chatlog Module loaded.");
        }
    }
}
