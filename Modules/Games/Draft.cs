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

                        int teamCount = Convert.ToInt32(e.Args[0]);
                        int playersPerTeam = 10;
                        
                        if (!string.IsNullOrEmpty(e.Args[1])) playersPerTeam = Convert.ToInt32(e.Args[1]);

                        var draftMsg = await e.Channel.SendMessage(
                            $"@here, There are currently 0/{teamCount * playersPerTeam} players waiting. " +
                            $"\nType `{Program.config.DefaultPrefix}draft join` to play!" +
                            "\n**Time Remaining:** 30 seconds"
                            );

                        int timer = 30;
                        int maxPlayers = teamCount * playersPerTeam;
                        while (Players.Count < maxPlayers)
                        {
                            if (!DraftInProgress) {
                                await draftMsg.Delete();
                                await e.Channel.SendMessage("Draft Cancelled.");
                                return;
                            }

                            await Task.Delay(1000);
                            timer -= 1;
                            await draftMsg.Edit(
                                $"@here, There are currently {Players.Count()}/{maxPlayers} players waiting. " +
                                $"Type `{Program.config.DefaultPrefix}draft join` to play!" +
                                $"\n**Time Remaining:** {timer} seconds");
                            if (timer <= 0) break;
                        }

                        await draftMsg.Delete();
                        if (Players.Count() < maxPlayers)
                        {
                            await e.Channel.SendMessage($"I need {maxPlayers - Players.Count()} more players to start a draft.");
                            return;
                        } else
                        {
                            List<int> chosenTeams = new List<int>();
                            List<int> chosenCapts = new List<int>();
                            for (int i = 0; i <= teamCount; i++)
                            {
                                int teami = new Random().Next(0, DraftColor.Colors.Count() - 1);
                                do { teami = new Random().Next(0, DraftColor.Colors.Count() - 1); } while (chosenTeams.Contains(teami));
                                chosenTeams.Add(teami);
                                int capti = new Random().Next(0, Players.Count() - 1);
                                do { capti = new Random().Next(0, Players.Count() - 1); } while (chosenCapts.Contains(capti));
                                chosenCapts.Add(capti);

                                DraftTeam team = new DraftTeam()
                                {
                                    Name = DraftColor.Names[teami],
                                    Color = DraftColor.Colors[teami],
                                    Captain = Players[capti]
                                };

                                team.Role = await e.Server.CreateRole(
                                    name: "Team " + team.Name,
                                    color: team.Color);
                            }

                            string captainMsg = "The team captains have been chosen!\n";
                            foreach(DraftTeam team in Teams)
                            {
                                captainMsg += $"{team.Name}'s captain is {team.Captain.Mention}";
                            }
                            await e.Channel.SendMessage(captainMsg);
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
        
        public static async void MoveTeams(CommandEventArgs e, List<DraftTeam> draftTeams)
        {
            await e.Channel.SendMessage("");
        }

    }
}
