using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class SkylightChainProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Skylight");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 40;
            Projectile.extraUpdates = 5;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
        }
        int radians = 16;
        public override void AI()
        {
            int num3;
            for (int num20 = 0; num20 < 1; num20 = num3 + 1)
            {
                float num21 = Projectile.velocity.X / 4f * (float)num20;
                float num22 = Projectile.velocity.Y / 4f * (float)num20;
                int num23 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 156, 0f, 0f, 0, default, 1.6f);
                Main.dust[num23].position.X = Projectile.Center.X - num21;
                Main.dust[num23].position.Y = Projectile.Center.Y - num22;
                Dust dust3 = Main.dust[num23];
                dust3.velocity *= 0f;
                dust3.noGravity = true;
                Main.dust[num23].scale = 1f;
                num3 = num20;
            }

            Projectile.ai[0] += 1;

            if (Projectile.ai[0] >= 8)
            {
                Vector2 perturbedSpeed = new Vector2(Projectile.velocity.X, Projectile.velocity.Y).RotatedByRandom(MathHelper.ToRadians(radians));
                if (radians >= 16)
                {
                    radians = -24;
                }
                if (radians <= -16)
                {
                    radians = 16;
                }
                float scale = 1f - (Main.rand.NextFloat() * .3f);
                perturbedSpeed = perturbedSpeed * scale;
                Projectile.velocity.Y = perturbedSpeed.Y;
                Projectile.velocity.X = perturbedSpeed.X;
                Projectile.ai[0] = 0;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[Projectile.owner] = 5; 
            //this allows some immunity frames to be skipped, 0 is a bad idea it basically does damage every tick, and it lags
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item66, Projectile.position);
            int num3;
            int num570 = 6;
            for (int num571 = 0; num571 < num570; num571 = num3 + 1)
            {
                int num572 = Dust.NewDust(Projectile.Center, 0, 0, 156, 0f, 0f, 226, default, 1f);
                Dust dust = Main.dust[num572];
                dust.velocity *= 1.6f;
                Dust dust56 = Main.dust[num572];
                dust56.velocity.Y = dust56.velocity.Y - 1f;
                dust = Main.dust[num572];
                dust.velocity += -Projectile.velocity * (Main.rand.NextFloat() * 2f - 1f) * 0.5f;
                Main.dust[num572].scale = 1f;
                Main.dust[num572].fadeIn = 0.5f;
                Main.dust[num572].noGravity = true;
                num3 = num571;
            }
        }
    }
}