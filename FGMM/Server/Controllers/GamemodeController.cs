using System;
using System.Dynamic;
using System.Collections.Generic;
using FGMM.SDK.Core.Diagnostics;
using FGMM.SDK.Server.Controllers;
using FGMM.SDK.Server.Events;
using FGMM.SDK.Server.RPC;
using FGMM.SDK.Server.Gamemodes;
using FGMM.Server.Models;
using FGMM.SDK.Core.RPC.Events;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace FGMM.Server.Controllers
{
    class GamemodeController : Controller
    {
        private IGamemode CurrentGamemode {get; set;}

        public GamemodeController(ILogger logger, IEventManager events, IRpcHandler rpc) : base(logger, events, rpc)
        {
            Logger.Info("Gamemode controller started");
            CurrentGamemode = null;
            Rpc.Event(ClientEvents.RequestGamemode).On(OnGamemodeNameRequest);
            Events.On<IGamemode>(ServerEvents.MissionStarted, OnGamemodeChanged);
            Rpc.Event(ClientEvents.RequestTeamSelection).On(OnTeamSelectionRequested);
            Rpc.Event(ClientEvents.JoinTeamRequest).On<int>(OnJoinTeamRequested);
            Rpc.Event("baseevents:onPlayerDied").OnRaw(new Action<Player, int, List<object>>(OnPlayerDied));
            Rpc.Event("baseevents:onPlayerKilled").OnRaw(new Action<Player, int, ExpandoObject>(OnPlayerKilled));
            Rpc.Event(ClientEvents.PlayerDropped).OnRaw(new Action<Player, string, CallbackDelegate>(OnPlayerDropped));
            Rpc.Event(ClientEvents.PlayerConnecting).OnRaw(new Action<Player, string, CallbackDelegate, ExpandoObject>(OnPlayerConnecting));
        }

        private void OnPlayerDied([FromSource]Player player, int killerType, List<object> coords)
        {
            Logger.Debug($"onPlayerDied recieved and forwarded to {CurrentGamemode?.Mission.Gamemode} gamemode");
            CurrentGamemode?.HandleDeath(player, null);
        }

        private void OnPlayerKilled([FromSource]Player player, int killerId, ExpandoObject data)
        {
            Logger.Debug($"onPlayerKilled recieved and forwarded to {CurrentGamemode?.Mission.Gamemode} gamemode");
            CurrentGamemode?.HandleDeath(player, new PlayerList()[killerId]);
        }

        private void OnPlayerConnecting([FromSource]Player player, string playerName, CallbackDelegate drop, ExpandoObject callbacks)
        {
            Logger.Info($"Player {player.Name} Connected on IP: {player.EndPoint}");
        }

        public void OnPlayerDropped([FromSource] Player player, string disconnectMessage, CallbackDelegate drop)
        {
            Logger.Info($"Player {player.Name} ({player.Handle}) disconnected. Reason: {disconnectMessage}");
            Rpc.Event(ClientEvents.CleanupPeds).Trigger();
        }

        private void OnJoinTeamRequested(IRpcEvent rpc, int teamId)
        {
            Player player = new PlayerList()[rpc.Client.Handle];
            bool joinedGame = CurrentGamemode.HandleTeamJoinRequest(player, teamId);
            rpc.Reply(joinedGame);                 
        }

        private void OnTeamSelectionRequested(IRpcEvent rpc)
        {
            rpc.Reply(CurrentGamemode.Mission.SelectionData);
        }

        private void OnGamemodeChanged(IGamemode gamemode)
        {
            CurrentGamemode = gamemode;
        }

        private void OnGamemodeNameRequest(IRpcEvent rpc)
        {
            if(CurrentGamemode == null || CurrentGamemode.Mission == null)
                rpc.Reply(string.Empty);
            else
                rpc.Reply(CurrentGamemode.Mission.Gamemode);
        }
    }
}
