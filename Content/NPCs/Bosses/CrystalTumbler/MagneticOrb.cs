using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class MagneticOrb : ModProjectile
    {
        private const float Gravity = 0.3f;
        private const float WaterBuoyancy = -0.15f;
        private bool inWater = false;
        private float bobbingTime = 0f;
        private float bobbingSpeed = 0.1f;
        private float bobbingDecay = 0.99f;

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
        }

        public override void AI()
        {
            int waterLayerTile = ArenaData.WaterLayer;
            float waterLevelY = waterLayerTile * 16;
            Projectile.rotation += 0.2f;
            if (Projectile.timeLeft > 590)
            {
                Projectile.velocity.Y += 0.3f;
                return;
            }
            if (!inWater)
            {
                Projectile.velocity.Y += 0.3f;
                if (Projectile.velocity.Y > 6f)
                    Projectile.velocity.Y *= 0.98f;
                if (Projectile.Center.Y >= waterLevelY)
                {
                    inWater = true;
                    Projectile.velocity.Y *= -0.4f;
                }
            }

            if (inWater)
            {
                Projectile.velocity.Y += -0.2f;
                if (Projectile.velocity.Y > 0)
                    Projectile.velocity.Y *= 0.92f;
                if (Projectile.Center.Y < waterLevelY - 4f)
                {
                    Projectile.velocity.Y = 1.5f;
                }
                bobbingTime += 0.1f;
                float bobbingOffset = (float)Math.Sin(bobbingTime) * 1.5f;
                if (bobbingSpeed > 0.02f)
                    bobbingSpeed *= 0.99f;
                Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, bobbingOffset, 0.1f);
            }
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch);
                Main.dust[dust].scale = 1.2f;
                Main.dust[dust].velocity *= 0.2f;
                Main.dust[dust].noGravity = true;
            }

            if (Projectile.velocity.Y > 0)
            {

            }
                Main.NewText("fringus");
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < 8; i++)
            {
                Color col = i == 0 ? Color.SkyBlue with { A = 0 } : Color.DeepSkyBlue with { A = 0 };

                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, col * 1f, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Size() / 2, Projectile.scale * 1.1f, SpriteEffects.None, 0f);
            }
            return true;
        }
    }






    public class MagneticExplosion : ModProjectile
    {

        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.aiStyle = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 30;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.light = 0.8f;
            Projectile.scale = 1f;
            Projectile.alpha = 50;
        }

        public override void AI()
        {
            Projectile.scale *= 0.95f;
            Projectile.alpha += 5;
            int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 87);
            Main.dust[dust].velocity *= 0.5f;
        }
    }
}