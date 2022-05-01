namespace AerovelenceMod.Common.ShieldSystem
{
    public class ExampleShield : ModShield
    {
        public override ShieldTypes ShieldType => ShieldTypes.Bubble;
        public override ShieldData BaseShieldData => new ShieldData(1000, 5, 5, 20, 100);
    }
}
