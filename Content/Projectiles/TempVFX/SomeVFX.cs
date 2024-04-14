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

namespace AerovelenceMod.Content.Projectiles.TempVFX
{
    //Spawn twice in quick succession
    public class ThunderStar : ModProjectile
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

        public override bool? CanDamage()
        {
            return false;
        }

        int timer = 0;
        float scale = 0;
        float alpha = 1;

        public override void AI()
        {
            if (timer < 30)
            {
                scale = MathHelper.Lerp(scale, 0.65f, 0.2f);
            }

            if (timer >= 25)
            {
                scale = scale + 0.035f;
                alpha -= 0.08f;
            }

            Projectile.timeLeft = 2;

            if (alpha <= 0)
                Projectile.active = false;

            timer++;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            //COOLNESS BELOW
            //A: Flare = DrawnStar | caus = Noise_1 | Grad = SofterBlueGrad | Noise = Swirl | 0.3, 1, 0.8, 0.06, 0, time * 0.01, alpha * 1.6 | drawn twice
            //^ Spawn twice in quick succession 

            //B: Flare = spotlight_8 | caus = Noise_1 | Grad = orangeGrad | Noise = Swirl | 0.3, 1, 0.8, 0.06, 0, time * 0.01, alpha * 2.6 | drawn twice

            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/DrawnStar").Value;


            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/SofterBlueGrad").Value);
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
            myEffect.Parameters["distortStrength"].SetValue(0.06f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            myEffect.Parameters["colorIntensity"].SetValue(alpha * 1.6f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Flare.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Flare.Size() / 2, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    public class FireShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public int timer = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 500;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {

            if (timer > 5)
                Projectile.velocity.X *= .99f;
            Projectile.velocity.Y += 0.15f;

            Projectile.rotation += Projectile.velocity.Length() * 0.02f;


            Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * -3f;

            Dust c = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), vel.RotatedByRandom(0.25f), 0, Color.Orange, 0.8f);
            c.alpha = 2;
            c.noLight = true;

            if (Main.rand.NextBool())
            {
                Dust a = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelRise>(), vel.RotatedByRandom(0.5f), 0, Color.Lerp(Color.Orange, Color.OrangeRed, 0.25f), 0.5f);
                a.alpha = 2;
                a.noLight = true;
            }

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/RainbowRod");
            Texture2D glow2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/DiamondGlow");

            Vector2 scalee = new Vector2(1, 0.5f) * Projectile.scale * 1.5f;

            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 }, Projectile.velocity.ToRotation(), glow.Size() / 2, scalee, 0, 0f);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.Orange with { A = 0 }, Projectile.velocity.ToRotation(), glow.Size() / 2, scalee * 0.8f, 0, 0f);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.velocity.ToRotation(), glow.Size() / 2, scalee * 0.5f, 0, 0f);


            return false;
        }


        public override void Kill(int timeLeft)
        {


        }

    }

    public class SlashTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 500;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }
        TrailInfo trail1 = new TrailInfo();
        TrailInfo trail2 = new TrailInfo();
        public override void AI()
        {

            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value;
            trail1.trailColor = Color.White * 0f;
            trail1.trailPointLimit = 800;
            trail1.trailWidth = 60;
            trail1.trailMaxLength = 200;
            trail1.timesToDraw = 1;
            trail1.usePinchedWidth = true;


            trail1.trailTime = timer * 0.02f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center;
            trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/LavaTrailV1").Value;
            trail2.trailColor = Color.Wheat;
            trail2.trailPointLimit = 800;
            trail2.trailWidth = 40;
            trail2.trailMaxLength = 600;
            trail2.timesToDraw = 2;
            trail2.usePinchedWidth = true;

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/FireGradLoop").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = timer * 0.03f;

            //trail2.trailTime = 0.75f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center;
            trail2.TrailLogic();

            Projectile.velocity = Projectile.velocity.RotatedBy(0.05f);

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            return false;
        }

    }

    public class PhanticDiamond : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public int timer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 500;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public float offset = 20;
        public float rotation = Main.rand.NextFloat(6.28f);
        public float scale = 1f;

        float progress = 0f;

        public override void AI()
        {
            if (timer == 0)
            {
                for (int i = 0; i < 0; i++)
                {
                    Vector2 randomStart = Main.rand.NextVector2CircularEdge(2f, 2f) * 2f;
                    Dust dust1 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<MuraLineBasic>(), randomStart * Main.rand.NextFloat(0.5f, 2.5f));
                    dust1.scale = 0.4f + Main.rand.NextFloat(-0.1f, 0.1f);
                    dust1.alpha = 2 + Main.rand.Next(0, 8);
                    dust1.color = Color.Red;
                    dust1.noGravity = true;
                }
            }



            rotation += 0.045f;
            offset += 4f;

            if (timer > 3)
                progress = Math.Clamp(progress + 0.04f, 0f, 1f);

            if (timer > 1)
                initalProgress = Math.Clamp(MathHelper.Lerp(initalProgress, 2, 0.05f), 0, 1);

            timer++;
        }


        float initalProgress = 0;
        float overallAlpha = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Diamond = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Projectiles/TempVFX/StretchDiamondEdit");
            Texture2D White = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Projectiles/TempVFX/StretchDiamondWhite");
            Texture2D Orb = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/impact_2fade2");
            Texture2D specil = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/TestTex");

            Color backCol = Color.Lerp(Color.White, Color.Crimson, initalProgress);


            int numberOfSpikes = 5;
            for (int i = 0; i < numberOfSpikes; i++)
            {
                float rot = rotation + MathHelper.ToRadians((360 / numberOfSpikes) * i);
                Vector2 position = (rot.ToRotationVector2() * offset) - Main.screenPosition + Main.MouseWorld;

                Vector2 vec2Scale = new Vector2(1f - progress, 1f + (0.5f * (progress / 1))) * scale * 0.5f;


                Main.spriteBatch.Draw(White, position, null, backCol with { A = 0 } * (1f - initalProgress), rot + MathHelper.PiOver2, White.Size() / 2, vec2Scale * 1.1f, 0, 0f);
                Main.spriteBatch.Draw(Diamond, position, null, Color.Red with { A = 0 } * initalProgress, rot + MathHelper.PiOver2, Diamond.Size() / 2, vec2Scale, 0, 0f);
                Main.spriteBatch.Draw(Diamond, position, null, Color.Red with { A = 150 } * initalProgress, rot + MathHelper.PiOver2, Diamond.Size() / 2, vec2Scale, 0, 0f);
            }
            //float prog = Math.Clamp(progress * 2, 0, 1);
            //Main.spriteBatch.Draw(Orb, -Main.screenPosition + Main.MouseWorld, null, Color.Red with { A = 0 } * (1f - prog), rotation, Orb.Size() / 2, 0.2f + (0.7f * progress), 0, 0f);

            return false;
        }

    }

    public class FireBlast : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public bool PinkTrueBlueFalse = false;
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

        public override bool? CanDamage()
        {
            return false;
        }

        int timer = 0;
        public float scale = 0.25f;
        float alpha = 1;

        public override void AI()
        {
            //if (timer < 10)
            //{
            //scale = MathHelper.Lerp(scale, 1.25f, 0.3f);
            //}

            if (timer >= 0)
            {
                scale = scale + 0.09f;

                if (timer >= 6)
                    alpha -= 0.065f;
            }

            Projectile.timeLeft = 2;

            if (alpha <= 0)
                Projectile.active = false;

            timer++;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/spotlight_8").Value;
            Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/bright_star").Value;
            Texture2D Star2 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_16").Value;


            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/PinkGrad").Value); //also works well with GreenGrad and softer blue grad
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
            myEffect.Parameters["distortStrength"].SetValue(0.06f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            myEffect.Parameters["colorIntensity"].SetValue(alpha * 3f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Flare.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Flare.Size() / 2, scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Star.Size() / 2, scale * 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star2, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Star2.Size() / 2, scale * 1f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    public class FireBlastAlt : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public bool PinkTrueBlueFalse = false;
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

        public override bool? CanDamage()
        {
            return false;
        }

        int timer = 0;
        public float scale = 0.1f;
        float alpha = 1;

        public override void AI()
        {
            //if (timer < 10)
            //{
            //scale = MathHelper.Lerp(scale, 1.25f, 0.3f);
            //}

            scale = MathHelper.Clamp(MathHelper.Lerp(scale, 1.25f, 0.1f), 0f, 1f);

            if (Projectile.scale >= 0.8f)
                alpha = MathHelper.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.1f), 0, 2);

            if (alpha <= 0)
                Projectile.active = false;

            /*
            if (timer >= 0)
            {
                if (timer < 6)
                    scale = scale + 0.09f;
                else
                    scale = scale + 0.055f;

                if (timer >= 6)
                    alpha -= 0.1f;
                else
                    alpha -= 0.015f;
            }
            */
            Projectile.timeLeft = 2;

            if (alpha <= 0)
                Projectile.active = false;

            timer++;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Orbs/whiteFireEyeArc45").Value;
            Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/bright_star").Value;
            Texture2D Star2 = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_16").Value;

            float rot1 = (float)Main.timeForVisualEffects * 0.0f;
            float rot2 = (float)Main.timeForVisualEffects * 0.0f;

            Vector2 scale2 = new Vector2(scale, scale * 1f) * 1.25f;

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;

            myEffect.Parameters["causticTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1").Value);
            myEffect.Parameters["gradientTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/orangeGrad").Value); //also works well with GreenGrad and softer blue grad
            myEffect.Parameters["distortTexture"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl").Value);

            myEffect.Parameters["flowSpeed"].SetValue(0.3f);
            myEffect.Parameters["vignetteSize"].SetValue(1f);
            myEffect.Parameters["vignetteBlend"].SetValue(0.8f);
            myEffect.Parameters["distortStrength"].SetValue(0.06f);
            myEffect.Parameters["xOffset"].SetValue(0.0f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.01f);
            myEffect.Parameters["colorIntensity"].SetValue(alpha * 3f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.DeepPink, rot1, Flare.Size() / 2, scale2, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.HotPink, rot2, Flare.Size() / 2, scale2, SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Star.Size() / 2, scale * 2f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Star2, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Star2.Size() / 2, scale * 1f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    public class DreadDart : ModProjectile
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

            Projectile.extraUpdates = 6;
        }

        BaseTrailInfo trail1 = new BaseTrailInfo();
        BaseTrailInfo trail2 = new BaseTrailInfo();

        int timer = 0;
        float alpha = 0f;
        Vector2 trailOffset = Vector2.Zero;

        public List<Vector2> previousPositions;
        public List<float> previousRotations;

        public override void AI()
        {
            if (timer == 0)
            {
                previousPositions = new List<Vector2>();
                previousRotations = new List<float>();

                Projectile.scale = 1f;
            }

            //NOPE TOO LAGGY CUZ EXTRA UPDATES

            var target = Projectile.FindTargetWithLineOfSight(240f);
            if (target != -1)
            {
                Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity,
                    Vector2.Normalize(Main.npc[target].Center - Projectile.Center) * Projectile.velocity.Length(), 0.05f)) * Projectile.velocity.Length();
            }
            

            if (timer % 2 == 0)
            {

                previousPositions.Add(Projectile.Center + Projectile.velocity);
                previousRotations.Add(Projectile.velocity.ToRotation());

                if (previousPositions.Count > 40)
                {
                    previousPositions.RemoveAt(0);
                    previousRotations.RemoveAt(0);

                }
            }

            if (timer < 20 * Projectile.extraUpdates)
            {
                alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.5f, 0.02f), 0, 1);
            }

            if (timer > 15 * Projectile.extraUpdates)
            {
                if (timer < 30)
                {
                    alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.25f, 0.05f / Projectile.extraUpdates), 0, 1);
                    Projectile.scale -= 0.01f / Projectile.extraUpdates;
                    Projectile.scale = Math.Clamp(Projectile.scale, 0, 100);

                    Projectile.velocity *= 0.97f;

                }
                else
                {
                    alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.25f, 0.08f / Projectile.extraUpdates), 0, 1);
                    Projectile.scale -= 0.02f / Projectile.extraUpdates;
                    Projectile.scale = Math.Clamp(Projectile.scale, 0, 100);

                    Projectile.velocity *= 0.98f;

                }


            }

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {

            if (previousRotations == null)
                return false;
            
            //Texture2D Swing = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/SwordSwipeA").Value;
            Texture2D Swing = Mod.Assets.Request<Texture2D>("Assets/TrailImages/BusterGlow").Value;

            //Texture2D SwingStandard = Mod.Assets.Request<Texture2D>("Assets/TrailImages/TestTex").Value;
            Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_4").Value;

            Vector2 scale = new Vector2(0.35f * alpha, 1f) * Projectile.scale;
            Vector2 scale2 = new Vector2(0.15f * alpha, 1f) * Projectile.scale;

            for (int afterI = 0; afterI < previousPositions.Count; afterI++)
            {
                float scaleMultiplier = (float)afterI / previousPositions.Count;
                Main.spriteBatch.Draw(Swing, previousPositions[afterI] - Main.screenPosition, null, Color.Black * alpha * 1f, previousRotations[afterI] + MathHelper.PiOver2, Swing.Size() / 2, scale * 1.5f * scaleMultiplier, SpriteEffects.None, 0f);

            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Swing, Projectile.Center - Main.screenPosition + -Projectile.velocity, null, Color.White * alpha, Projectile.velocity.ToRotation() + MathHelper.PiOver2, Swing.Size() / 2, scale2 * 2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Swing, Projectile.Center - Main.screenPosition + -Projectile.velocity, null, Color.White * alpha * 0.5f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, Swing.Size() / 2, scale2 * 2.4f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Swing, Projectile.Center - Main.screenPosition + -Projectile.velocity, null, Color.White * alpha * 0.2f, Projectile.velocity.ToRotation() + MathHelper.PiOver2, Swing.Size() / 2, scale2 * 2.6f, SpriteEffects.None, 0f);


            for (int afterI = 0; afterI < previousPositions.Count; afterI++)
            {
                float scaleMultiplier = (float)afterI / previousPositions.Count;

                Main.spriteBatch.Draw(Swing, previousPositions[afterI] - Main.screenPosition, null, getGradColor((float)afterI / previousPositions.Count) * alpha * 0.6f, previousRotations[afterI] + MathHelper.PiOver2, Swing.Size() / 2, scale * 1.7f * scaleMultiplier, SpriteEffects.None, 0f);


                Main.spriteBatch.Draw(Swing, previousPositions[afterI] - Main.screenPosition, null, getGradColor((float)afterI / previousPositions.Count) * alpha * 0.45f, previousRotations[afterI] + MathHelper.PiOver2, Swing.Size() / 2, scale * 1.7f * scaleMultiplier, SpriteEffects.None, 0f);

            }


            Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Projectile.velocity * 2f, null, Color.MediumPurple * alpha * 0.7f, (float)Main.timeForVisualEffects * 0.03f + 1f, Star.Size() / 2, 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Projectile.velocity * 2f, null, Color.Lavender * alpha * 0.7f, (float)Main.timeForVisualEffects * -0.04f + 0.5f, Star.Size() / 2, 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + Projectile.velocity * 2f, null, Color.Purple * alpha * 0.7f, (float)Main.timeForVisualEffects * 0.05f, Star.Size() / 2, 0.5f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

        public Color mainCol = Color.DodgerBlue;//new Color(Main.rand.Next(0, 255), Main.rand.Next(0, 255), Main.rand.Next(0, 255));
        public Color getGradColor(float progress)
        {
            Color myCol = Color.White;

            Color color1 = Color.White;
            Color color2 = mainCol; //Red/MediumPurple
            Color color3 = mainCol; //DarkRed/Purple


            if (progress < 0.33f)
            {
                myCol = Color.Lerp(color1, color2, progress * 3f);
            } 
            else if (progress < 0.66f)
            {
                float fakeProgress = progress - 0.33f;
                myCol = Color.Lerp(color2, color3, fakeProgress * 3f);

            }
            else
            {
                float fakeProgress = progress - 0.66f;
                myCol = Color.Lerp(color3, color2, fakeProgress * 3f);

            }
            return myCol;
        }
    }

    public class FireDart : ModProjectile
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
            Projectile.timeLeft = 400;
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
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/LineGraduation2").Value;
            trail1.trailColor = Color.OrangeRed;
            trail1.trailPointLimit = 300;
            trail1.trailWidth = 15;
            trail1.trailMaxLength = 300;
            trail1.timesToDraw = 2;
            trail1.pinch = true;
            trail1.pinchAmount = 0.2f;

            //trail1.trailTime = mainTimer * 0.03f;
            trail1.trailRot = Projectile.velocity.ToRotation();

            trail1.trailPos = Projectile.Center + Projectile.velocity;
            trail1.TrailLogic();

            //Trail2
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail4").Value;
            trail2.trailColor = Color.OrangeRed;
            trail2.trailPointLimit = 300;
            trail2.trailWidth = 30;
            trail2.trailMaxLength = 300;
            trail2.timesToDraw = 2;
            //trail1.pinch = true;
            //trail1.pinchAmount = 0.5f;

            //trail2.trailTime = mainTimer * -0.06f;
            trail2.trailRot = Projectile.velocity.ToRotation();

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

            if (mainTimer < 20)
                Projectile.velocity = Projectile.velocity.RotatedBy(0.08f * Projectile.ai[0]);

            if (mainTimer > 25)
                Projectile.velocity *= 0.9f;

            if (Projectile.timeLeft < 320)
                Projectile.active = false;

            mainTimer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/BossDrops/Cyvercry/GreyScaleStar").Value;
            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/Glow").Value;

            return false;
        }
    }

    public class GreenStarThing : ModProjectile
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
            Projectile.timeLeft = 400;
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
            trail1.trailColor = Color.LightGreen;
            trail1.trailPointLimit = 300;
            trail1.trailWidth = 15;
            trail1.trailMaxLength = 300;
            trail1.timesToDraw = 2;
            trail1.pinch = true;
            trail1.pinchAmount = 0.4f;

            //trail1.trailTime = mainTimer * 0.03f;
            trail1.trailRot = Projectile.velocity.ToRotation();

            trail1.trailPos = Projectile.Center + Projectile.velocity;
            trail1.TrailLogic();

            //Trail2
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail4").Value;
            trail2.trailColor = Color.DarkGreen;
            trail2.trailPointLimit = 300;
            trail2.trailWidth = 10;
            trail2.trailMaxLength = 300;
            trail2.timesToDraw = 1;
            //trail1.pinch = true;
            //trail1.pinchAmount = 0.5f;

            //trail2.trailTime = mainTimer * 0.06f;
            trail2.trailRot = Projectile.velocity.ToRotation();

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

            if (mainTimer < 20)
                Projectile.velocity = Projectile.velocity.RotatedBy(0.04f * Projectile.ai[0]);

            if (mainTimer > 25)
                Projectile.velocity *= 0.9f;

            if (Projectile.timeLeft < 320)
                Projectile.active = false;

            mainTimer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/FireSpike").Value;
            float rot = (float)Main.timeForVisualEffects * 0.1f;


            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.ForestGreen with { A = 0 }, Projectile.velocity.ToRotation() + rot, glow.Size() / 2, 0.2f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.ForestGreen with { A = 0 }, Projectile.velocity.ToRotation() + rot + MathHelper.PiOver2, glow.Size() / 2, 0.2f, SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.velocity.ToRotation(), glow.Size() / 2, 0.1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.velocity.ToRotation() + MathHelper.PiOver2, glow.Size() / 2, 0.1f, SpriteEffects.None, 0f);

            return false;
        }
    }

    public class ArcriTrail : ModProjectile
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

    public class BisexualIceCreamCone : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Ball");
            Main.projFrames[Projectile.type] = 9;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public float strength = 1f;
        int fakeTimeLeft = 540;
        public override void SetDefaults()
        {
            Projectile.width = 62;
            Projectile.height = 48;
            Projectile.timeLeft = 540;
            Projectile.penetrate = -1;
            Projectile.damage = 120;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        BaseTrailInfo relativeTrail = new BaseTrailInfo();
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.9f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.7f / 255f);
            Projectile.rotation = MathHelper.ToRadians(180) + Projectile.velocity.ToRotation();
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            float approaching = ((540f - Projectile.timeLeft) / 540f) * strength;
            Lighting.AddLight(Projectile.Center, 0.5f, 0.65f, 0.75f);

            Player player = Main.player[(int)Projectile.ai[0]];
            int dust = Dust.NewDust(Projectile.Center + new Vector2(0, -4), 0, 0, DustID.Electric, 0, 0, Projectile.alpha, default, 0.5f);
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity += Projectile.velocity;
            Main.dust[dust].velocity *= 0.1f;
            Main.dust[dust].scale *= 0.7f;
            if (player.active)
            {
                float x = Main.rand.Next(-10, 11) * 0.005f * approaching;
                float y = Main.rand.Next(-10, 11) * 0.005f * approaching;
                Vector2 toPlayer = Projectile.Center - player.Center;
                toPlayer = toPlayer.SafeNormalize(Vector2.Zero);
                Projectile.velocity += -toPlayer * (0.155f * Projectile.timeLeft / 540f) + new Vector2(x, y);
            }

            if (Projectile.timeLeft == 380)
                Projectile.Kill();

            //trail
            relativeTrail.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/gooeyLightningDim").Value;
            relativeTrail.trailColor = new Color(78, 225, 245) * 0.75f;
            relativeTrail.trailPointLimit = 800;
            relativeTrail.trailWidth = 18;
            relativeTrail.trailMaxLength = 100;
            relativeTrail.timesToDraw = 1;
            relativeTrail.trailTime = (float)Main.timeForVisualEffects * 0.05f;
            relativeTrail.trailRot = Projectile.rotation;

            relativeTrail.trailPos = Projectile.Center + Projectile.velocity;
            relativeTrail.TrailLogic();

            fakeTimeLeft--;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            relativeTrail.TrailDrawing(Main.spriteBatch);
            relativeTrail.trailColor = Color.White;
            relativeTrail.trailWidth = 7;

            relativeTrail.TrailDrawing(Main.spriteBatch);
            relativeTrail.trailColor = new Color(78, 225, 245);
            relativeTrail.trailWidth = 21;

            var newTex = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle128PMA").Value;
            //var BallTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/EnergyBall").Value;
            var BallTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/EnergyBallWhite").Value;

            int frameHeight = BallTexture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, BallTexture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Vector2 bonus = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5f;
            Vector2 vec2Scale = new Vector2(1f, 1f) * Projectile.scale;

            //Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, sourceRectangle, Color.Black * 1f, Projectile.rotation, origin, 1.3f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(newTex, Projectile.Center - Main.screenPosition + bonus, null, Color.Black * 0.3f, Projectile.rotation, newTex.Size() / 2, vec2Scale * 0.4f, SpriteEffects.None, 0f);


            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float progress = k / (float)Projectile.oldPos.Length;
                Vector2 scale = new Vector2(1f, 1f - (progress * 0.5f)) * (Projectile.scale + (progress * 0.25f));

                float alpha = ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY) - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10;
                Color color = Color.Lerp(Color.DeepPink, Color.DeepSkyBlue, progress) with { A = 0 } * alpha;
                Main.spriteBatch.Draw(BallTexture, drawPos, sourceRectangle, color * 0.8f, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(newTex, Projectile.Center - Main.screenPosition + bonus, null, Color.HotPink with { A = 0 } * 0.17f, Projectile.rotation, newTex.Size() / 2, vec2Scale * 1.35f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(newTex, Projectile.Center - Main.screenPosition + bonus, null, Color.DeepPink with { A = 0 } * 0.4f, Projectile.rotation, newTex.Size() / 2, vec2Scale * 0.65f, SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, sourceRectangle, Color.White with { A = 0 }, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);

            return false;

            #region OptionA
            /*
            Vector2 vec2Scale = new Vector2(1f, 1f) * Projectile.scale;

            Main.spriteBatch.Draw(newTex, Projectile.Center - Main.screenPosition + bonus, null, Color.HotPink with { A = 0 } * 0.17f, Projectile.rotation, newTex.Size() / 2, vec2Scale * 1.35f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(newTex, Projectile.Center - Main.screenPosition + bonus, null, Color.DeepPink with { A = 0 } * 0.45f, Projectile.rotation, newTex.Size() / 2, vec2Scale * 0.5f, SpriteEffects.None, 0f);



            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + bonus, Tex.Frame(1, 1, 0, 0), Color.DeepPink with { A = 0 } * 0.5f, Projectile.rotation, Tex.Size() / 2, 0.7f, SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Tex, Projectile.Center - Main.screenPosition + bonus, Tex.Frame(1, 1, 0, 0), Color.HotPink with { A = 0 } * 0.2f, Projectile.rotation, Tex.Size() / 2, 0.9f, SpriteEffects.None, 0f);

            var BallTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/EnergyBall").Value;

            int frameHeight = BallTexture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            // Get this frame on texture
            Rectangle sourceRectangle = new Rectangle(0, startY, BallTexture.Width, frameHeight);


            Vector2 origin = sourceRectangle.Size() / 2f;

            for (int i = 0; i < 0; i++)
            {
                Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10 + Main.rand.NextVector2Circular(2f, 2f),
                    sourceRectangle, Color.DeepPink with { A = 0 } * 0.5f, Projectile.rotation, origin, 1.15f, SpriteEffects.None, 0f);

            }

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                float progress = k / (float)Projectile.oldPos.Length;
                Vector2 scale = new Vector2(1f, 1f - (progress * 0.5f)) * Projectile.scale;

                float alpha = ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin + new Vector2(0f, Projectile.gfxOffY) - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10;
                Color color = Color.HotPink with { A = 0 } * alpha;
                Main.spriteBatch.Draw(BallTexture, drawPos, sourceRectangle, color * 0.8f, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            }

            //Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, sourceRectangle, Color.HotPink with { A = 0 } * 0.25f, Projectile.rotation, origin, 1.15f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, sourceRectangle, Color.White with { A = 0 }, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            */
            #endregion
        }
    }
}