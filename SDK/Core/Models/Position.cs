namespace FGMM.SDK.Core.Models
{
    public class Position
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float A { get; set; }

        public Position()
        {
            X = Y = Z = A = 0.0f;
        }
    }
}
