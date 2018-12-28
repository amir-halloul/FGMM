using FGMM.SDK.Core.Models;

namespace FGMM.SDK.Server.Models
{
    public interface IMission
    {
        string Name { get; set; }

        string Gamemode { get; set; }

        int Duration { get; set; }

        SelectionData SelectionData { get; set; }
    }
}
