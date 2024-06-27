using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Graphics;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Utilities;
using static Humanizer.In;
using System.Linq;

namespace AerovelenceMod.Content.Projectiles.TempVFX
{
    public class SigilForBlast : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 370;

        }


        float fadeVal = 30f;
        float intensity = 1f;
        float rotation = 0f;
        float progress = 0f;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];


            if (timer > 20)
            {
                float prog = Math.Clamp((timer - 20f) / 60f, 0f, 1f);
                //progress = Math.Clamp((timer - 20f) / 30f, 0f, 1f);
                progress = Easings.easeInOutSine(prog);

                fadeVal = MathHelper.Lerp(30f, 0.2f, Easings.easeInOutSine(progress));

            }
            fadeVal = 0.2f;

            timer++;
        }

        Effect myEffect1 = null;
        Effect myEffect2 = null;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D MagicCircle = Mod.Assets.Request<Texture2D>("Assets/Orbs/whiteFireEyeA").Value;
            Texture2D ExtraGlow = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/bright_star").Value;

            float sin1 = MathF.Sin((float)Main.timeForVisualEffects * 0.04f);
            float sin2 = MathF.Cos((float)Main.timeForVisualEffects * 0.06f);

            Vector2 stretchScale = new Vector2(Math.Clamp(MathHelper.Lerp(1f, 0.2f, progress), 0.3f, 1f), 1f) * Projectile.scale * 1f;

            if (myEffect1 == null)
                myEffect1 = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/Sigil", AssetRequestMode.ImmediateLoad).Value;
            if (myEffect2 == null)
                myEffect2 = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/Sigil", AssetRequestMode.ImmediateLoad).Value;

            myEffect2 = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/Sigil", AssetRequestMode.ImmediateLoad).Value;
            myEffect2.Parameters["rotation"].SetValue((float)Main.timeForVisualEffects * 0.05f);
            myEffect2.Parameters["inputColor"].SetValue(new Color(255, 60, 5).ToVector3());
            myEffect2.Parameters["intensity"].SetValue(3f);
            myEffect2.Parameters["fadeStrength"].SetValue(fadeVal);
            myEffect2.Parameters["glowThreshold"].SetValue(0.7f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect2, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(MagicCircle, Projectile.Center - Main.screenPosition, null, Color.Orange, Projectile.rotation, MagicCircle.Size() / 2, stretchScale * 1f, 0f);

            Main.EntitySpriteDraw(ExtraGlow, Projectile.Center - Main.screenPosition, null, Color.Orange, Projectile.rotation, ExtraGlow.Size() / 2, stretchScale * 1f, 0f);
            Main.EntitySpriteDraw(ExtraGlow, Projectile.Center - Main.screenPosition, null, Color.Orange, Projectile.rotation, ExtraGlow.Size() / 2, stretchScale * 1f, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }
    }


    public class BigShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int mainTimer = 0;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 370;

            Projectile.extraUpdates = 2;
        }

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (mainTimer == 0)
                Projectile.ai[0] = Main.rand.NextBool() ? -1 : 1;

            if (mainTimer == 7)
                Projectile.ai[0] *= -1;

            #region trailInfo
            //Trail1 
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/FireEdge").Value;
            //trail1.trailColor = Color.DodgerBlue;
            trail1.trailPointLimit = 300;
            trail1.trailWidth = 30;
            trail1.trailMaxLength = 700;
            trail1.timesToDraw = 10;
            trail1.pinch = true;
            trail1.pinchAmount = 0.4f;

            //trail1.trailTime = mainTimer * 0.03f;
            trail1.trailRot = Projectile.velocity.ToRotation();

            trail1.trailPos = Projectile.Center + Projectile.velocity;
            trail1.TrailLogic();

            trail1.gradient = true;
            trail1.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad2").Value;
            trail1.shouldScrollColor = true;
            trail1.gradientTime = (float)Main.timeForVisualEffects * 0.03f;

            //Trail2
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
            trail2.trailColor = Color.White;
            trail2.trailPointLimit = 300;
            trail2.trailWidth = 100;
            trail2.trailMaxLength = 500;
            trail2.timesToDraw = 2;
            trail2.pinch = true;
            trail2.pinchAmount = 0.4f;

            trail2.trailTime = mainTimer * 0.02f;
            trail2.trailRot = Projectile.velocity.ToRotation();

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad2").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = ((float)Main.timeForVisualEffects * 0.02f) + 0.3f;

            trail2.trailPos = Projectile.Center + Projectile.velocity;
            trail2.TrailLogic();

            #endregion

            if (Projectile.direction == 1)
            {
                Projectile.rotation += 0.1f;
            }
            else
            {
                Projectile.rotation -= 0.1f;
            }

            if (mainTimer < 105)
                Projectile.velocity = Projectile.velocity.RotatedBy(0.03f * Projectile.ai[0]);
            else if (mainTimer < 125)
                Projectile.velocity = Projectile.velocity.RotatedBy(-0.01f * Projectile.ai[0]);

            if (mainTimer > 25)
            {
                Projectile.velocity *= 0.99f;

            }

            if (Projectile.timeLeft < 210)
                Projectile.active = false;

            mainTimer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/FireSpike").Value;
            Texture2D glow2 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/bright_star").Value;

            float rot = (float)Main.timeForVisualEffects * 0.1f;

            Vector2 vecScale = new Vector2(1f, 1f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(glow2, Projectile.Center - Main.screenPosition - (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20), null, Color.DeepSkyBlue * 0.5f, Projectile.velocity.ToRotation(), glow2.Size() / 2, 0.8f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow2, Projectile.Center - Main.screenPosition - (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 20), null, Color.SkyBlue * 0.5f, Projectile.velocity.ToRotation(), glow2.Size() / 2, 0.4f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            //Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.ForestGreen with { A = 0 }, Projectile.velocity.ToRotation() + rot, glow.Size() / 2, 0.2f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.ForestGreen with { A = 0 }, Projectile.velocity.ToRotation() + rot + MathHelper.PiOver2, glow.Size() / 2, 0.2f, SpriteEffects.None, 0f);


            //Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.velocity.ToRotation(), glow.Size() / 2, 0.1f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.velocity.ToRotation() + MathHelper.PiOver2, glow.Size() / 2, 0.1f, SpriteEffects.None, 0f);

            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            return false;
        }
    }

    
    public class TempJadeFirePulse : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.scale = 0.1f;

            Projectile.timeLeft = 29;
            Projectile.tileCollide = false; //false;
            Projectile.width = 20;
            Projectile.height = 20;
        }

        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.rotation = Main.rand.NextFloat(6.28f);
            }

            Projectile.scale = MathHelper.Lerp(Projectile.scale, 0.81f, 0.2f); //1.51
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player Player = Main.player[Projectile.owner];
            Texture2D texture = Mod.Assets.Request<Texture2D>("Assets/Orbs/ElectricPop").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/FireBallShader", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["caustics"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/tstar").Value);
            myEffect.Parameters["distort"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/noise").Value);
            myEffect.Parameters["gradient"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/fireStar2").Value);
            myEffect.Parameters["uTime"].SetValue(timer * 0.08f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            int height1 = texture.Height;
            Vector2 origin1 = new Vector2(texture.Width / 2f, height1 / 2f);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.Orange, Projectile.rotation, origin1, Projectile.scale * 0.15f, SpriteEffects.None, 0.0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

    }

}