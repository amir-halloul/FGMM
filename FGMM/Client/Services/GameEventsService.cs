using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FGMM.SDK.Client.Events;
using FGMM.SDK.Client.RPC;
using FGMM.SDK.Core.Diagnostics;
using FGMM.SDK.Client.Services;
using FGMM.SDK.Core.RPC.Events;
using FGMM.SDK.Client.Gamemodes;
using FGMM.SDK.Client.Diagnostics;
using FGMM.SDK.Core.Models;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace FGMM.Client.Services
{
    class GameEventsService : Service
    {
        private const int PlayerCount = 64;

        private bool IsDead;

        public GameEventsService(ILogger logger, IEventManager events, IRpcHandler rpc, ITickManager Tick) : base(logger, events, rpc)
        {
            IsDead = false;
            Tick.Attach(DeathTick);
        }

        private async Task DeathTick()
        {
            if(API.IsPedDeadOrDying(Game.PlayerPed.Handle, true) && !IsDead)
            {
                IsDead = true;
                Logger.Debug("Death event detected!");
                uint weapon = 0;
                int killerEntity = API.NetworkGetEntityKillerOfPlayer(Game.Player.Handle, ref weapon);              
                int killerType = API.GetEntityType(killerEntity);

                int killerId = -1;

                if (killerType == 1) // Killer is ped
                {
                    killerId = GetPlayerFromPedId(killerEntity);
                    if (killerId != -1)
                        killerId = API.GetPlayerServerId(killerId);
                }
                    
                int playerID = GetPlayerFromPedId(Game.PlayerPed.Handle);

                Rpc.Event(ClientEvents.PlayerDied).Trigger(killerId);
            }
            else if(!API.IsPedDeadOrDying(Game.PlayerPed.Handle, true))
            {
                IsDead = false;
            }
        }

        private int GetPlayerFromPedId(int id)
        {
            for (int i = 0; i < PlayerCount; i++)
            {
                if (API.NetworkIsPlayerActive(i) && (API.GetPlayerPed(i) == id))
                    return i;
            }
            return -1;
        }
    }
}
