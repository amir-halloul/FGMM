using FGMM.SDK.Server.Diagnostics;
using FGMM.Server.Events;
using FGMM.SDK.Core.RPC.Events;
using FGMM.Server.Controllers;
using FGMM.Server.RPC;
using CitizenFX.Core;

namespace FGMM.Server
{
    class Main : BaseScript
    {
        private Logger Logger { get; set; }
        private EventManager Events { get; set; }

        public Main()
        {
            Logger = new Logger("SERVER");
            Logger.Info("Initializing server...");

            RpcManager.Configure(EventHandlers);

            Events = new EventManager();

            GamemodeController gamemodeController = new GamemodeController(new Logger("GamemodeController"), Events, new RpcHandler());
            MissionController missionController = new MissionController(new Logger("MissionController"), Events, new RpcHandler());
            
            Logger.Info("Server Initialized!");

            Events.Raise(ServerEvents.Initialized);
        }
    }
}
