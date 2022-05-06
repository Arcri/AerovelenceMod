using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.TheFallen
{
    public class PinkSpiritBlast : ModProjectile
    {
        public int i;
        public int counter = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Electric Laser");
        }
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.extraUpdates = 2;
            Projectile.scale = 1f;
            Projectile.timeLeft = 600;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void AI()
        {
            Projectile.velocity *= 1.01f;
            int num294 = Main.rand.Next(3, 7);
            for (int num295 = 0; num295 < num294; num295++)
            {
                counter++;
                if (counter >= 17)
                {
                    int num296 = Dust.NewDust(Projectile.Center - Projectile.velocity / 2f, 0, 0, DustID.FlameBurst, 0f, 0f, 100, default, 2.1f);
                    Dust dust105 = Main.dust[num296];
                    Dust dust2 = dust105;
                    dust2.velocity *= 2f;
                    Main.dust[num296].noGravity = true;
                }
            }
            if (Projectile.ai[1] != 1f)
            {
                Projectile.ai[1] = 1f;
                Projectile.position += Projectile.velocity;
                Projectile.velocity = Projectile.velocity;
            }
        }
    }
}