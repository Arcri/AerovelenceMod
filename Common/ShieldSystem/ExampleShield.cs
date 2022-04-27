namespace AerovelenceMod.Common.ShieldSystem
{
    public class ExampleShield : ModShield
    {
        public override ShieldTypes ShieldType => ShieldTypes.Impact;
        public override int MaxCapacity => 1000;
        public override float Delay => 5;
        public override int RechargeRate => 5;
        public override int HPPenalty => 20;
        public override int Radius => 100;
    }
}
