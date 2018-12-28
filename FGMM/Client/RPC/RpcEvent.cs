using FGMM.SDK.Client.RPC;

namespace FGMM.Client.RPC
{
    public class RpcEvent : IRpcEvent
    {
        public string Event { get; set; }

        public void Reply(params object[] payloads)
        {
            // TODO
        }
    }
}
