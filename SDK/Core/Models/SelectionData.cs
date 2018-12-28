using System.Collections.Generic;

namespace FGMM.SDK.Core.Models
{
    public class SelectionData
    {
        public Position Position { get; set; }
        public string Animation { get; set; }
        public List<string> Teams { get; set; }
        public List<Weapon> Weapons { get; set; }
        public List<string> Skins { get; set; }

        public SelectionData()
        {

        }
    }
}
