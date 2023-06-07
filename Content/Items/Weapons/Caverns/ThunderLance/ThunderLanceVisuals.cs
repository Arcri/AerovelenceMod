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
using AerovelenceMod.Content.Projectiles;
using Terraria.Utilities;
using System.Net;

namespace AerovelenceMod.Content.Items.Weapons.Caverns.ThunderLance
{
    public class ThunderBall : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false; //false;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.timeLeft = 800;
            Projectile.width = 20;
            Projectile.height = 20;
        }

        int timer = 0;
        float overallScale = 1f;

        bool popping = false;
        float popProgress = 0;

        public override void AI()
        {
            Projectile.scale = 1f;

            ballScale = 1 - ((float)Math.Sin((float)timer * 0.03f) * 0.12f);
            ballBright = (float)Math.Sin((float)timer * 0.02f);

            Projectile.rotation += 0.04f;
            Projectile.timeLeft = 2;

            Projectile.velocity *= 0.92f;

            timer++;
        }

        float ballScale = 0;
        float ballBright = 0;
        Vector2 drawScale = Vector2.Zero;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/bigCircle2").Value;
            Texture2D Lightning = Mod.Assets.Request<Texture2D>("Assets/Orbs/bigCircle2").Value;

            //ElectricRadialEffect
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/foam_mask_bloom").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/ThunderGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(0.2f);
            myEffect.Parameters["vignetteBlend"].SetValue(1f);
            myEffect.Parameters["distortStrength"].SetValue(0.1f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue(timer * 0.015f);

            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(0, 0, 0, 0), Projectile.rotation, Ball.Size() / 2, 0.15f * ballScale * 2f * Projectile.scale, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(0, 0, 0, 0) * 0.5f, Projectile.rotation, Ball.Size() / 2, 0.15f * ballScale * 2.5f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            Color col = Color.DeepSkyBlue;
            //col.A = 0;
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation * -1, Ball.Size() / 2, 0.15f * ballScale * 2f * Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, col * 0.8f, Projectile.rotation, Ball.Size() / 2, 0.2f * ballScale * 2f * Projectile.scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, col * 0.3f, Projectile.rotation * -1, Ball.Size() / 2, 0.3f * ballScale * 2f * Projectile.scale, SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.Black * 0.6f, Projectile.rotation, Ball.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            Vector2 scaleV = new Vector2(1f, 1f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            myEffect.CurrentTechnique.Passes[0].Apply();

            //Main.spriteBatch.Draw(Pupil, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), 0f, Pupil.Size() / 2, Projectile.scale * scaleV, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Pupil, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), 0f, Pupil.Size() / 2, Projectile.scale * scaleV, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Pupil, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), 0f, Pupil.Size() / 2, Projectile.scale * scaleV, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Lightning, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Lightning.Size() / 2, 0.4f * Projectile.scale * (ballScale + 0.4f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Lightning, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * -1f, Lightning.Size() / 2, 0.4f * Projectile.scale * 0.7f * (ballScale + 0.4f), SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.spriteBatch.Draw(Lightning, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Lightning.Size() / 2, Projectile.scale * (ballScale + 0.4f), SpriteEffects.None, 0f);

            return false;
        }
    }

    public class LightningTrailTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;

        }

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        int timer = 0;

        bool chase = false;
        public override void AI()
        {
            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTextureButBlack").Value;
            trail1.trailColor = Color.White * 0.7f;
            trail1.trailPointLimit = 300;
            trail1.trailWidth = 22;
            trail1.trailMaxLength = 600;
            trail1.timesToDraw = 2;
            trail1.pinch = true;
            trail1.pinchAmount = 0.1f;


            trail1.trailTime = timer * 0.02f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center + Projectile.velocity;
            trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5Loop").Value;
            trail2.trailColor = Color.DeepSkyBlue;
            trail2.trailPointLimit = 300;
            trail2.trailWidth = 80;
            trail2.trailMaxLength = 600;
            trail2.timesToDraw = 2;
            trail2.pinch = true;
            trail2.pinchAmount = 0.1f;


            //trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/LoopingThunderGrad").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = timer * 0.03f;

            trail2.trailTime = timer * 0.03f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center + Projectile.velocity;
            trail2.TrailLogic();

            Projectile.velocity.Y += 0.02f;

            //Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 15;

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/bigCircle2").Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Ball.Size() / 2, Projectile.scale * 0.1f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }
    }

    public class ThunderPop : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;

            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.timeLeft = 800;
            Projectile.scale = 1f;

        }

        int timer = 0;
        float scale = 0;
        float alpha = 1;
        public override void AI()
        {
            float ts = 0.4f;
            if (timer < 40)
                scale = Math.Clamp(MathHelper.Lerp(scale, 2f * ts, (float)(timer * 0.02f)), 0, 1.5f * ts);
            else
                scale = Math.Clamp(MathHelper.Lerp(scale, -0.5f, 0.05f), 0, 1.5f * ts);

            Projectile.rotation += 0.12f;


            Projectile.timeLeft = 2;

            if (timer > 180)
                alpha -= 0.02f;

            if (alpha <= 0)
                Projectile.active = false;

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Pupil = Mod.Assets.Request<Texture2D>("Assets/Noise/CoolNoise").Value;
            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/ElectricRadialEffect").Value;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/ThunderGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(1.0f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.7f);
            myEffect.Parameters["distortStrength"].SetValue(0.02f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue(timer * 0.01f);


            //MapOdd
            //PinkExplosion
            //ThunderGrad2
            //SwirlDistort
            //2 draws with opposite rotation directions

            //Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(0, 0, 0, 255) * 0.1f, Projectile.rotation, Ball.Size() / 2, scale * 0.2f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(0, 0, 0, 255) * 0.1f, Projectile.rotation * -1, Ball.Size() / 2, scale * 0.2f, SpriteEffects.None, 0f);

            Vector2 scaleV = new Vector2(1f, 1f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            myEffect.CurrentTechnique.Passes[0].Apply();

            //Main.spriteBatch.Draw(Pupil, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Pupil.Size() / 2, 0.35f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Pupil, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Pupil.Size() / 2, 0.35f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Ball.Size() / 2, scale * scaleV, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * -1, Ball.Size() / 2, scale * scaleV * 1.25f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation, Ball.Size() / 2, scale * scaleV, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255, 0), Projectile.rotation * -1, Ball.Size() / 2, scale * scaleV, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    public class LanceViusalTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.friendly = false;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.tileCollide = false;

            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.timeLeft = 800;
            Projectile.scale = 1f;

        }

        int timer = 0;
        float scale = 1;
        float alpha = 1;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            spearBackGlow();


            Texture2D Spear = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Caverns/ThunderLance/ThunderLanceProjectile");

            Vector2 origin = new Vector2(Spear.Width * 0.95f, Spear.Height * 0.05f);

            Main.spriteBatch.Draw(Spear, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, origin, scale * Projectile.scale, SpriteEffects.None, 0f);


            return false;
        }

        public void spearBackGlow()
        {
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Caverns/ThunderLance/ThunderLanceBackGlow");

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/ThunderGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(0f);
            myEffect.Parameters["vignetteBlend"].SetValue(1f);
            myEffect.Parameters["distortStrength"].SetValue(0.01f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue(timer * 0.02f);

            Vector2 scaleV = new Vector2(1f, 1f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);

            myEffect.CurrentTechnique.Passes[0].Apply();


            Vector2 origin = new Vector2(Glow.Width * 0.95f, Glow.Height * 0.05f);

            Main.spriteBatch.Draw(Glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, scale * Projectile.scale, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }
    }
    public class LightningHitFX : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CultistBossLightningOrbArc;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10; //10
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.scale = 0.2f;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.alpha = 200;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 90 * (Projectile.extraUpdates + 1);
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() { return false; }

        float colorlerp;
        public bool shouldFade = true;

        int timer = 0;
        public override void AI()
        {
            if (shouldFade)
                FadeBehavior();

            timer++;

            Projectile.frameCounter = Projectile.frameCounter + 1;
            Lighting.AddLight(Projectile.Center, Color.DeepSkyBlue.ToVector3() * 0.5f);
            colorlerp += 0.05f;


            if (Projectile.velocity == Vector2.Zero)
            {
                if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
                {
                    Projectile.frameCounter = 0;
                    bool flag = true;
                    for (int index = 1; index < Projectile.oldPos.Length; ++index)
                    {
                        if (Projectile.oldPos[index] != Projectile.oldPos[0])
                            flag = false;
                    }
                    if (flag)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                if (!Main.rand.NextBool(Projectile.extraUpdates))
                    return;
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    float num1 = Projectile.rotation + (float)((Main.rand.NextBool(2) ? -1.0 : 1.0) * 1.57079637050629);
                    float num2 = (float)(Main.rand.NextDouble() * 0.800000011920929 + 1.0);
                    Vector2 vector2 = new((float)Math.Cos(num1) * num2, (float)Math.Sin(num1) * num2);
                    int index2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Electric, vector2.X, vector2.Y, 0, new Color(), 1f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].scale = 1.2f;
                }
                if (!Main.rand.NextBool(5))
                    return;
                int index3 = Dust.NewDust(Projectile.Center + Projectile.velocity.RotatedBy(1.57079637050629, new Vector2()) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width - Vector2.One * 4f, 8, 8, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust = Main.dust[index3];
                dust.velocity *= 0.5f;
                Main.dust[index3].velocity.Y = -Math.Abs(Main.dust[index3].velocity.Y);
            }
            else
            {
                if (Projectile.frameCounter < Projectile.extraUpdates * 2)
                    return;
                Projectile.frameCounter = 0;
                float num1 = Projectile.velocity.Length();
                UnifiedRandom unifiedRandom = new((int)Projectile.ai[1]);
                int num2 = 0;
                Vector2 spinningpoint = -Vector2.UnitY;
                Vector2 rotationVector2;
                int num3;
                do
                {
                    int num4 = unifiedRandom.Next();
                    Projectile.ai[1] = num4;
                    rotationVector2 = ((float)(num4 % 100 / 100.0 * 6.28318548202515)).ToRotationVector2();
                    if (rotationVector2.Y > 0.0)
                        rotationVector2.Y--;
                    bool flag = false;
                    if (rotationVector2.Y > -0.0199999995529652)
                        flag = true;
                    if (rotationVector2.X * (double)(Projectile.extraUpdates + 1) * 2.0 * num1 + Projectile.localAI[0] > 40.0)
                        flag = true;
                    if (rotationVector2.X * (double)(Projectile.extraUpdates + 1) * 2.0 * num1 + Projectile.localAI[0] < -40.0)
                        flag = true;
                    if (flag)
                    {
                        num3 = num2;
                        num2 = num3 + 1;
                    }
                    else
                        goto label_3460;
                }
                while (num3 < 100);
                Projectile.velocity = Vector2.Zero;
                Projectile.localAI[1] = 1f;
                goto label_3461;
            label_3460:
                spinningpoint = rotationVector2;
            label_3461:
                if (Projectile.velocity == Vector2.Zero || Projectile.velocity.Length() < 4f)
                {
                    Projectile.velocity = Vector2.UnitX.RotatedBy(Projectile.ai[0]).RotatedByRandom(Math.PI / 4) * 7f;
                    Projectile.ai[1] = Main.rand.Next(100);
                    return;
                }
                Projectile.localAI[0] += (float)(spinningpoint.X * (double)(Projectile.extraUpdates + 1) * 2.0) * num1;
                Projectile.velocity = spinningpoint.RotatedBy(Projectile.ai[0] + 1.57079637050629, new Vector2()) * num1;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.570796f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color27 = Projectile.GetAlpha(lightColor);
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i - 1] == Projectile.oldPos[i])
                    continue;
                Vector2 offset = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
                int length = (int)offset.Length();
                float scale = Projectile.scale * (float)Math.Sin(i / MathHelper.Pi);
                offset.Normalize();
                const int step = 2;
                for (int j = 0; j < length; j += step)
                {
                    Vector2 value5 = Projectile.oldPos[i] + offset * j;
                    Main.spriteBatch.Draw(texture2D13, value5 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, Projectile.rotation, origin2, scale, SpriteEffects.FlipHorizontally, 0f);
                    //Main.spriteBatch.Draw(texture2D13, value5 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.DeepSkyBlue, Projectile.rotation, origin2, scale, SpriteEffects.FlipHorizontally, 0f);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;


        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(Color.White * 0.25f, Color.DeepSkyBlue, 0.5f + (float)Math.Sin(colorlerp) / 2) * 0.85f;
        }

        public void FadeBehavior()
        {
            if (timer > 15)
                Projectile.scale -= 0.01f; 

            if (Projectile.scale < 0.1f)
            {
                if (Projectile.scale < 0)
                {
                    Projectile.active = false;
                }
            }
        }
    }


    
}