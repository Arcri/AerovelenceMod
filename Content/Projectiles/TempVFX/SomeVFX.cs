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
            trail1.trailColor = Color.White * 0.5f;
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
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail4").Value;
            trail2.trailColor = Color.Wheat;
            trail2.trailPointLimit = 800;
            trail2.trailWidth = 40;
            trail2.trailMaxLength = 600;
            trail2.timesToDraw = 1;
            trail2.usePinchedWidth = false;

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
}