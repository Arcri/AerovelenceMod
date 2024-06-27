using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;
using System.Collections.Generic;
using Steamworks;

namespace AerovelenceMod.Content.Items.Accessories.Boss
{


    public class CyverMasterAccessoryVFX : ModProjectile
    {

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;

        }

        public override bool? CanDamage() { return false; }
        public override bool? CanCutTiles() { return false; }


        int timer = 0;
        public override void AI()
        {
            Projectile.Center = Main.player[Projectile.owner].Center;

            //starAlpha
            if (timer < 10)
            {
                spikeAlpha = Math.Clamp(MathHelper.Lerp(spikeAlpha, 1.5f, 0.1f), 0, 1f);
                starAlpha = Math.Clamp(MathHelper.Lerp(starAlpha, 1.3f, 0.1f), 0, 1f);



            }
            if (timer >= 15)
            {
                starScale = Math.Clamp(MathHelper.Lerp(starScale, -0.2f, 0.06f), 0, 1f);
                spikeScale = Math.Clamp(MathHelper.Lerp(spikeScale, -0.3f, 0.06f), 0, 1f);

                spikeAlpha = Math.Clamp(MathHelper.Lerp(spikeAlpha, -0.5f, 0.14f), 0, 1f);

            }



            starRot += 0.08f * (starRotDir ? -1 : 1); 

            timer++;
        }


        public float starRot = Main.rand.NextFloat(6.28f);
        public bool starRotDir = Main.rand.NextBool();
        public float starScale = 1f;
        float starAlpha = 0f;

        float spikeAlpha = 0f;
        float spikeScale = 1f;

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D Spike = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/MuzzleFlashes/muzzle_flash_13pixel");
            Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/flare_1");

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 spikeOrigin = new Vector2(0, Spike.Height / 2);
            Vector2 vec2Scale = new Vector2(1f, 0.5f) * spikeScale * 2f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);


            Main.spriteBatch.Draw(Spike, drawPos, null, Color.HotPink with { A = 0 } * spikeAlpha, Projectile.rotation, spikeOrigin, 1f * vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Spike, drawPos, null, Color.White with { A = 0 } * 0.5f * spikeAlpha, Projectile.rotation, spikeOrigin, 0.4f * vec2Scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Spike, drawPos, null, Color.HotPink with { A = 0 } * spikeAlpha, Projectile.rotation + MathF.PI, spikeOrigin, 1f * vec2Scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Spike, drawPos, null, Color.White with { A = 0 } * spikeAlpha * 0.5f, Projectile.rotation + MathF.PI, spikeOrigin, 0.4f * vec2Scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Star, drawPos, null, Color.Black * starAlpha * 0.2f, starRot, Star.Size() / 2, starScale * 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, drawPos, null, Color.Black * starAlpha * 0.2f, starRot * -1, Star.Size() / 2, starScale * 1f, SpriteEffects.None, 0f);

            //Star
            for (int i = 0; i < 1; i++)
            {
                Main.spriteBatch.Draw(Star, drawPos, null, Color.HotPink with { A = 0 } * starAlpha, starRot, Star.Size() / 2, starScale * 0.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Star, drawPos, null, Color.HotPink with { A = 0 } * 0.4f * starAlpha, starRot * -1, Star.Size() / 2, starScale * 0.75f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Star, drawPos, null, Color.White with { A = 0 } * starAlpha, starRot * -1, Star.Size() / 2, starScale * 0.25f, SpriteEffects.None, 0f);

            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, default, default, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
