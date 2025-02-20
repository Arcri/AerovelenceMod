using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class MagneticOrb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f;
            if (Main.rand.NextBool(10))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87);
                Main.dust[dust].velocity *= 0.3f;
            }
        }
    }

    public class MagneticExplosion : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.8f;
            Projectile.scale = 1f;
            Projectile.alpha = 50;
        }

        public override void AI()
        {
            Projectile.scale *= 0.95f;
            Projectile.alpha += 5;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87);
            Main.dust[dust].velocity *= 0.5f;
        }
    }
}