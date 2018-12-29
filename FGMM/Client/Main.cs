using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FGMM.Client.Events;
using FGMM.Client.RPC;
using FGMM.SDK.Client.Diagnostics;
using FGMM.SDK.Core.RPC.Events;
using FGMM.Client.Services;
using CitizenFX.Core;

namespace FGMM.Client
{
    class Main : BaseScript
    {
        private Logger Logger { get; set; }
        private EventManager EventManager { get; set; }
        private RpcHandler Rpc { get; set; }
        private TickManager TickManager { get; set; }

        public Main()
        {        
            Logger = new Logger("CLIENT");
            Logger.Info("Initializing client.");

            RpcManager.Configure(EventHandlers);

            EventManager = new EventManager();
            Rpc = new RpcHandler();
            TickManager = new TickManager(c => this.Tick += c, c => this.Tick -= c);

            TeamSelectionService teamSelectionService = new TeamSelectionService(new Logger("TeamSelectionService"), EventManager, new RpcHandler(), TickManager, EventHandlers);
            GamemodeService gamemodeService = new GamemodeService(new Logger("GamemodeService"), EventManager, new RpcHandler(), TickManager);
            DisableAIService disableAIService = new DisableAIService(new Logger("DisableAIService"), EventManager, new RpcHandler(), TickManager);

            Logger.Info("Client Inizialized!");
            Rpc.Event(ClientEvents.Initialized).Trigger();     
        }

    }
}
