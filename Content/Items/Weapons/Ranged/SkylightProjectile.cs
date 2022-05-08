using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;

namespace AerovelenceMod.Content.Items.Weapons.Ranged
{
    public class SkylightProjectile : ModProjectile
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
            Projectile.timeLeft = 7;
            Projectile.extraUpdates = 1;
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
                int num23 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 156, 0f, 0f, 0, default(Color), 1.6f);
                Main.dust[num23].position.X = Projectile.Center.X - num21;
                Main.dust[num23].position.Y = Projectile.Center.Y - num22;
                Dust dust3 = Main.dust[num23];
                dust3.velocity *= 0f;
                dust3.noGravity = true;
                Main.dust[num23].scale = 0.07f;
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
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item66, Projectile.position);
            int projectileCount = 5;
            for (int i = 0; i < projectileCount; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity, ProjectileType<SkylightChainProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
            }
        }
    }
}