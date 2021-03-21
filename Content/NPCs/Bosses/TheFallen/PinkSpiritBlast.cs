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
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = false;
            projectile.hostile = true;
            projectile.extraUpdates = 2;
            projectile.scale = 1f;
            projectile.timeLeft = 600;
            projectile.magic = true;
            projectile.ignoreWater = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void AI()
        {
            projectile.velocity *= 1.01f;
            int num294 = Main.rand.Next(3, 7);
            for (int num295 = 0; num295 < num294; num295++)
            {
                counter++;
                if (counter >= 17)
                {
                    int num296 = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, DustID.FlameBurst, 0f, 0f, 100, default, 2.1f);
                    Dust dust105 = Main.dust[num296];
                    Dust dust2 = dust105;
                    dust2.velocity *= 2f;
                    Main.dust[num296].noGravity = true;
                }
            }
            if (projectile.ai[1] != 1f)
            {
                projectile.ai[1] = 1f;
                projectile.position += projectile.velocity;
                projectile.velocity = projectile.velocity;
            }
        }
    }
}