namespace FGMM.SDK.Client.RPC
{
    public interface IRpcHandler
    {
        IRpc Event(string @event);
    }
}
