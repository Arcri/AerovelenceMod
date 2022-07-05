
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Melee
{
    public class IcySaberProj : ModProjectile
    {

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.None;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;

            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;

            Projectile.penetrate = 4;

            Projectile.timeLeft = 180;
        }

        int Timer = 0;
        public override void AI()
        {
            Projectile.velocity *= 1.003f;

            for (int j = 0; j < 80; j++)
            {
                float x = Projectile.position.X - Projectile.velocity.X / 10f * (float)j;
                float y = Projectile.position.Y - Projectile.velocity.Y / 10f * (float)j;
                Dust dust = Dust.NewDustDirect(new Vector2(x, y), 1, 1, 20, 0, 0, 0, Color.Blue, 0.9f);
                dust.position.X = x;
                dust.position.Y = y;
                dust.velocity *= 0f;
                dust.noGravity = true;
            }
            if (++Projectile.localAI[1] > 10)
            {
                float amountOfDust = 16f;
                for (int i = 0; i < amountOfDust; ++i)
                {
                    Vector2 spinningpoint5 = -Vector2.UnitY.RotatedBy(i * (MathHelper.TwoPi / amountOfDust)) * new Vector2(1f, 4f);
                    spinningpoint5 = spinningpoint5.RotatedBy(Projectile.velocity.ToRotation());

                    Dust dust = Dust.NewDustPerfect(Projectile.Center + spinningpoint5, 20, spinningpoint5, 0, Color.Blue, 1.3f);
                    dust.noGravity = true;
                }

                Projectile.localAI[1] = 0;
            }
        }      
    }
}
