using System.Collections.Generic;
using CitizenFX.Core;

namespace FGMM.SDK.Gamemodes.Models
{
    public class Team
    {
        public string Name { get; set; }
        
        public List<Player> Players { get; set; }

        public int Score { get; set; }

        public Team()
        {
            Players = new List<Player>();
        }
    }
}
