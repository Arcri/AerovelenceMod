using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.NPCs.Bosses.VoidReaver
{
    public class TimeSawblade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Timeblade");

        }

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.aiStyle = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 999;
            Projectile.timeLeft = 600;
            Projectile.alpha = 100;
        }
    }
}