namespace FGMM.SDK.Core.Models
{
    public class Weapon
    {
        public string Hash { get; set; }
        public uint Ammo { get; set; }

        public Weapon()
        {
        }

        public Weapon(string hash, uint ammo)
        {
            Hash = hash;
            Ammo = ammo;
        }
    }
}
