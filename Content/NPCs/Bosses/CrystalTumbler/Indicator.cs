using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AerovelenceMod.Common.Utilities;
using System;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class Indicator : ModProjectile
    {
        private float reticleProgress = 0f;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 1;
        }

        public override void AI()
        {
            Projectile.alpha += 2;
            if (Projectile.alpha < 0) Projectile.alpha = 0;
            reticleProgress += 1.05f;
            reticleProgress = MathHelper.Clamp(reticleProgress, 0f, 1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = MathHelper.Lerp(0f, 1f, Easings.easeInQuad(reticleProgress * 1.15f)) * (1f - Projectile.alpha / 255f);
            Texture2D textureRed = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/RedOuterExclam").Value;
            Texture2D textureWhite = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/WhiteInnerExclam").Value;
            Vector2 origin = new(textureRed.Width / 2, textureRed.Height / 2);
            Vector2 position = Projectile.Center - Main.screenPosition;
            Color col = Color.Lerp(Color.White, Color.Red, 1f);
            Main.spriteBatch.Draw(textureRed, position, null, Color.White with { A = 0 } * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(textureWhite, position, null, col * opacity, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}