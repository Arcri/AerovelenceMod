
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Ranged
{
    public class WispProjectileRanged : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 17;
            projectile.height = 18;
            projectile.aiStyle = -1;
            projectile.tileCollide = false;
            projectile.ranged = projectile.friendly = true;
            projectile.timeLeft = 180;
            
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            Texture2D texture2D = mod.GetTexture("Assets/Glow");
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                float scale = projectile.scale * (projectile.oldPos.Length - k) / projectile.oldPos.Length * .45f;
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + Main.projectileTexture[projectile.type].Size() / 3f;
                Color color = projectile.GetAlpha(Color.Black) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);

                spriteBatch.Draw(texture2D, drawPos, null, color, projectile.rotation, Main.projectileTexture[projectile.type].Size(), scale, SpriteEffects.None, 0f);
            }

            return true;
        }

        int radians = 16;
        int Timer = 0;
        private bool spawned;
        public override void AI()
        {

            projectile.Opacity = MathHelper.Clamp(projectile.timeLeft / 20f, 0f, 1f);

            if (!spawned)
            {
                spawned = true;
                Main.PlaySound(SoundID.Item104);
            }
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.velocity *= 1.03f;

            if (Timer < 60)
                projectile.velocity /= 1.03f;


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
    }
}
