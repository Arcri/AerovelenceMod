using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Throwing
{
    public class ThornBall : ModProjectile
    {
        int bounces;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn Ball");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;

        }

        public override void SetDefaults()
        {
            projectile.width = 35;
            projectile.height = 35;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 4;
            projectile.tileCollide = true;
            bounces = 3;

        }

        public override void AI()
        {
            projectile.rotation += projectile.velocity.Length() * 0.1f * projectile.direction;
            projectile.velocity.X *= 0.984f;
            projectile.velocity.Y += 0.28f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);

            Main.PlaySound(SoundID.Dig, projectile.position);

            projectile.velocity.Y = -projectile.oldVelocity.Y + bounces;

            projectile.penetrate -= 1;
            bounces--;

            return bounces < 0;
        }

       
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Color color = Color.Lerp(Color.Red, Color.Pink, 0.5f + (float)Math.Sin(MathHelper.ToRadians(projectile.frame)) / 2f) * 0.5f;
            for (int i = 0; i < projectile.oldPos.Length; i++)
            {
                Main.spriteBatch.Draw(texture, projectile.oldPos[i] + projectile.Size / 2f - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle), color, projectile.oldRot[i], rectangle.Size() / 2f, 1f, projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            }

            return (true);
        }
    }
}