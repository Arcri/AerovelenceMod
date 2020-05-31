using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.Weapons.Ranged
{
    public class BounceBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 3;
            DisplayName.SetDefault("Power Cloud");
        }

        public override void SetDefaults()
        {
            projectile.width = 15;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.ranged = true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.townNPC)
            {
                return;
            }

            Vector2 newVelocity = -projectile.velocity;

            Projectile.NewProjectile(
                projectile.position.X,
                projectile.position.Y,
                newVelocity.X,
                newVelocity.Y,
                ModContent.ProjectileType<BounceBullet>(),
                damage,
                knockback,
                projectile.owner
            );

        }

        public override void AI()
        {
            base.AI();

            if (++projectile.frameCounter >= 3)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 3)
                {
                    projectile.frame = 0;
                }
            }
        }
    }
}