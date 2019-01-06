namespace FGMM.SDK.Core.RPC.Events
{
    public class ServerEvents
    {
        public const string Initialized = "fgmm:server:initialized";

        public const string EndMission = "fgmm:server:mission:end:request";

        public const string MissionStarted = "fgmmm:mission:started";
        public const string MissionEnded = "fgmmm:mission:ended";

        public const string StartRespawn = "fgmm:server:respawn";
    }
}
