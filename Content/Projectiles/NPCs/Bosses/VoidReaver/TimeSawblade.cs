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
            projectile.width = 200;
            projectile.height = 200;
            projectile.aiStyle = 18;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 999;
            projectile.timeLeft = 600;
            projectile.alpha = 100;
        }
    }
}