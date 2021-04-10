
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
            projectile.width = 50;
            projectile.height = 50;
            projectile.aiStyle = 0;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            projectile.velocity = Vector2.Zero;
            projectile.alpha += 10;
            if (projectile.alpha > 200)
            {
                projectile.friendly = false;
            }
            if (projectile.alpha >= 255)
            {
                projectile.active = false;
            }
            projectile.rotation += MathHelper.ToRadians(3);
        }
    }
}
