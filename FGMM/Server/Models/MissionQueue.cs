using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitizenFX.Core.Native;


namespace FGMM.Server.Models
{
    public class MissionQueue
    {
        public List<string> Missions;

        public int UpcomingMission;

        private Random Rng { get; set; }

        public MissionQueue()
        {
            Rng = new Random();
            Update();
        }

        public void Update()
        {
            Missions = GetInstalledMissions().ToList();
            Shuffle();
        }

        public void Shuffle()
        {
            if (Missions.Count < 1)
                throw new Exception("Your missions folder is empty!");

            int n = Missions.Count;
            while (n > 1)
            {
                n--;
                int k = Rng.Next(n + 1);
                string value = Missions[k];
                Missions[k] = Missions[n];
                Missions[n] = value;
            }
            UpcomingMission = 0;
        }

        public string Pop()
        {
            if (UpcomingMission < Missions.Count)
                return Missions[UpcomingMission++];
            string LastMission = Missions[Missions.Count - 1];
            do
            {
                Shuffle();
            }
            while (LastMission == Missions[0] && Missions.Count > 1);
            UpcomingMission = 0;
            return Missions[UpcomingMission++];
        }

        private IEnumerable<string> GetInstalledMissions()
        {
            string Path = $"resources/{API.GetCurrentResourceName()}/Missions";
            DirectoryInfo directoryInfo = new DirectoryInfo(Path);
            FileInfo[] filesInfo = directoryInfo.GetFiles();
            foreach (FileInfo info in filesInfo)
                yield return info.Name;
        }

    }
}
