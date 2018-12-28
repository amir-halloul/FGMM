namespace FGMM.SDK.Client.RPC
{
    public interface IRpcEvent
    {
        string Event { get; }
        void Reply(params object[] payloads);
    }
}
