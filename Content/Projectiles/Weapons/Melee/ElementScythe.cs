using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Melee
{
    public class ElementScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Element Blade");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        private Vector2 velocity = Vector2.Zero;

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.penetrate = 5;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.aiStyle = -1;
            projectile.scale = 0.8f;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width, Main.projectileTexture[projectile.type].Height);
            Texture2D texture2D = mod.GetTexture("Content/Projectiles/Weapons/Melee/ElementScythe");
            Vector2 origin = new Vector2(texture2D.Width / 2, texture2D.Height / 2);
            for (int k = 0; k < projectile.oldPos.Length; k++)
            {
                float scale = projectile.scale * (projectile.oldPos.Length - k) / projectile.oldPos.Length * 1.0f;
                Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + Main.projectileTexture[projectile.type].Size() / 3f;
                Color color = projectile.GetAlpha(FetchRainbow()) * ((projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
                for (int i = 0; i < 6; i++)
                {
                    if (i == 0)
                        spriteBatch.Draw(texture2D, drawPos, null, color, projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                    spriteBatch.Draw(texture2D, drawPos + new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), null, Color.White.MultiplyRGBA(color * 0.5f), projectile.rotation, origin, scale * 0.6f, SpriteEffects.None, 0f);
                }
            }
            return false;
        }

        Vector2 storedpos = new Vector2();

        public Color FetchRainbow()
        {
            float sin1 = (float)Math.Sin(MathHelper.ToRadians(projectile.ai[1]));
            float sin2 = (float)Math.Sin(MathHelper.ToRadians(projectile.ai[1] + 120));
            float sin3 = (float)Math.Sin(MathHelper.ToRadians(projectile.ai[1] + 240));
            int middle = 180;
            int length = 75;
            float r = middle + length * sin1;
            float g = middle + length * sin2;
            float b = middle + length * sin3;
            Color color = new Color((int)r, (int)g, (int)b);
            return color;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(projectile.localAI[0]);
            writer.Write(projectile.localAI[1]);
            writer.WriteVector2(velocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            projectile.localAI[0] = reader.ReadSingle();
            projectile.localAI[1] = reader.ReadSingle();
            velocity = reader.ReadVector2();
        }

        float angle = 0;
        int i = 0;
        bool once = true;
        float mouseangleAlpha;
        float direction;

        public override void AI()
        {
            projectile.ai[1] += 2f;
            Color rainbow = FetchRainbow();
            Lighting.AddLight(new Vector2(projectile.Center.X, projectile.Center.Y), rainbow.R / 255f, rainbow.G / 255f, rainbow.B / 255f);
            if (Main.rand.NextBool(10))
            {
                int num2 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 267);
                Dust dust = Main.dust[num2];
                Color color2 = new Color(110, 110, 110, 0).MultiplyRGBA(rainbow);
                dust.color = color2;
                dust.noGravity = true;
                dust.fadeIn = 0.1f;
                dust.scale *= 2f;
                dust.alpha = 255 - (int)(255 * (projectile.timeLeft / 720f));
                dust.velocity *= 0.5f;
                dust.velocity += projectile.velocity * 0.4f;
            }
            Vector2 mousepos = new Vector2(Main.MouseScreen.X - 960, Main.MouseScreen.Y - 506);
            float ipo = (float)Math.Sqrt(Math.Pow(mousepos.X, 2) + Math.Pow(mousepos.Y, 2));


            if (once)
            {
                once = false;
                mouseangleAlpha = (float)Math.Asin(mousepos.Y / ipo);
                direction = Main.player[Main.myPlayer].direction;
            }

            i += 45;

            angle += (float)Math.Abs((Math.PI * i / 180.0));
            storedpos.Y += (float)(3 * Math.Sin(angle));
            storedpos.X = 15 * direction;
            projectile.velocity = new Vector2((float)(storedpos.X * Math.Cos(mouseangleAlpha) - storedpos.Y * Math.Sin(mouseangleAlpha)) / 1.3f, (float)(storedpos.X * Math.Sin(mouseangleAlpha) + storedpos.Y * Math.Cos(mouseangleAlpha)) * direction / 1.3f);

            projectile.rotation = projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }
    }
}