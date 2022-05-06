
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Throwing
{
    public class BlossomProjectile : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blossom Projectile");
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.aiStyle = 0;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.velocity = Vector2.Zero;
            Projectile.alpha += 10;
            if (Projectile.alpha > 200)
            {
                Projectile.friendly = false;
            }
            if (Projectile.alpha >= 255)
            {
                Projectile.active = false;
            }
            Projectile.rotation += MathHelper.ToRadians(3);
        }
    }
}
