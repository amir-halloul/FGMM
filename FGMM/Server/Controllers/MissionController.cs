using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Threading.Tasks;
using FGMM.SDK.Core.Diagnostics;
using FGMM.SDK.Server.Controllers;
using FGMM.SDK.Server.Events;
using FGMM.SDK.Server.RPC;
using FGMM.SDK.Server.Gamemodes;
using FGMM.Server.Models;
using FGMM.SDK.Core.RPC.Events;
using FGMM.SDK.Server.Diagnostics;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Dynamic;

namespace FGMM.Server.Controllers
{
    class MissionController : Controller
    {
        private MissionQueue Missions;
        private Dictionary<string, IGamemode> Gamemodes;
        private string CurrentGamemode;

        public MissionController(ILogger logger, IEventManager events, IRpcHandler rpc) : base(logger, events, rpc)
        {
            Logger.Info("Initializing...");

            API.SetGameType("Waiting for players...");
            API.SetMapName("N/A");

            Gamemodes = new Dictionary<string, IGamemode>();
            Missions = new MissionQueue();

            Rpc.Event(ClientEvents.PlayerDropped).OnRaw(new Action<Player, string, CallbackDelegate>(OnPlayerDropped));
            Rpc.Event(ClientEvents.PlayerConnecting).OnRaw(new Action<Player, string, CallbackDelegate, ExpandoObject>(OnPlayerConnecting));

            Events.On(ServerEvents.EndMission, OnEndMissionRequested);

            if(API.GetNumPlayerIndices() > 0)
                StartNextMission();
        }

        private void OnPlayerConnecting([FromSource]Player player, string name, CallbackDelegate drop, ExpandoObject callbacks)
        {
            // Start a mission when the first player logs in

            int players = API.GetNumPlayerIndices();
            Logger.Debug($"Number of players: {players + 1}");
            // First player logged in
            if (players == 0)
                StartNextMission();
        }

        private void OnPlayerDropped([FromSource]Player player, string reason, CallbackDelegate callbacks)
        {
            // Stop current mission when last player logs out

            int players = API.GetNumPlayerIndices();
            Logger.Debug($"Number of players: {players - 1}");
            // Last player logged out
            if (players == 1)
                StopMission();
        }

        private async void OnEndMissionRequested()
        {
            // End current mission and start next one.
            await StopMission();
            await BaseScript.Delay(10000);
            StartNextMission();
        }

        public async Task StopMission()
        {
            if (CurrentGamemode == null)
                return;

            await BaseScript.Delay(0);
            
            Gamemodes[CurrentGamemode].Stop();
            CurrentGamemode = null;

            API.SetGameType("Waiting for players...");
            API.SetMapName("N/A");

            Rpc.Event(ServerEvents.MissionEnded).Trigger();
        }

        public void StartNextMission()
        {
            // Pick a mission
            string mission = Missions.Pop();
            string missionGamemode = GetMissionGamemode(mission);

            // Load gamemode if not already loaded.
            if (!Gamemodes.ContainsKey(missionGamemode))
            {
                IGamemode gamemode = LoadGamemode(missionGamemode);
                Gamemodes.Add(missionGamemode, gamemode);
            }

            CurrentGamemode = missionGamemode;
            Gamemodes[CurrentGamemode].Start(mission);

            API.SetGameType(CurrentGamemode);
            API.SetMapName(Gamemodes[CurrentGamemode].Mission.Name);

            Events.Raise(ServerEvents.MissionStarted, Gamemodes[CurrentGamemode]);
            Rpc.Event(ServerEvents.MissionStarted).Trigger(missionGamemode);           
        }

        private IGamemode LoadGamemode(string gamemode)
        {
            string path = $"resources/{API.GetCurrentResourceName()}/Gamemodes/{gamemode}/FGMM.Gamemode.{gamemode}.Server.net.dll";

            if (!File.Exists(path))
                throw new FileNotFoundException($"Gamemode \"{gamemode}\" not found in path: {path}");

            Assembly assembly = Assembly.LoadFrom(path);
            Type type = assembly.GetType($"FGMM.Gamemode.{gamemode}.Server.{gamemode}");
            List<object> ctorArgs = new List<object>()
            {
                new Logger($"{gamemode} Gamemode"),
                Events,
                Rpc
            };

            IGamemode gamemodeInstance = (IGamemode)Activator.CreateInstance(type, ctorArgs.ToArray());
            return gamemodeInstance;
        }

        private string GetMissionGamemode(string mission)
        {
            string Path = $"resources/{API.GetCurrentResourceName()}/Missions/{mission}";
            if (!File.Exists(Path))
                throw new Exception($"Mission \"{mission}\" was not found in your missions folder.");
            try
            {
                XmlDocument missionDoc = new XmlDocument();
                missionDoc.Load(Path);
                XmlNodeList xmlNodeList = missionDoc.GetElementsByTagName("Gamemode");
                return xmlNodeList[0].InnerText;
            }
            catch
            {
                throw new Exception($"Unable to find mission \"{mission}\"'s type.");
            }
        }
    }
}
