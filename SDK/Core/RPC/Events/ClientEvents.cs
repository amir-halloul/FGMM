namespace FGMM.SDK.Core.RPC.Events
{
    public class ClientEvents
    {
        public const string PlayerConnecting = "playerConnecting";
        public const string PlayerDropped = "playerDropped";     

        public const string ResourceStarted = "onClientResourceStart";
        public const string ResourceStopped = "onClientResourceStop";

        public const string Initialized = "fgmm:client:initialized";

        public const string RequestGamemode = "fgmm:client:gamemode:request";

        public const string RequestTeamSelection = "fgmm:client:selection:request";
        public const string StartTeamSelection = "fgmm:client:selection:start";

        public const string JoinTeamRequest = "fgmm:client:team:join:request";

        public const string PlayerDied = "fgmm:client:player:death";

        public const string CleanupPeds = "fgmm:client:peds:cleanup";
    }
}
