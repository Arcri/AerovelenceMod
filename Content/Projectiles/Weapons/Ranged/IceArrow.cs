
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class IceArrow : ModProjectile
    {

        public override void SetStaticDefaults() => DisplayName.SetDefault("Ice Arrow");

        public override void SetDefaults()
        {
            projectile.aiStyle = 1;

            projectile.width = projectile.height = 18;

            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.friendly = true;
        }

        public override void AI()
        {
            projectile.velocity.X *= 0.983f;
            projectile.velocity.Y += 0.14f;
        }
    }
}
