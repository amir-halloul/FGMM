using FGMM.SDK.Core.RPC;
using FGMM.SDK.Server.RPC;
using FGMM.SDK.Server.Diagnostics;
using CitizenFX.Core;

namespace FGMM.Server.RPC
{
    public static class RpcManager
    {
        private static Logger logger;
        private static Serializer serializer;
        private static RpcTrigger trigger;
        private static ClientHandler handler;

        public static void Configure(EventHandlerDictionary events)
        {
            logger = new Logger("RPC");
            serializer = new Serializer();
            trigger = new RpcTrigger(logger, serializer);
            handler = new ClientHandler(events);
        }

        public static IRpc Event(string @event) => new Rpc(@event, logger, handler, trigger, serializer);
    }
}
