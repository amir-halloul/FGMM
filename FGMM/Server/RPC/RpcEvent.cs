using FGMM.SDK.Server.RPC;

namespace FGMM.Server.RPC
{
    public class RpcEvent : IRpcEvent
    {
        public string Event { get; }

        public IClient Client { get; }

        public RpcEvent(string @event, IClient client)
        {
            this.Event = @event;
            this.Client = client;
        }

        public void Reply(params object[] payloads)
        {
            this.Client.Event(this.Event).Trigger(payloads);
        }
    }
}
