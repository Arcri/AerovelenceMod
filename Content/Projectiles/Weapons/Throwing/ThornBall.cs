using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

namespace AerovelenceMod.Content.Projectiles.Weapons.Throwing
{
    public class ThornBall : ModProjectile
    {
        int bounces;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn Ball");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;

        }

        public override void SetDefaults()
        {
            Projectile.width = 35;
            Projectile.height = 35;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 4;
            Projectile.tileCollide = true;
            bounces = 3;

        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.1f * Projectile.direction;
            Projectile.velocity.X *= 0.984f;
            Projectile.velocity.Y += 0.28f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);

            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

            Projectile.velocity.Y = -Projectile.oldVelocity.Y + bounces;

            Projectile.penetrate -= 1;
            bounces--;

            return bounces < 0;
        }

       
		public override bool PreDraw(ref Color lightColor) {
        {
            Texture2D texture = (Texture2D)TextureAssets.Projectile[Projectile.type];
            Rectangle rectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Color color = Color.Lerp(Color.Red, Color.Pink, 0.5f + (float)Math.Sin(MathHelper.ToRadians(Projectile.frame)) / 2f) * 0.5f;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Main.EntitySpriteDraw(texture, Projectile.oldPos[i] + Projectile.Size / 2f - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(rectangle), color, Projectile.oldRot[i], rectangle.Size() / 2f, 1f, Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }

            return (true);
        }
    }
}