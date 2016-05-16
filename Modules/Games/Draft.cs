using Discord;
using Discord.Commands;
using Discord.Modules;
using Dogey.Modules.Games.Types;
using Dogey.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Games
{
    public class Draft : IModule
    {
        private ModuleManager _manager;
        private DiscordClient _dogey;

        private bool DraftInProgress = false;
        private List<User> Players = new List<User>();
        private List<DraftTeam> Teams = new List<DraftTeam>();

        void IModule.Install(ModuleManager manager)
        {
            _manager = manager;
            _dogey = manager.Client;

            manager.CreateCommands("draft", cmd =>
            {
                cmd.CreateCommand("new")
                    .Description("Start a team draft.")
                    .Parameter("teams", ParameterType.Required)
                    .Parameter("playersPerTeam", ParameterType.Optional)
                    .Do(async e =>
                    {
                        if (DraftInProgress) return;
                        DraftInProgress = true;
                        Players.Clear();

                        int teams = Convert.ToInt32(e.Args[0]);
                        int playersPerTeam = 10;
                        
                        if (!string.IsNullOrEmpty(e.Args[1])) playersPerTeam = Convert.ToInt32(e.Args[1]);

                    var draftMsg = await e.Channel.SendMessage(
                        $"@here, There are currently 0/{teams * playersPerTeam} players waiting. " +
                        $"\nType `{Program.config.DefaultPrefix}draft join` to play!" +
                        "\n**Time Remaining:** 30 seconds"
                        );
                        
                        int timer = 30;
                        int maxPlayers = teams * playersPerTeam;
                        while (Players.Count < maxPlayers)
                        {
                            if (!DraftInProgress) { await e.Channel.SendMessage("Draft Cancelled."); return; }

                            await Task.Delay(1000);
                            timer -= 1;
                            await draftMsg.Edit(
                                $"@here, There are currently {Players.Count()}/{maxPlayers} players waiting. " +
                                $"Type `{Program.config.DefaultPrefix}draft join` to play!" +
                                $"\n**Time Remaining:** {timer} seconds");
                            if (timer <= 0) break;
                        }

                        if (Players.Count() < maxPlayers)
                        {
                            await e.Channel.SendMessage("Not enough players joined to initiate a draft.");
                            return;
                        } else
                        {
                            //Pick team captains, pick teams, create voice channels, move teams to channels.
                        }

                        DraftInProgress = false;
                    });
                cmd.CreateCommand("join")
                    .Description("Join the currently running draft.")
                    .Do(e =>
                    {
                        Players.Add(e.User);
                    });
                cmd.CreateCommand("stop")
                    .Description("Stop the currently running draft.")
                    .Do(e =>
                    {
                        DraftInProgress = false;
                    });
            });

            DogeyConsole.Write("Games Module loaded.");
        }
    }
}
