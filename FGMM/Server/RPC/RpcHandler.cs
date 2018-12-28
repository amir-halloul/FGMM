using FGMM.SDK.Server.RPC;

namespace FGMM.Server.RPC
{
    public class RpcHandler : IRpcHandler
    {
        public IRpc Event(string @event) => RpcManager.Event(@event);
    }
}
