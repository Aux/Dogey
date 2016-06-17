using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Common.Models;
using Dogey.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dogey.Common.Modules
{
    public class CustomModule : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;
        private string serverFolder;
        
        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;
            serverFolder = null;

            LoadExistingCommands(manager);

            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand("commands")
                    .Description("Displays a list of all available custom commands for this server.")
                    .Do(async e =>
                    {
                        serverFolder = $@"servers\{e.Server.Id}\commands\";

                        var commands = new List<string>();
                        var dir = new DirectoryInfo(serverFolder);
                        var commandFiles = dir.GetFiles("*.doge");
                        foreach(FileInfo file in commandFiles)
                        {
                            commands.Add(file.Name.Replace(".doge", ""));
                        }

                        await e.Channel.SendMessage($"**Commands:**\n{string.Join(", ", commands)}");
                    });
                cmd.CreateCommand("create")
                    .Description("Create a new custom command.")
                    .Parameter("custom", ParameterType.Required)
                    .Parameter("message", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        serverFolder = $@"servers\{e.Server.Id}\commands\";
                        if (e.Args.Count() < 1)
                        {
                            await e.Channel.SendMessage("`create {command} [message]`");
                            return;
                        }

                        if (!Directory.Exists(serverFolder)) Directory.CreateDirectory(serverFolder);

                        string commandText = Regex.Replace(e.Args[0], @"[^\w\s]", "");

                        if (File.Exists(Path.Combine(serverFolder, commandText + ".doge")))
                        {
                            await e.Channel.SendMessage($"Command `{commandText}` already exists.");
                            return;
                        }
                        
                        var command = new CustomCommand()
                        {
                            Name = commandText,
                            CreatedBy = e.User.Id,
                            CreatedOn = DateTime.Now
                        };
                        if (e.Args.Count() > 1) command.Messages.Add(e.Args[1]);

                        string json = JsonConvert.SerializeObject(command);
                        File.Create(Path.Combine(serverFolder, command.Name + ".doge")).Close();
                        File.WriteAllText(Path.Combine(serverFolder, command.Name + ".doge"), json);

                        CreateCommand(manager, command);

                        await e.Channel.SendMessage($"Successfully created `{command.Name}`");
                    });
                cmd.CreateCommand("delete")
                    .Description("Delete an existing custom command.")
                    .Parameter("custom", ParameterType.Required)
                    .Do(async e =>
                    {
                        string commandFile = $@"servers\{e.Server.Id}\commands\{e.Args[0]}.doge";

                        if (File.Exists(commandFile))
                        {
                            File.Delete(commandFile);
                        } else
                        {
                            await e.Channel.SendMessage($"{e.Args[0]} is not an existing command.");
                        }

                        await e.Channel.SendMessage($"Successfully deleted `{e.Args[0]}`.");
                    });
            });

            DogeyConsole.Write("Custom Module loaded.");
        }

        public void LoadExistingCommands(ModuleManager manager)
        {
            int servers = 0;
            int commands = 0;
            foreach (string folder in Directory.GetDirectories("servers"))
            {
                foreach (string file in Directory.GetFiles(Path.Combine(folder, "commands")))
                {
                    var cmd = JsonConvert.DeserializeObject<CustomCommand>(File.ReadAllText(file));

                    CreateCommand(manager, cmd);
                    commands++;
                }
                servers++;
            }

            DogeyConsole.Write($"Loaded {commands} command(s) for {servers} server(s).");
        }

        public void CreateCommand(ModuleManager manager, CustomCommand command)
        {
            manager.CreateCommands("", cmd =>
            {
                cmd.CreateCommand(command.Name)
                    .Description("Custom command.")
                    .Parameter("Index", ParameterType.Optional)
                    .Do(async e =>
                    {
                        string commandFile = $@"servers\{e.Server.Id}\commands\{e.Command.Text}.doge";
                        if (!File.Exists(commandFile)) return;

                        var cmdObj = JsonConvert.DeserializeObject<CustomCommand>(File.ReadAllText(commandFile));

                        int i;
                        bool isNumeric = Int32.TryParse(e.Args[0], out i);
                        if (isNumeric)
                        {
                            await e.Channel.SendMessage($"**{i}** {cmdObj.Messages[i]}");
                            return;
                        }

                        var r = new Random();
                        string message = cmdObj.Messages[r.Next(0, cmdObj.Messages.Count())];

                        await e.Channel.SendMessage($"**{cmdObj.Messages.IndexOf(message)}** {message}");
                    });
                cmd.CreateCommand(command.Name + ".add")
                    .Description("Add a new message to the custom command.")
                    .Parameter("Message", ParameterType.Unparsed)
                    .Do(async e =>
                    {
                        string commandName = e.Command.Text.Split('.')[0];
                        string commandFile = $@"servers\{e.Server.Id}\commands\{commandName}.doge";
                        if (!File.Exists(commandFile)) return;
                        if (string.IsNullOrEmpty(e.Args[0])) return;

                        var cmdObj = JsonConvert.DeserializeObject<CustomCommand>(File.ReadAllText(commandFile));
                        cmdObj.Messages.Add(e.Args[0]);

                        File.WriteAllText(commandFile, JsonConvert.SerializeObject(cmdObj));
                        
                        await e.Channel.SendMessage($"Added message #{cmdObj.Messages.Count()} to `{cmdObj.Name}`.");
                    });
                cmd.CreateCommand(command.Name + ".del")
                    .Description("Custom command.")
                    .Parameter("Index", ParameterType.Required)
                    .Do(async e =>
                    {
                        string commandName = e.Command.Text.Split('.')[0];
                        string commandFile = $@"servers\{e.Server.Id}\commands\{commandName}.doge";
                        if (!File.Exists(commandFile)) return;

                        int i;
                        bool isNumeric = Int32.TryParse(e.Args[0], out i);
                        if (!isNumeric)
                        {
                            await e.Channel.SendMessage($"**{e.Args[0]}** is not a valid index.");
                            return;
                        }
                        
                        var cmdObj = JsonConvert.DeserializeObject<CustomCommand>(File.ReadAllText(commandFile));
                        if (cmdObj.Messages.Count() < i)
                        {
                            await e.Channel.SendMessage($"**{e.Args[0]}** is not a valid index.");
                            return;
                        }

                        cmdObj.Messages.RemoveAt(i);

                        File.WriteAllText(commandFile, JsonConvert.SerializeObject(cmdObj));

                        await e.Channel.SendMessage($"Deleted message #{i} from `{cmdObj.Name}`.");
                    });
                cmd.CreateCommand(command.Name + ".count")
                    .Description("Custom command.")
                    .Do(async e =>
                    {
                        string commandName = e.Command.Text.Split('.')[0];
                        string commandFile = $@"servers\{e.Server.Id}\commands\{commandName}.doge";
                        if (!File.Exists(commandFile)) return;

                        var cmdObj = JsonConvert.DeserializeObject<CustomCommand>(File.ReadAllText(commandFile));
                        
                        await e.Channel.SendMessage($"`{cmdObj.Name}` currently contains **{cmdObj.Messages.Count()}** message(s).");
                    });
                cmd.CreateCommand(command.Name + ".created")
                    .Description("Get the time and who created this custom command.")
                    .Do(async e =>
                    {
                        string commandName = e.Command.Text.Split('.')[0];
                        string commandFile = $@"servers\{e.Server.Id}\commands\{commandName}.doge";
                        if (!File.Exists(commandFile)) return;

                        var cmdObj = JsonConvert.DeserializeObject<CustomCommand>(File.ReadAllText(commandFile));

                        string message = $"The command `{cmdObj.Name}` was created on "+
                                         $"**{cmdObj.CreatedOn.ToString("MMM d, yyyy")}** at " +
                                         $"**{cmdObj.CreatedOn.ToString("h:mm:ss tt z")}** " +
                                         $"by **{e.Server.GetUser(cmdObj.CreatedBy).Name}**.";

                        await e.Channel.SendMessage(message);
                    });
            });
        }
    }
}