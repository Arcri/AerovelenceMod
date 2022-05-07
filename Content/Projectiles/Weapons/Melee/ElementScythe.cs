using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles.Weapons.Melee
{
    public class ElementScythe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Element Blade");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        private Vector2 velocity = Vector2.Zero;

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.penetrate = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.scale = 0.8f;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
        }

		public override bool PreDraw(ref Color lightColor)
        {
            // Vector2 drawOrigin = new Vector2((Texture2D)TextureAssets.Projectile[projectile.type].Width, (Texture2D)TextureAssets.Projectile[projectile.type].Height);
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("Content/Projectiles/Weapons/Melee/ElementScythe").Value;
            Vector2 origin = new Vector2(texture2D.Width / 2, texture2D.Height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - k) / Projectile.oldPos.Length * 1.0f;
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + TextureAssets.Projectile[Projectile.type].Size() / 3f;
                Color color = Projectile.GetAlpha(FetchRainbow()) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                for (int i = 0; i < 6; i++)
                {
                    if (i == 0)
                        Main.EntitySpriteDraw(texture2D, drawPos, null, color, Projectile.rotation, origin, scale, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(texture2D, drawPos + new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)), null, Color.White.MultiplyRGBA(color * 0.5f), Projectile.rotation, origin, scale * 0.6f, SpriteEffects.None, 0);
                }
            }
            return false;
        }

        Vector2 storedpos = new Vector2();

        public Color FetchRainbow()
        {
            float sin1 = (float)Math.Sin(MathHelper.ToRadians(Projectile.ai[1]));
            float sin2 = (float)Math.Sin(MathHelper.ToRadians(Projectile.ai[1] + 120));
            float sin3 = (float)Math.Sin(MathHelper.ToRadians(Projectile.ai[1] + 240));
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
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
            writer.WriteVector2(velocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
            velocity = reader.ReadVector2();
        }

        float angle = 0;
        int i = 0;
        bool once = true;
        float mouseangleAlpha;
        float direction;

        public override void AI()
        {
            Projectile.ai[1] += 2f;
            Color rainbow = FetchRainbow();
            Lighting.AddLight(new Vector2(Projectile.Center.X, Projectile.Center.Y), rainbow.R / 255f, rainbow.G / 255f, rainbow.B / 255f);
            if (Main.rand.NextBool(10))
            {
                int num2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 267);
                Dust dust = Main.dust[num2];
                Color color2 = new Color(110, 110, 110, 0).MultiplyRGBA(rainbow);
                dust.color = color2;
                dust.noGravity = true;
                dust.fadeIn = 0.1f;
                dust.scale *= 2f;
                dust.alpha = 255 - (int)(255 * (Projectile.timeLeft / 720f));
                dust.velocity *= 0.5f;
                dust.velocity += Projectile.velocity * 0.4f;
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
            Projectile.velocity = new Vector2((float)(storedpos.X * Math.Cos(mouseangleAlpha) - storedpos.Y * Math.Sin(mouseangleAlpha)) / 1.3f, (float)(storedpos.X * Math.Sin(mouseangleAlpha) + storedpos.Y * Math.Cos(mouseangleAlpha)) * direction / 1.3f);

            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
        }
    }
}