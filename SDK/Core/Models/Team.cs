using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FGMM.SDK.Core.Models
{
    public class Team
    {
        public string Name { get; set; }
        public List<string> Skins { get; set; }
        public List<Position> SpawnPoints { get; set; }
        public List<Weapon> Loadout { get; set; }

        public Team()
        {
            
        }

        public Position GetRandomSpawnPoint()
        {
            Random Rng = new Random();
            int point = Rng.Next(SpawnPoints.Count);
            return SpawnPoints[point];
        }

        public string GetRandomSkin()
        {
            Random Rng = new Random();
            int skin = Rng.Next(Skins.Count);
            return Skins[skin];
        }
    }
}
