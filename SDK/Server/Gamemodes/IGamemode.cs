using System;
using FGMM.SDK.Server.Models;
using FGMM.SDK.Server.RPC;
using CitizenFX.Core;


namespace FGMM.SDK.Server.Gamemodes
{
    public interface IGamemode
    {
        IMission Mission { get; set;}

        int RemaingingTime { get; set; }
        
        void Start(string mission);

        void Stop();

        void HandleDeath(Player player, Player killer);

        bool HandleTeamJoinRequest(Player player, int team);
      
        void HandleDisconnect(Player player);
    }
}
