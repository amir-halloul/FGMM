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

        [JsonIgnore]
        private Random Rng { get; set; }

        public Team()
        {
            Rng = new Random();
        }

        public Position GetRandomSpawnPoint()
        {
            int point = Rng.Next(SpawnPoints.Count);
            return SpawnPoints[point];
        }

        public string GetRandomSkin()
        {
            int skin = Rng.Next(Skins.Count);
            return Skins[skin];
        }
    }
}
