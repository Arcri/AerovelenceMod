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

namespace AerovelenceMod.Content.Projectiles.TempVFX
{

    public class ComboLaserTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        int mainTimer = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 400;
        }

        float startAngle = 0f;
        float endAngle = 0f;

        int timer = 0;
        public override void AI()
        {

            timer++;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/EasyLightray").Value;
            Texture2D black = Mod.Assets.Request<Texture2D>("Assets/ThinGlowLine").Value;

            Vector2 newScale = new Vector2(2f, 0.5f) * 0.5f;
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaser", AssetRequestMode.ImmediateLoad).Value;

            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition - new Vector2(0f, 0f), null, Color.Black * 0.35f, Projectile.rotation, glow.Size() / 2, newScale, SpriteEffects.None, 0f);

            ShaderParams();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition - new Vector2(0f, 0f), null, Color.White, Projectile.rotation, glow.Size() / 2, newScale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public void ShaderParams()
        {
            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
            myEffect.Parameters["sampleTexture3"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail7").Value);
            myEffect.Parameters["sampleTexture4"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTextureButBlack").Value);

            Color c1 = Color.DeepSkyBlue;
            Color c2 = Color.CornflowerBlue;
            Color c3 = Color.DodgerBlue;
            Color c4 = Color.DodgerBlue;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color3"].SetValue(c3.ToVector4());
            myEffect.Parameters["Color4"].SetValue(c4.ToVector4());

            myEffect.Parameters["Color1Mult"].SetValue(1.75f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["Color3Mult"].SetValue(0.25f);
            myEffect.Parameters["Color4Mult"].SetValue(0.75f);
            myEffect.Parameters["totalMult"].SetValue(1.15f);

            myEffect.Parameters["tex1reps"].SetValue(1f);
            myEffect.Parameters["tex2reps"].SetValue(1f);
            myEffect.Parameters["tex3reps"].SetValue(1f);
            myEffect.Parameters["tex4reps"].SetValue(1f);

            myEffect.Parameters["satPower"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.03f); //4


            /*
            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5Loop").Value);
            myEffect.Parameters["sampleTexture3"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
            myEffect.Parameters["sampleTexture4"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinGlowLine").Value);

            Color c1 = Color.DeepPink;
            Color c2 = Color.HotPink;
            Color c3 = Color.HotPink;
            Color c4 = Color.DeepPink;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color3"].SetValue(c3.ToVector4());
            myEffect.Parameters["Color4"].SetValue(c4.ToVector4());

            myEffect.Parameters["Color1Mult"].SetValue(1.5f);
            myEffect.Parameters["Color2Mult"].SetValue(1.5f);
            myEffect.Parameters["Color3Mult"].SetValue(0.5f);
            myEffect.Parameters["Color4Mult"].SetValue(1.1f);
            myEffect.Parameters["totalMult"].SetValue(1.15f);

            myEffect.Parameters["tex1reps"].SetValue(1f);
            myEffect.Parameters["tex2reps"].SetValue(1f);
            myEffect.Parameters["tex3reps"].SetValue(1f);
            myEffect.Parameters["tex4reps"].SetValue(1f);

            myEffect.Parameters["satPower"].SetValue(2f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.03f);
            */
        }
    }

    public class ComboVertexTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 7500;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 3400; //180
            Projectile.extraUpdates = 20;
        }


        float startAngle = 0f;
        float endAngle = 0f;
        int timer = 0;
        float width = 1f;

        public List<Vector2> previousPositions;
        public List<float> previousRotations;

        Vector2 origin;
        Vector2[] positions;
        float[] rotations;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (timer == 0)
            {
                previousPositions = new List<Vector2>();
                previousRotations = new List<float>();
                origin = Projectile.Center;

                Projectile.timeLeft = 3400;
            }

            if (timer < 1000)
            {
                previousPositions.Add(Projectile.Center);
                previousRotations.Add(new Vector2(1.5f + (timer * 0.0014f), 0f).RotatedBy((timer * 0.0125f) + startAngle).ToRotation());

                Projectile.velocity = new Vector2(1.5f + (timer * 0.0014f), 0f).RotatedBy((timer * 0.0125f) + startAngle);

                positions = previousPositions.ToArray();
                rotations = previousRotations.ToArray();

                //if (previousRotations.Count > 800) { previousRotations.RemoveAt(0); }
                //if (previousPositions.Count > 800) { previousPositions.RemoveAt(0); }
            }

            /*
            int val = timer * 6;
            positions = new Vector2[val];
            rotations = new float[val];
            for (int i = 0; i < val; i++)
            {
                positions[i] = Projectile.Center + new Vector2((float)val * (i / (float)val), 0).RotatedBy(Projectile.rotation + (i * 0.0025f));
                rotations[i] = Projectile.rotation + (i * 0.0025f);
            }
            */

            timer++;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/circle_053").Value;
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaserVertex", AssetRequestMode.ImmediateLoad).Value;

            /*
            int val = timer * 6;

            Vector2[] positions = new Vector2[val];
            float[] rotations = new float[val];
            for (int i = 0; i < val; i++)
            {
                positions[i] = Projectile.Center + new Vector2((float)val * (i / (float)val), 0).RotatedBy(Projectile.rotation + (i * 0.0025f)) ;
                rotations[i] = Projectile.rotation + (i * 0.0025f);
            }
            */
            VertexStrip strip = new VertexStrip();
            strip.PrepareStripWithProceduralPadding(positions, rotations, StripColor, StripWidth, -Main.screenPosition, true);
            ShaderParamsAlt();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            //myEffect.CurrentTechnique.Passes["DefaultPass"].Apply();
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            strip.DrawTrail();
            //strip.DrawTrail();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);




            //Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            return false;
        }

        public void ShaderParams()
        {
            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);

            myEffect.Parameters["onTex"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinLineGlowClear").Value);
            myEffect.Parameters["baseColor"].SetValue(Color.White.ToVector3() * 1f);
            myEffect.Parameters["satPower"].SetValue(1f);

            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5Loop").Value);
            myEffect.Parameters["sampleTexture3"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/gooeyLightningDim").Value);
            myEffect.Parameters["sampleTexture4"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinGlowLine").Value);

            Color c1 = new Color(255, 96, 40, 255);
            Color c2 = new Color(240, 79, 40, 255);
            Color c3 = new Color(255, 173, 40, 255);
            Color c4 = new Color(255, 149, 40, 255);

            //Color c1 = new Color(115, 0, 255, 255) * 1;
            //Color c2 = new Color(69, 0, 237, 255) * 1;
            //Color c3 = new Color(139, 0, 255, 255) * 1;
            //Color c4 = new Color(149, 33, 213, 255) * 1;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color3"].SetValue(c3.ToVector4());
            myEffect.Parameters["Color4"].SetValue(c4.ToVector4());

            myEffect.Parameters["Color1Mult"].SetValue(1f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["Color3Mult"].SetValue(0.15f);
            myEffect.Parameters["Color4Mult"].SetValue(1f);
            myEffect.Parameters["totalMult"].SetValue(1.15f);

            myEffect.Parameters["tex1reps"].SetValue(3f);
            myEffect.Parameters["tex2reps"].SetValue(3f);
            myEffect.Parameters["tex3reps"].SetValue(3f);
            myEffect.Parameters["tex4reps"].SetValue(1f);

            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.015f);

        }

        public void ShaderParamsAlt()
        {
            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);

            myEffect.Parameters["onTex"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/WackBeam").Value);
            myEffect.Parameters["baseColor"].SetValue(Color.White.ToVector3() * 1f);
            myEffect.Parameters["satPower"].SetValue(0.75f);

            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/LavaTrailV1").Value);
            myEffect.Parameters["sampleTexture3"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail7").Value);
            myEffect.Parameters["sampleTexture4"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTextureButBlack").Value);

            //Color c1 = new Color(255, 96, 40, 255);
            //Color c2 = new Color(240, 79, 40, 255); 
            //Color c3 = new Color(255, 173, 40, 255);
            //Color c4 = new Color(255, 149, 40, 255);

            Color c1 = Color.DeepSkyBlue;
            Color c2 = Color.CornflowerBlue;
            Color c3 = Color.DodgerBlue;
            Color c4 = Color.DodgerBlue;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color3"].SetValue(c3.ToVector4());
            myEffect.Parameters["Color4"].SetValue(c4.ToVector4());

            myEffect.Parameters["Color1Mult"].SetValue(1.75f);
            myEffect.Parameters["Color2Mult"].SetValue(0.75f);
            myEffect.Parameters["Color3Mult"].SetValue(0.25f);
            myEffect.Parameters["Color4Mult"].SetValue(0.75f);
            myEffect.Parameters["totalMult"].SetValue(1.15f);

            myEffect.Parameters["tex1reps"].SetValue(5f);
            myEffect.Parameters["tex2reps"].SetValue(5f);
            myEffect.Parameters["tex3reps"].SetValue(5f);
            myEffect.Parameters["tex4reps"].SetValue(5f);

            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.02f);

        }
        public Color StripColor(float progress)
        {
            Color color = Color.White * 0f;
            color.A = 0;
            return color;
        }
        public float StripWidth(float progress)
        {
            float size = Utils.GetLerpValue(3400f, 2800f, Projectile.timeLeft, true) * Utils.GetLerpValue(0f, 200f, Projectile.timeLeft, true);
            float start = Math.Clamp(1.5f * (float)Math.Pow(progress, 0.5f), 0f, 1f);
            float cap = (float)Math.Cbrt(Utils.GetLerpValue(1f, 0.85f, progress, true));
            return start * size * 150f * cap;// * (1.1f + (float)Math.Cos(timer) * (0.08f - progress * 0.06f));

        }
    }

    public class OtherVertexLaserTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 7500;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 3100; //180
            Projectile.extraUpdates = 20;
        }


        float startAngle = 0f;
        float endAngle = 0f;
        int timer = 0;
        float width = 1f;

        public List<Vector2> previousPositions;
        public List<float> previousRotations;

        Vector2 origin;
        Vector2[] positions;
        float[] rotations;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (timer == 0)
            {
                previousPositions = new List<Vector2>();
                previousRotations = new List<float>();
                origin = Projectile.Center;

                Projectile.timeLeft = 3700;
            }

            if (timer < 1000)
            {
                previousPositions.Add(Projectile.Center);
                previousRotations.Add(new Vector2(1.5f + (timer * 0.0014f), 0f).RotatedBy((timer * 0.0125f) + startAngle).ToRotation());

                Projectile.velocity = new Vector2(1.5f + (timer * 0.0014f), 0f).RotatedBy((timer * 0.0125f) + startAngle);

                positions = previousPositions.ToArray();
                rotations = previousRotations.ToArray();

                //if (previousRotations.Count > 800) { previousRotations.RemoveAt(0); }
                //if (previousPositions.Count > 800) { previousPositions.RemoveAt(0); }
            }
            else
                Projectile.velocity = Vector2.Zero;
            
            int mod = 30;
            if (timer % mod == 0)
            {
                int maxAdd = Projectile.timeLeft < 300 ? 50 : 100;
                for (int i = Main.rand.Next(0, 15); i < previousPositions.Count; i += Main.rand.Next(5, maxAdd))
                {
                    int randChance = 20;
                    if (Main.rand.NextBool(randChance))
                    {

                        float size = Main.rand.NextFloat(0.35f, 0.5f) * 0.5f;
                        //Color col = Main.rand.NextBool() ? new Color(255, 75, 15) : Color.DarkOrange;
                        Color col = Main.rand.NextBool() ? new Color(255, 75, 15) : Color.OrangeRed;
                        Vector2 sideOffset = (previousRotations[i] + MathHelper.PiOver2).ToRotationVector2() * (Projectile.timeLeft < 300 ? Main.rand.NextFloat(-5, 5.01f) : Main.rand.NextFloat(-10, 10.1f));

                        Dust star = Dust.NewDustPerfect(previousPositions[i] + sideOffset, ModContent.DustType<GlowPixelAlts>(),
                        previousRotations[i].ToRotationVector2().RotatedByRandom(0.2f) * Main.rand.NextFloat(4f, 14f), newColor: col, Scale: Main.rand.NextFloat(0.35f, 0.50f) * 1.25f);

                        //star.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                            //rotPower: 0.15f, preSlowPower: 0.91f, timeBeforeSlow: 15, postSlowPower: 0.90f, velToBeginShrink: 2f, fadePower: 0.93f, shouldFadeColor: false);


                        //float maxVel = Projectile.timeLeft < 200 ? 7.5f : 10f;

                        //Dust orb = Dust.NewDustPerfect(previousPositions[i] + sideOffset, ModContent.DustType<GlowStrong>(),
                            //previousRotations[i].ToRotationVector2().RotatedByRandom(0.15f) * Main.rand.NextFloat(4f, maxVel), 
                            //newColor: col, Scale: size);


                    }
                }
            }

            /*
            int val = timer * 6;
            positions = new Vector2[val];
            rotations = new float[val];
            for (int i = 0; i < val; i++)
            {
                positions[i] = Projectile.Center + new Vector2((float)val * (i / (float)val), 0).RotatedBy(Projectile.rotation + (i * 0.0025f));
                rotations[i] = Projectile.rotation + (i * 0.0025f);
            }
            */

            timer++;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/circle_053").Value;
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaserVertexFade", AssetRequestMode.ImmediateLoad).Value;

            /*
            int val = timer * 6;

            Vector2[] positions = new Vector2[val];
            float[] rotations = new float[val];
            for (int i = 0; i < val; i++)
            {
                positions[i] = Projectile.Center + new Vector2((float)val * (i / (float)val), 0).RotatedBy(Projectile.rotation + (i * 0.0025f)) ;
                rotations[i] = Projectile.rotation + (i * 0.0025f);
            }
            */
            VertexStrip strip = new VertexStrip();
            strip.PrepareStripWithProceduralPadding(positions, rotations, StripColor, StripWidth, -Main.screenPosition, true);
            ShaderParams();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            //myEffect.CurrentTechnique.Passes["DefaultPass"].Apply();
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            strip.DrawTrail();
            //strip.DrawTrail();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            return false;
        }

        public void ShaderParams()
        {
            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);

            myEffect.Parameters["onTex"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/WackBeam").Value);
            myEffect.Parameters["baseColor"].SetValue(Color.White.ToVector3() * 1f);
            myEffect.Parameters["satPower"].SetValue(0.95f);

            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
            myEffect.Parameters["sampleTexture3"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail7").Value);
            myEffect.Parameters["sampleTexture4"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTextureButBlack").Value);

            // orange = 255, 165, 0 
            // orangeRed = 255 69 0 
            Color a = Color.Orange;
            Color b = Color.OrangeRed;
            Color c1 = new Color(255, 96, 30, 255);
            Color c2 = new Color(240, 79, 30, 255);
            Color c3 = new Color(255, 173, 30, 255);
            Color c4 = new Color(255, 169, 30, 255);

            //Color c1 = Color.DeepSkyBlue;
            //Color c2 = Color.CornflowerBlue;
            //Color c3 = Color.DodgerBlue;
            //Color c4 = Color.DodgerBlue;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color3"].SetValue(c3.ToVector4());
            myEffect.Parameters["Color4"].SetValue(c4.ToVector4());

            myEffect.Parameters["Color1Mult"].SetValue(1.75f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["Color3Mult"].SetValue(0.25f);
            myEffect.Parameters["Color4Mult"].SetValue(0.75f);
            myEffect.Parameters["totalMult"].SetValue(1.15f);

            myEffect.Parameters["tex1reps"].SetValue(2.5f);
            myEffect.Parameters["tex2reps"].SetValue(3f);
            myEffect.Parameters["tex3reps"].SetValue(3f);
            myEffect.Parameters["tex4reps"].SetValue(3f);

            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.017f * 1f);

        }
        public Color StripColor(float progress)
        {
            Color color = Color.White * 0f;
            color.A = 0;
            return color;
        }
        public float StripWidth(float progress)
        {
            float size = Utils.GetLerpValue(3700f, 3100f, Projectile.timeLeft, true) * Easings.easeOutQuart(Utils.GetLerpValue(0f, 600f, Projectile.timeLeft, true));
            float start = Math.Clamp(1.5f * (float)Math.Pow(progress, 0.5f), 0f, 1f);
            float cap = (float)Math.Cbrt(Utils.GetLerpValue(1f, 0.85f, progress, true));
            return start * size * 150f * cap;// * (1.1f + (float)Math.Cos(timer) * (0.08f - progress * 0.06f));

        }
    }

    public class WindBarrierTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 7500;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 4400; //180
            Projectile.extraUpdates = 25;
        }


        float startAngle = 0f;
        float endAngle = 0f;
        int timer = 0;
        float width = 1f;

        public List<Vector2> previousPositions;
        public List<float> previousRotations;

        Vector2 origin;
        Vector2[] positions;
        float[] rotations;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (timer == 0)
            {
                previousPositions = new List<Vector2>();
                previousRotations = new List<float>();
                origin = Projectile.Center;

                Projectile.timeLeft = 13400;
            }

            if (timer < 1400)
            {
                previousPositions.Add(Projectile.Center);
                previousRotations.Add(new Vector2(3f + (timer * 0.002f), 0f).RotatedBy((timer * 0.012f) + startAngle).ToRotation());

                Projectile.velocity = new Vector2(3f + (timer * 0.002f), 0f).RotatedBy((timer * 0.012f) + startAngle);

                positions = previousPositions.ToArray();
                rotations = previousRotations.ToArray();

                //if (previousRotations.Count > 800) { previousRotations.RemoveAt(0); }
                //if (previousPositions.Count > 800) { previousPositions.RemoveAt(0); }
            }
            else
            {
                Projectile.velocity = Vector2.Zero;
                Projectile.extraUpdates = 20;
            }

            /*
            int val = timer * 6;
            positions = new Vector2[val];
            rotations = new float[val];
            for (int i = 0; i < val; i++)
            {
                positions[i] = Projectile.Center + new Vector2((float)val * (i / (float)val), 0).RotatedBy(Projectile.rotation + (i * 0.0025f));
                rotations[i] = Projectile.rotation + (i * 0.0025f);
            }
            */

            timer++;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/MuzzleFlashes/circle_053").Value;
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaserVertexFade", AssetRequestMode.ImmediateLoad).Value;

            /*
            int val = timer * 6;

            Vector2[] positions = new Vector2[val];
            float[] rotations = new float[val];
            for (int i = 0; i < val; i++)
            {
                positions[i] = Projectile.Center + new Vector2((float)val * (i / (float)val), 0).RotatedBy(Projectile.rotation + (i * 0.0025f)) ;
                rotations[i] = Projectile.rotation + (i * 0.0025f);
            }
            */
            VertexStrip strip = new VertexStrip();
            strip.PrepareStripWithProceduralPadding(positions, rotations, StripColor, StripWidth, -Main.screenPosition, true);
            ShaderParams();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            //myEffect.CurrentTechnique.Passes["DefaultPass"].Apply();
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            strip.DrawTrail();
            //strip.DrawTrail();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);




            //Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            return false;
        }

        public void ShaderParams()
        {
            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);

            myEffect.Parameters["onTex"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/WackBeam").Value);
            myEffect.Parameters["baseColor"].SetValue(Color.Gray.ToVector3() * 0f);
            myEffect.Parameters["satPower"].SetValue(0.25f);

            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail7").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlameTrail").Value);
            myEffect.Parameters["sampleTexture3"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
            myEffect.Parameters["sampleTexture4"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);


            Color c1 = Color.DimGray * 0.35f;
            Color c2 = Color.LightSlateGray * 0.35f;
            Color c3 = Color.DimGray * 0.35f;
            Color c4 = Color.DimGray * 0.35f;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color3"].SetValue(c3.ToVector4());
            myEffect.Parameters["Color4"].SetValue(c4.ToVector4());

            myEffect.Parameters["Color1Mult"].SetValue(1f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["Color3Mult"].SetValue(1f);
            myEffect.Parameters["Color4Mult"].SetValue(1f);
            myEffect.Parameters["totalMult"].SetValue(0f);

            myEffect.Parameters["tex1reps"].SetValue(3f);
            myEffect.Parameters["tex2reps"].SetValue(3f);
            myEffect.Parameters["tex3reps"].SetValue(3f);
            myEffect.Parameters["tex4reps"].SetValue(3f);

            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.017f * 0.8f);

        }
        public Color StripColor(float progress)
        {
            Color color = Color.White * 0f;
            color.A = 0;
            return color;
        }
        public float StripWidth(float progress)
        {
            float size = Utils.GetLerpValue(13400f, 12800f, Projectile.timeLeft, true) * Utils.GetLerpValue(0f, 200f, Projectile.timeLeft, true);
            float start = Math.Clamp(1.5f * (float)Math.Pow(progress, 0.5f), 0f, 1f);
            float cap = (float)Math.Cbrt(Utils.GetLerpValue(1f, 0.85f, progress, true));
            return start * size * 150f * cap;// * (1.1f + (float)Math.Cos(timer) * (0.08f - progress * 0.06f));

        }
    }

    public class TendrilTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 7500;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 4400; //180
            Projectile.extraUpdates = 0;
            Projectile.penetrate = -1;
        }


        int timer = 0;
        float width = 1f;

        public List<Vector2> Positions;
        public List<float> Rotations;

        Vector2[] arr_positions = new Vector2[150];
        float[] arr_rotations = new float[150];

        float[] draw_rotations = new float[150];
        Vector2[] draw_positions = new Vector2[150];


        int TotalPoints = 150; 

        Vector2 anchor = Vector2.Zero;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
                        
            if (timer == 0)
            {
                //Projectile.velocity = Vector2.Zero;
                anchor = Projectile.Center;

                //Create all of the points and set all the rotations to be the same
                for (int i = 0; i < TotalPoints; i++)
                {
                    arr_positions[i] = Vector2.Zero + Projectile.rotation.ToRotationVector2() * (2.5f * i);
                    arr_rotations[i] = Projectile.rotation;
                }

                Projectile.ai[0] = 1f;
            }

            anchor += Projectile.velocity;


            //Rotate the anchor
            Projectile.rotation += 0.07f * Projectile.ai[0];

            //if (timer % 130 == 0 && timer != 0)
                //Projectile.ai[0] *= -1f;

            //Have all points try to rotate towards the acnhor
            for (int j = 0; j < TotalPoints; j++)
            {
                float progress = (j / (float)TotalPoints);

                //The further along the trail, the weaker the turning
                float lerpValue = MathHelper.Lerp(1f, 0.3f, progress);


                //Keep angle within 2pi 
                float NormalizedGoalRotation = Projectile.rotation;//Projectile.rotation.ToRotationVector2().ToRotation();

                float newRotation = MathHelper.Lerp(arr_rotations[j], NormalizedGoalRotation, lerpValue * 0.175f); //0.2

                arr_rotations[j] = newRotation;
                arr_positions[j] = Vector2.Zero + newRotation.ToRotationVector2() * (2.5f * j);
            }

            for (int k = 0; k < TotalPoints - 1; k++)
            {
                //We have to flip the first point over for some reason or else we get a weird tear.
                if (k == 0)
                    draw_rotations[k] = arr_rotations[k] + MathHelper.Pi;
                else
                    draw_rotations[k] = (arr_positions[k - 1] - arr_positions[k]).ToRotation();
                draw_positions[k] = arr_positions[k] + anchor;
            }

            timer++;
        }

        Effect myEffect = null;
        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D trailTexture = Mod.Assets.Request<Texture2D>("Assets/spark_07_Black").Value;
            Texture2D trailTexture = Mod.Assets.Request<Texture2D>("Assets/spark_07_Black").Value;
            Texture2D trailTexture2 = Mod.Assets.Request<Texture2D>("Assets/Extra_196_Black").Value;            

            Texture2D glow = Mod.Assets.Request<Texture2D>("Assets/TrailImages/VanillaStar").Value;
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;


            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(draw_positions, draw_rotations, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStripBlack = new VertexStrip();
            vertexStripBlack.PrepareStrip(draw_positions, draw_rotations, StripColorBlack, StripWidthBlack, -Main.screenPosition, includeBacksides: true);


            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            myEffect.Parameters["progress"].SetValue(timer * -0.04f);

            //Black Layer
            #region black
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            myEffect.Parameters["ColorOne"].SetValue(Color.Black.ToVector3() * 1f);
            myEffect.Parameters["glowThreshold"].SetValue(1f);

            myEffect.CurrentTechnique.Passes["MainPS"].Apply();

            //vertexStripBlack.DrawTrail();
            #endregion

            //Main layer
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            myEffect.Parameters["ColorOne"].SetValue(Color.OrangeRed.ToVector3() * 4f);

            myEffect.Parameters["glowThreshold"].SetValue(0.8f);
            myEffect.Parameters["glowIntensity"].SetValue(1.4f);


            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip.DrawTrail();
            vertexStrip.DrawTrail();

            //Layer 2
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture2);
            myEffect.Parameters["ColorOne"].SetValue(Color.OrangeRed.ToVector3() * 4f);

            myEffect.Parameters["glowThreshold"].SetValue(0.5f);
            myEffect.Parameters["glowIntensity"].SetValue(1.8f);


            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStripBlack.DrawTrail();
            vertexStripBlack.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
            
            return false;
        }

        public Color StripColor(float progress)
        {
            float alpha = 1f;

            alpha = 1f - Easings.easeOutQuad(progress);

            Color color = new Color(0f, 0f, 0f, alpha); 
            
            return color;
        }
        public float StripWidth(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, 1f - progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 140f, Easings.easeInCirc(num)) * 1.15f; // 0.3f 
        }

        public Color StripColorBlack(float progress)
        {
            float alpha = 1f;

            alpha = 1f - Easings.easeOutQuad(progress);

            Color color = new Color(0f, 0f, 0f, alpha);

            return color;
        }
        public float StripWidthBlack(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, 1f - progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 60f, Easings.easeInCirc(num)) * 1f; // 0.3f 
        }
    }

}