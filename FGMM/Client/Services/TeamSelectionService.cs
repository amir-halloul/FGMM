using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FGMM.SDK.Client.Events;
using FGMM.SDK.Client.RPC;
using FGMM.SDK.Client.Services;
using FGMM.SDK.Core.Diagnostics;
using FGMM.SDK.Core.Models;
using FGMM.SDK.Core.RPC.Events;
using FGMM.SDK.Client.UI;
using FGMM.SDK.Core.RPC;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;


namespace FGMM.Client.Services
{
    class TeamSelectionService : Service
    {
        private bool IsAwaitingSpawn = false;
        private bool IsSwitchingTeam = false;
        private int SelectedTeam = 0;

        private SelectionData Data { get; set; }
        private Camera Camera { get; set; }
        private UIManager UIManager { get; set; }

        public TeamSelectionService(ILogger logger, IEventManager events, IRpcHandler rpc, ITickManager tickManager, EventHandlerDictionary eventDict) : base(logger, events, rpc, tickManager)
        {
            UIManager = new UIManager(eventDict);

            Events.On<SelectionData>(ClientEvents.StartTeamSelection, StartTeamSelection);
            Rpc.Event(ServerEvents.EndMission).On(OnMissionEndRequested);

            UIManager.RegisterNUICallback("selectpreviousteam", NUI_OnPreviousTeamSelect);
            UIManager.RegisterNUICallback("selectnextteam", NUI_OnNextTeamSelect);
            UIManager.RegisterNUICallback("spawnplayer", NUI_OnSpawnRequest);
        }

        private void OnMissionEndRequested(IRpcEvent rpc)
        {
            ToggleSelectionScreenNui(false);
        }

        public async void StartTeamSelection(SelectionData data)
        {
            ToggleSelectionScreenNui(false);
            IsAwaitingSpawn = false;
            SelectedTeam = 0;

            Data = data;

            API.SetPlayerTeam(Game.Player.Handle, -1);

            API.NetworkSetFriendlyFireOption(true);
            API.SetCanAttackFriendly(Game.PlayerPed.Handle, true, false);

            API.SetManualShutdownLoadingScreenNui(true);

            API.ShutdownLoadingScreen();

            API.ShutdownLoadingScreenNui();

            Screen.Fading.FadeOut(0);

            API.NetworkSetInMpCutscene(true, false);
            API.SetLocalPlayerVisibleInCutscene(true, true);

            API.SwitchInPlayer(API.PlayerPedId());
            while (!await Game.Player.ChangeModel(new Model((PedHash)Enum.Parse(typeof(PedHash), data.Skins[SelectedTeam], true)))) await BaseScript.Delay(100);
            API.SetPedDefaultComponentVariation(Game.PlayerPed.Handle);

            Game.PlayerPed.Weapons.RemoveAll();
            Game.PlayerPed.Weapons.Give((WeaponHash)Enum.Parse(typeof(WeaponHash), data.Weapons[SelectedTeam].Hash, true), 1, true, true);

            await BaseScript.Delay(3000);

            Game.Player.CanControlCharacter = false;
            Game.PlayerPed.IsPositionFrozen = true;
            Game.PlayerPed.PositionNoOffset = new Vector3(data.Position.X, data.Position.Y, data.Position.Z);
            Game.PlayerPed.Rotation = new Vector3(0, 0, data.Position.A);
            API.ClearPedTasksImmediately(Game.PlayerPed.Handle);

            // Camera code            
            Camera = new Camera(API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true));
            Camera.IsActive = true;
            API.RenderScriptCams(true, false, 0, false, false);
            Camera.Position = Game.PlayerPed.Position + Game.PlayerPed.ForwardVector * 2f + new Vector3(0, 0, 0.2f);
            Camera.PointAt(Game.PlayerPed.Position);

            Screen.Hud.IsRadarVisible = false;

            while (!API.HasCollisionLoadedAroundEntity(Game.PlayerPed.Handle))
                await BaseScript.Delay(100);

            Screen.Fading.FadeIn(1000);
            while (Screen.Fading.IsFadingIn)
                await BaseScript.Delay(100);

            SetSelectionScreenTeamName(Data.Teams[SelectedTeam]);
            if (Data.Teams.Count < 2)
                ToggleSelectionButtons(false, false);
            else
                ToggleSelectionButtons(false, true);

            ToggleSelectionScreenNui(true);
        }

        private async Task SelectTeam(int team)
        {
            if (IsSwitchingTeam || IsAwaitingSpawn)
                return;

            IsSwitchingTeam = true;

            if (team < 0 || team >= Data.Teams.Count)
                return;

            SelectedTeam = team;

            API.SetPlayerTeam(Game.Player.Handle, SelectedTeam);

            while (!await Game.Player.ChangeModel(new Model((PedHash)Enum.Parse(typeof(PedHash), Data.Skins[SelectedTeam], true)))) await BaseScript.Delay(100);
            API.SetPedDefaultComponentVariation(Game.PlayerPed.Handle);

            Game.PlayerPed.Weapons.RemoveAll();
            Game.PlayerPed.Weapons.Give((WeaponHash)Enum.Parse(typeof(WeaponHash), Data.Weapons[SelectedTeam].Hash, true), 1, true, true);

            SetSelectionScreenTeamName(Data.Teams[SelectedTeam]);
            bool HasNext = SelectedTeam < (Data.Teams.Count - 1);
            bool HasPrev = SelectedTeam > 0;

            ToggleSelectionButtons(HasPrev, HasNext);
            IsSwitchingTeam = false;
        }

        private CallbackDelegate NUI_OnSpawnRequest(IDictionary<string, object> data, CallbackDelegate result)
        {
            NUI_OnSpawnRequestAsync();
            result("ok");
            return result;
        }

        private async Task NUI_OnSpawnRequestAsync()
        {
            if (!IsAwaitingSpawn)
            {
                IsAwaitingSpawn = true;
                bool response = await Rpc.Event(ClientEvents.JoinTeamRequest).Request<bool>(SelectedTeam);
                if(!response)
                {
                    Screen.ShowNotification("The team your are trying to join is full!");
                    IsAwaitingSpawn = false;
                }
                else
                {
                    ToggleSelectionScreenNui(false);
                    API.SetPlayerTeam(Game.Player.Handle, SelectedTeam);
                    API.NetworkSetInMpCutscene(false, false);
                    Screen.Hud.IsRadarVisible = true;             
                }                          
            }
        }

        private void ToggleSelectionScreenNui(bool toggle)
        {
            Serializer serializer = new Serializer();
            Dictionary<string, object> message = new Dictionary<string, object>()
            {
                { "type", "ToggleSelection"},
                { "toggle", toggle}
            };
            API.SendNuiMessage(serializer.Serialize(message));
            API.SetNuiFocus(toggle, toggle);
        }

        private void SetSelectionScreenTeamName(string name)
        {
            Serializer serializer = new Serializer();
            Dictionary<string, object> message = new Dictionary<string, object>()
            {
                { "type", "UpdateTeamName"},
                { "name", name}
            };
            API.SendNuiMessage(serializer.Serialize(message));
        }

        private void ToggleSelectionButtons(bool prev, bool next)
        {
            Serializer serializer = new Serializer();
            Dictionary<string, object> message = new Dictionary<string, object>()
            {
                { "type", "ToggleSelectionButtons"},
                { "togprev", prev},
                { "tognext", next}
            };
            API.SendNuiMessage(serializer.Serialize(message));
        }

        private CallbackDelegate NUI_OnNextTeamSelect(IDictionary<string, object> data, CallbackDelegate result)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SelectTeam(SelectedTeam + 1);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            result("ok");
            return result;
        }

        private CallbackDelegate NUI_OnPreviousTeamSelect(IDictionary<string, object> data, CallbackDelegate result)
        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            SelectTeam(SelectedTeam - 1);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            result("ok");
            return result;
        }
    }
}
