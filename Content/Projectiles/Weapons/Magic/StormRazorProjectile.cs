using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class StormRazorProjectile : ModProjectile
    {
        public float maxVelocity = 18f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Razor");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 15;
            Projectile.timeLeft = 200;
            Projectile.alpha = 100;
        }
        public override bool PreDraw(ref Color lightColor)
        {
        Vector2 vector = new Vector2(TextureAssets.Projectile[Projectile.type].Width() * 0.5f, Projectile.height * 0.5f);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 position = Projectile.oldPos[i] - Main.screenPosition + vector + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - i) / Projectile.oldPos.Length);
                Main.spriteBatch.Draw((Texture2D)TextureAssets.Projectile[Projectile.type], position, null, color, Projectile.rotation, vector, Projectile.scale, SpriteEffects.None, 0f);
            }
            return true;
        }
        public override void PostDraw(Color lightColor)
        {
        Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Projectiles/Weapons/Magic/StormRazorProjectile_Glow");
            Main.EntitySpriteDraw(
                texture,
                new Vector2
                (
                    Projectile.Center.Y - Main.screenPosition.X,
                    Projectile.Center.X - Main.screenPosition.Y
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                Projectile.rotation,
                texture.Size(),
                Projectile.scale,
                SpriteEffects.None,
                0
            );
        }
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.Length() * 0.1f * Projectile.direction;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 63, Projectile.velocity.X, Projectile.velocity.Y, 0, Color.White, 1);
            Main.dust[dust].velocity /= 1.2f;
            Main.dust[dust].noGravity = true;
            if (Projectile.ai[0] > 0) Projectile.ai[0]--;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 offset = new Vector2(0, 0);
            SoundEngine.PlaySound(SoundID.Item10);
            {
                Projectile.penetrate--;
                if (Projectile.penetrate <= 0)
                {
                    Projectile.Kill();
                }
                else
                {
                    if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
                    {
                        Projectile.velocity.X = oldVelocity.X * -2;
                    }
                    if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
                    {
                        Projectile.velocity.Y = oldVelocity.Y * -2;
                    }
                    Projectile.velocity *= 0.75f;
                    if (Projectile.velocity.Length() > maxVelocity)
                    {
                        Projectile.velocity = Projectile.velocity.SafeNormalize(Projectile.velocity) * maxVelocity;
                    }
                    SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
                    //Kinda bandaid fix for projectile having an aneurysm on confined spaces, maybe someone finds a better fix ~Exitium
                    if (Projectile.ai[0] > 0)
                    {
                        Projectile.velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(360));
                    }
                    Projectile.ai[0] = 5;
                }
                for (float i = 0; i < 360; i += 0.5f)
                {
                    float ang = (float)(i * Math.PI) / 180;
                    float x = (float)(Math.Cos(ang) * 15) + Projectile.Center.X;
                    float y = (float)(Math.Sin(ang) * 15) + Projectile.Center.Y;
                    Vector2 vel = Vector2.Normalize(new Vector2(x - Projectile.Center.X, y - Projectile.Center.Y)) * 7;
                    int dustIndex = Dust.NewDust(new Vector2(x - 3, y - 3), 6, 6, 63, vel.X, vel.Y);
                    Main.dust[dustIndex].noGravity = true;
                }
                return false;
            }
        }
    }
}