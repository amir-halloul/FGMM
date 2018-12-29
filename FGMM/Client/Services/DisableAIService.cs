using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FGMM.SDK.Client.Events;
using FGMM.SDK.Client.RPC;
using FGMM.SDK.Client.Services;
using FGMM.SDK.Core.Diagnostics;
using FGMM.Client.Models;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using FGMM.SDK.Core.RPC.Events;

namespace FGMM.Client.Services
{
    class DisableAIService : Service
    {
        public DisableAIService(ILogger logger, IEventManager events, IRpcHandler rpc, ITickManager tickManager) : base(logger, events, rpc, tickManager)
        {
            logger.Info("DisableAI Service started!");
            Rpc.Event(ClientEvents.CleanupPeds).On(OnPedCleanupRequest);
            TickManager.Attach(DisableAITick);
            TickManager.Attach(CleanupPedsTick);
        }

        private async Task CleanupPedsTick()
        {
            int pedCount = 0;
            PedsPool Peds = new PedsPool();
            foreach (Ped ped in Peds)
            {
                pedCount++;
                if (!ped.IsPlayer)
                    ped.Delete();
            }
            await Delay(1000);
        }

        private void OnPedCleanupRequest(IRpcEvent obj)
        {
            Logger.Debug("Cleaning up peds");
            PedsPool Peds = new PedsPool();
            foreach (Ped ped in Peds)
            {
                if (!ped.IsPlayer)
                    ped.Delete();
            }
        }

        private async Task DisableAITick()
        {
            API.SetVehicleDensityMultiplierThisFrame(0.0f);
            API.SetPedDensityMultiplierThisFrame(0.0f);
            API.SetRandomVehicleDensityMultiplierThisFrame(0.0f);
            API.SetParkedVehicleDensityMultiplierThisFrame(0.0f);
            API.SetScenarioPedDensityMultiplierThisFrame(0.0f, 0.0f);
            API.SetSomeVehicleDensityMultiplierThisFrame(0.0f);
            await Task.FromResult(0);
        }
    }
}
