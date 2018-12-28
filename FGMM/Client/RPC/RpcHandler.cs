using FGMM.SDK.Client.RPC;

namespace FGMM.Client.RPC
{
    public class RpcHandler : IRpcHandler
    {
        public IRpc Event(string @event) => RpcManager.Event(@event);
    }
}
