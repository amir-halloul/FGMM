using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;

namespace FGMM.SDK.Gamemodes.Models
{
    public class TeamList
    {
        public List<Team> Teams { get; set; }

        public TeamList()
        {
            Teams = new List<Team>();
        }

        public Team GetWinningTeam()
        {
            Team team = Teams.OrderBy(s => s.Score).FirstOrDefault();
            return team;
        }

        public bool IsGameTied()
        {
            bool tied = false;
            int maxScore = int.MinValue;

            foreach(Team team in Teams)
            {
                if (team.Score == maxScore)
                    tied = true;
                else if(team.Score > maxScore)
                {
                    tied = false;
                    maxScore = team.Score;
                }
            }

            return tied;
        }

        public Team GetPlayerTeam(Player player)
        {
            foreach(Team team in Teams)
            {
                if (team.Players.Contains(player))
                    return team;
            }
            return null;
        }

        public bool CanPlayerJoinTeam(Team team)
        {
            if (!Teams.Contains(team))
                return false;

            if (Teams.Count == 1)
                return true;

            foreach (Team t in Teams)
            {
                if (t == team)
                    continue;
                if (t.Players.Count >= team.Players.Count)
                    return true;
            }

            return false;
        }

    }
}
