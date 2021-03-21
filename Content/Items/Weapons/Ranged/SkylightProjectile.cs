using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

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
            projectile.width = 16;
            projectile.height = 16;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.timeLeft = 7;
            projectile.extraUpdates = 1;
            projectile.friendly = true;
            projectile.magic = true;
        }
        int radians = 16;
        public override void AI()
        {
            int num3;
            for (int num20 = 0; num20 < 1; num20 = num3 + 1)
            {
                float num21 = projectile.velocity.X / 4f * (float)num20;
                float num22 = projectile.velocity.Y / 4f * (float)num20;
                int num23 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 156, 0f, 0f, 0, default(Color), 1.6f);
                Main.dust[num23].position.X = projectile.Center.X - num21;
                Main.dust[num23].position.Y = projectile.Center.Y - num22;
                Dust dust3 = Main.dust[num23];
                dust3.velocity *= 0f;
                dust3.noGravity = true;
                Main.dust[num23].scale = 0.07f;
                num3 = num20;
            }

            projectile.ai[0] += 1;

            if (projectile.ai[0] >= 8)
            {
                Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedByRandom(MathHelper.ToRadians(radians));
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
                projectile.velocity.Y = perturbedSpeed.Y;
                projectile.velocity.X = perturbedSpeed.X;
                projectile.ai[0] = 0;
            }
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item66, projectile.position);
            int projectileCount = 5;
            for (int i = 0; i < projectileCount; i++)
            {
                Projectile.NewProjectile(projectile.position, projectile.velocity, ProjectileType<SkylightChainProjectile>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
        }
    }
}