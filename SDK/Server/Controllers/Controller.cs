using FGMM.SDK.Core.Diagnostics;
using FGMM.SDK.Server.Events;
using FGMM.SDK.Server.RPC;

namespace FGMM.SDK.Server.Controllers
{
    public class Controller
    {
        protected ILogger Logger { get; set; }
        protected IEventManager Events { get; set; }
        protected IRpcHandler Rpc { get; set; }

        public Controller(ILogger logger, IEventManager events, IRpcHandler rpc)
        {
            Logger = logger;
            Events = events;
            Rpc = rpc;
        }
        
    }
}
