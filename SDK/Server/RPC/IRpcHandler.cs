namespace FGMM.SDK.Server.RPC
{
    public interface IRpcHandler
    {
        IRpc Event(string @event);
    }
}
