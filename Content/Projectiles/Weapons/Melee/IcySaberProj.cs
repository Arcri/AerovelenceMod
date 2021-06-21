
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Melee
{
    public class IcySaberProj : ModProjectile
    {

        public override string Texture => "Terraria/Projectile_" + ProjectileID.None;

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 8;

            projectile.aiStyle = -1;
            projectile.friendly = projectile.melee = projectile.tileCollide = false;

            projectile.penetrate = 4;

            projectile.timeLeft = 180;
        }

        int Timer = 0;
        public override void AI()
        {
            projectile.velocity *= 1.003f;

            for (int j = 0; j < 80; j++)
            {
                float x = projectile.position.X - projectile.velocity.X / 10f * (float)j;
                float y = projectile.position.Y - projectile.velocity.Y / 10f * (float)j;
                Dust dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 20, 0, 0, 0, Color.Blue, 0.9f);
                dust.position.X = x;
                dust.position.Y = y;
                dust.velocity *= 0f;
                dust.noGravity = true;
            }
            if (++projectile.localAI[1] > 10)
            {
                float amountOfDust = 16f;
                for (int i = 0; i < amountOfDust; ++i)
                {
                    Vector2 spinningpoint5 = -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / amountOfDust)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(projectile.velocity.ToRotation());

                    Dust dust = Dust.NewDustPerfect(projectile.Center + spinningpoint5, 20, spinningpoint5, 0, Color.Blue, 1.3f);
                    dust.noGravity = true;
                }

                projectile.localAI[1] = 0;
            }
        }      
    }
}
