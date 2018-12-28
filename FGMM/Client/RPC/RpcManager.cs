using FGMM.SDK.Client.Diagnostics;
using FGMM.SDK.Core.RPC;
using CitizenFX.Core;
using FGMM.SDK.Client.RPC;

namespace FGMM.Client.RPC
{
    public static class RpcManager
    {
        private static readonly Logger Logger;
        private static readonly Serializer Serializer;
        private static readonly RpcTrigger Trigger;
        private static ServerHandler handler;

        static RpcManager()
        {
            Logger = new Logger("RPC");
            Serializer = new Serializer();
            Trigger = new RpcTrigger(Logger, Serializer);
        }

        public static void Configure(EventHandlerDictionary events)
        {
            handler = new ServerHandler(events);
        }

        public static IRpc Event(string @event) => new Rpc(@event, Logger, handler, Trigger, Serializer);
    }
}
