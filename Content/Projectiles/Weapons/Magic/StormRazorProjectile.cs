using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class StormRazorProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Razor");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 15;
            projectile.timeLeft = 200;
            projectile.alpha = 100;
        }
        public override bool PreDraw(SpriteBatch sb, Color lightColor)
        {
            Vector2 vector = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Vector2 position = projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, projectile.gfxOffY);
                Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - i) / projectile.oldPos.Length);
                sb.Draw(Main.projectileTexture[projectile.type], position, null, color, projectile.rotation, vector, projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = mod.GetTexture("Projectiles/StormRazorProjectile_Glow");
            spriteBatch.Draw(
                texture,
                new Vector2
                (
                    projectile.Center.Y - Main.screenPosition.X,
                    projectile.Center.X - Main.screenPosition.Y
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                projectile.rotation,
                texture.Size(),
                projectile.scale,
                SpriteEffects.None,
                0f
            );
        }
        public override void AI()
        {
            projectile.rotation += projectile.velocity.Length() * 0.1f * projectile.direction;
            int dust = Dust.NewDust(projectile.position, projectile.width, projectile.height, 63, projectile.velocity.X, projectile.velocity.Y, 0, Color.White, 1);
            Main.dust[dust].velocity /= 1.2f;
            Main.dust[dust].noGravity = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 offset = new Vector2(0, 0);
            Main.PlaySound(SoundID.Item10);
            {
                projectile.penetrate--;
                if (projectile.penetrate <= 0)
                {
                    projectile.Kill();
                }
                else
                {
                    if (projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
                    {
                        projectile.velocity.X = oldVelocity.X * -2;
                    }
                    if (projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
                    {
                        projectile.velocity.Y = oldVelocity.Y * -2;
                    }
                    projectile.velocity *= 0.75f;
                    Main.PlaySound(SoundID.Item10, projectile.position);
                }
                for (float i = 0; i < 360; i += 0.5f)
                {
                    float ang = (float)(i * Math.PI) / 180;
                    float x = (float)(Math.Cos(ang) * 15) + projectile.Center.X;
                    float y = (float)(Math.Sin(ang) * 15) + projectile.Center.Y;
                    Vector2 vel = Vector2.Normalize(new Vector2(x - projectile.Center.X, y - projectile.Center.Y)) * 7;
                    int dustIndex = Dust.NewDust(new Vector2(x - 3, y - 3), 6, 6, 63, vel.X, vel.Y);
                    Main.dust[dustIndex].noGravity = true;
                }
                return false;
            }
        }
    }
}