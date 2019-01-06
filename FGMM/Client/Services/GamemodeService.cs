using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FGMM.SDK.Client.Diagnostics;
using FGMM.SDK.Client.Events;
using FGMM.SDK.Client.Gamemodes;
using FGMM.SDK.Client.RPC;
using FGMM.SDK.Client.Services;
using FGMM.SDK.Core.Diagnostics;
using FGMM.SDK.Core.Models;
using FGMM.SDK.Core.RPC.Events;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;

namespace FGMM.Client.Services
{
    class GamemodeService : Service
    {
        private Dictionary<string, IGamemode> Gamemodes;

        private string CurrentGamemode;
        private IGamemode Gamemode;

        public GamemodeService(ILogger logger, IEventManager events, IRpcHandler rpc, ITickManager tickManager) : base(logger, events, rpc, tickManager) { }

        public override async Task Started()
        {
            Logger.Info("Gamemode service started!");
            Gamemodes = new Dictionary<string, IGamemode>();

            Rpc.Event(ServerEvents.MissionStarted).On(OnMissionStarted);
            Rpc.Event(ServerEvents.MissionEnded).On(OnMissionEnded);

            CurrentGamemode = await RequestGamemode();
            if(!string.IsNullOrEmpty(CurrentGamemode))
            {
                Logger.Info($"The server is currently running {CurrentGamemode} gamemode.");
                Gamemode = GetGamemode(CurrentGamemode);
                Events.Raise(ClientEvents.StartTeamSelection, await Rpc.Event(ClientEvents.RequestTeamSelection).Request<SelectionData>());
            }
            else
                Logger.Info("No gamemode is currently running on the server. Waiting for the MissionStarted event!");
        }

        
        private async void OnMissionStarted(IRpcEvent rpc)
        {
            do
            {
                CurrentGamemode = await RequestGamemode();
                await BaseScript.Delay(2000);
            }
            while (string.IsNullOrEmpty(CurrentGamemode));

            Gamemode = GetGamemode(CurrentGamemode);           
            Events.Raise(ClientEvents.StartTeamSelection, await Rpc.Event(ClientEvents.RequestTeamSelection).Request<SelectionData>());
        }

        private async void OnMissionEnded(IRpcEvent rpc)
        {
            Logger.Debug("Ending mission...");
            if(Gamemode == null)
                Logger.Warning("The client tried to end a non existing gamemode!");
            else
            {
                Gamemode.Stop();
                Gamemode = null;
            }

            API.SetPlayerTeam(Game.Player.Handle, -1);

            Game.Player.CanControlCharacter = false;
            Game.PlayerPed.IsPositionFrozen = true;
            if(!Game.PlayerPed.IsDead)
                API.ClearPedTasksImmediately(Game.PlayerPed.Handle);

            Screen.Fading.FadeOut(1000);
            while (Screen.Fading.IsFadingOut)
                await BaseScript.Delay(100);

            API.SetCloudHatOpacity(0.01f);

            if (!API.IsPlayerSwitchInProgress())
                API.SwitchOutPlayer(API.PlayerPedId(), 0, 1);
            while (API.GetPlayerSwitchState() != 5)
                await BaseScript.Delay(50);

            Screen.Fading.FadeIn(1000);
        }

        private async Task<string> RequestGamemode()
        {
            return await Rpc.Event(ClientEvents.RequestGamemode).Request<string>();           
        }

        private IGamemode GetGamemode(string gamemode)
        {
            if (Gamemodes.ContainsKey(gamemode))
            {
                Gamemodes[gamemode].Start();
                return Gamemodes[gamemode];
            }
                

            string AssemblyName = $"FGMM.Gamemode.{gamemode}.Client.net";
            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name == AssemblyName).FirstOrDefault();
            Type type = assembly.GetType($"FGMM.Gamemode.{gamemode}.Client.{gamemode}");

            List<object> ctorArgs = new List<object>
            {
                new Logger($"{gamemode}Gamemode"),
                Events,
                Rpc,
                TickManager
            };

            IGamemode gamemodeInstance = (IGamemode)Activator.CreateInstance(type, ctorArgs.ToArray());
            Gamemodes.Add(gamemode, gamemodeInstance);
            gamemodeInstance.Start();
            return gamemodeInstance;
        }
    }
}
