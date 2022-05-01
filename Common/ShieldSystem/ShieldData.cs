namespace AerovelenceMod.Common.ShieldSystem
{
    /// <summary>
    /// Represents the data of a given shield.
    /// </summary>
    public class ShieldData
    {
        /// <summary>How much HP of damage this shield can take before failing.</summary>
        public int Capacity;

        /// <summary>How long it takes for this shield to start regenerating outside of combat in seconds.</summary>
        public float Delay;

        /// <summary>How long it takes this shield to fully regenerate in seconds.</summary>
        public int RechargeRate;

        /// <summary>How much HP this shield takes off of the player.</summary>
        public int HPPenalty;

        /// <summary>Radius of the shield in pixels. Allows for different sizes of shields if necessary.</summary>
        public int Radius;

        public ShieldData(int cap, float del, int recharge, int hpPen, int rad)
        {
            Capacity = cap;
            Delay = del;
            RechargeRate = recharge;
            HPPenalty = hpPen;
            Radius = rad;
        }

        public static ShieldData operator +(ShieldData a, ShieldData b) => new ShieldData(a.Capacity + b.Capacity, a.Delay + b.Delay, a.RechargeRate + b.RechargeRate, a.HPPenalty + b.HPPenalty, a.Radius + b.Radius);
        public static ShieldData operator -(ShieldData a, ShieldData b) => new ShieldData(a.Capacity - b.Capacity, a.Delay - b.Delay, a.RechargeRate - b.RechargeRate, a.HPPenalty - b.HPPenalty, a.Radius - b.Radius);
    }
}
