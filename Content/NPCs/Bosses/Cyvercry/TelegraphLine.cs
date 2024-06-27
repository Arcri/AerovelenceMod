using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using System;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Terraria.GameContent;
using Terraria.Audio;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.SkillStrikes;
using System.Collections.Generic;
using Terraria.Graphics;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{   
    public class TelegraphLineCyver : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

        public Vector2 endPoint;
        public float Rotation = 0;

        public bool sweepTell = false;
        public bool sweepDir = false;

        float rotOffset = 0f;

        public bool custom = false;
        public int timeToLast = 0;

        int timer = 0;

        public NPC NPCTetheredTo = null;
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 110 + 5;
            Projectile.hide = true;
        }
        public override bool? CanDamage() {  return false; }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override void AI()
        {
            if (timer == 0)
            {
                Rotation = Projectile.velocity.ToRotation();
                rotOffset = Rotation;
            }


            if (NPCTetheredTo != null)
            {
                if (NPCTetheredTo.active == false)
                    Projectile.active = false;
                if (sweepTell)
                {
                    uColorIntensity = 0.9f;
                    Projectile.Center = NPCTetheredTo.Center + NPCTetheredTo.rotation.ToRotationVector2() * -30;
                    Rotation += 0.06f * (sweepDir ? 1 : -1);// + (timer * 0.002f);
                } 
                else
                {
                    Projectile.Center = NPCTetheredTo.Center;
                    Rotation = NPCTetheredTo.rotation + MathHelper.Pi + rotOffset;
                }


            }
            endPoint = Projectile.Center + Rotation.ToRotationVector2() * 2500f;
            Projectile.velocity = Vector2.Zero;

            Projectile.ai[0] = Math.Clamp(MathHelper.Lerp(0f, 1f, Easings.easeInQuad(timer / 15f)), 0f, 1f);
            timer++;
        }

        public float uColorIntensity = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(new Color(255, 25, 155).ToVector3() * 1.5f);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.3f); //0.8
            myEffect.Parameters["uSaturation"].SetValue(1.2f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //myEffect.CurrentTechnique.Passes[0].Apply();

            if (timer > 0)
            {
                var texBeam = Mod.Assets.Request<Texture2D>("Assets/ThinLineGlowClear").Value;

                Vector2 origin2 = new Vector2(0, texBeam.Height / 2);

                float height = 30f * Projectile.scale * (sweepTell ? 0.5f : 1); //15
                float height2 = 15f * Projectile.scale * (sweepTell ? 0.5f : 1); //15

                if (height == 0)
                    Projectile.active = false;

                int width = (int)(Projectile.Center - endPoint).Length() - 24;

                var pos = Projectile.Center - Main.screenPosition + Vector2.UnitX.RotatedBy(Rotation) * 24;
                var target = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height * 1f));
                var target2 = new Rectangle((int)pos.X, (int)pos.Y, width, (int)(height2 * 1f));



                Main.spriteBatch.Draw(texBeam, target, null, Color.HotPink * Projectile.ai[0], Rotation, origin2, 0, 0);

                if (!sweepTell)
                {
                    Main.spriteBatch.Draw(texBeam, target2, null, Color.HotPink * Projectile.ai[0], Rotation, origin2, 0, 0);

                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class SweepLaserTell : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
        }

        public bool sweepDir = false;


        int timer = 0;
        public NPC NPCTetheredTo = null;

        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.scale = 1f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 40;
            Projectile.hide = true;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public float scale = 0;
        public override void AI()
        {

            if (NPCTetheredTo != null)
            {
                if (NPCTetheredTo.active == false)
                    Projectile.active = false;


                float progress = 1f - (Projectile.timeLeft / 40f);
                Projectile.rotation += (0.1f + (progress * 0.15f)) * (sweepDir ? 1 : -1);



                float progressA = 1f - (Projectile.timeLeft / 25f);
                float progressB = 1f - ((Projectile.timeLeft - 25f) / 25f);

                Projectile.Center = NPCTetheredTo.Center;

                //scale = 1.5f * MathF.Sin((MathF.PI * Projectile.timeLeft) / 50f);

                if (Projectile.timeLeft > 10)
                {
                    scale = Math.Clamp(MathHelper.Lerp(scale, 1.2f, 0.1f), 0, 1.2f);
                    Projectile.ai[0] = 2f;
                }
                else
                {
                    //Projectile.rotation += (0.02f) * (sweepDir ? 1 : -1);

                    scale = scale + 0.02f;
                    Projectile.ai[0] = Math.Clamp(Projectile.ai[0] * 0.92f, 0, 5);
                }


                //if (Projectile.timeLeft > 10)
                //scale = Math.Clamp(MathHelper.Lerp(scale, 1.1f, 0.1f), 0, 1);
                //else
                //scale = Math.Clamp(MathHelper.Lerp(scale, -0.1f, 0.1f), 0, 1);


            }
            Projectile.velocity = Vector2.Zero;

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * Projectile.ai[0]);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.4f); //0.8
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Texture2D twirl = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/twirl_01pixel");


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(twirl, Projectile.Center - Main.screenPosition, null, Color.DeepPink, Projectile.rotation, twirl.Size() / 2, 1.6f * scale, !sweepDir ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(twirl, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(2, 2), null, Color.DeepPink, Projectile.rotation, twirl.Size() / 2, 1.5f * scale, !sweepDir ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

            //Main.spriteBatch.Draw(twirl, Projectile.Center - Main.screenPosition, null, Color.DeepPink, Projectile.rotation + MathF.PI, twirl.Size() / 2, 1.5f * scale, !sweepDir ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            //Main.spriteBatch.Draw(twirl, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(2, 2), null, Color.DeepPink, Projectile.rotation + MathF.PI, twirl.Size() / 2, 1.4f * scale, !sweepDir ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }


    public class CurveLineTelegraph : ModProjectile
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
        }


        float startAngle = 0f;
        float endAngle = 0f;
        int timer = 0;
        float width = 1f;

        public float startRot = 0f;
        public float curveAmount = 1f;

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
                startRot = (owner.Center - Projectile.Center).ToRotation();

                Projectile.timeLeft = 3400;
            }

            if (timer < 1000)
            {
                //previousPositions.Add(Projectile.Center);
                //previousRotations.Add(new Vector2(1.5f + (timer * 0.0014f), 0f).RotatedBy((timer * 0.0125f) + startAngle).ToRotation());

                //Projectile.velocity = new Vector2(1.5f + (timer * 0.0014f), 0f).RotatedBy((timer * 0.0125f) + startAngle);

                //positions = previousPositions.ToArray();
                //rotations = previousRotations.ToArray();

                //if (previousRotations.Count > 800) { previousRotations.RemoveAt(0); }
                //if (previousPositions.Count > 800) { previousPositions.RemoveAt(0); }
            }

            
            int val = Math.Clamp(timer * 6, 0, 400);
            float fakeRotation = (owner.Center - Projectile.Center).ToRotation();
            Vector2 fakePosition = Projectile.Center;

            positions = new Vector2[val];
            rotations = new float[val];

            for (int i = 0; i < val; i++)
            {
                fakeRotation = fakeRotation.AngleTowards((owner.Center - fakePosition).ToRotation(), 0.01f);
                fakePosition = fakePosition + (fakeRotation.ToRotationVector2() * 5f);

                positions[i] = fakePosition;
                rotations[i] = fakeRotation;
            }


            /*
            for (int i = 0; i < val; i++)
            {
                if (i == 0)
                {
                    positions[i] = Projectile.Center;
                    rotations[i] = (owner.Center - Projectile.Center).ToRotation();
                }
                else if (i == 1)
                {
                    positions[i] = owner.Center - owner.velocity * 2f;
                    rotations[i] = (owner.Center - Projectile.Center).ToRotation();

                    fakeRotation = (owner.Center - Projectile.Center).ToRotation();
                    fakePosition = owner.Center - owner.velocity * 2f;
                }
                else
                {
                    fakeRotation = fakeRotation.AngleTowards((owner.Center - fakePosition).ToRotation(), 0.01f);

                    fakePosition = fakePosition + (fakeRotation.ToRotationVector2() * 5f);
                    positions[i] = fakePosition;
                    rotations[i] = fakeRotation;
                }

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
            ShaderParams();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();

            strip.DrawTrail();
            //strip.DrawTrail();
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

        public void ShaderParams()
        {
            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);

            myEffect.Parameters["onTex"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinLineGlowClear").Value);
            myEffect.Parameters["baseColor"].SetValue(Color.White.ToVector3() * 1f);
            myEffect.Parameters["satPower"].SetValue(1f);

            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinGlowLine").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5Loop").Value);
            myEffect.Parameters["sampleTexture3"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/gooeyLightningDim").Value);
            myEffect.Parameters["sampleTexture4"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/ThinGlowLine").Value);

            Color c1 = Color.HotPink;
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

            myEffect.Parameters["Color1Mult"].SetValue(2f);
            myEffect.Parameters["Color2Mult"].SetValue(0f);
            myEffect.Parameters["Color3Mult"].SetValue(0f);
            myEffect.Parameters["Color4Mult"].SetValue(0f);
            myEffect.Parameters["totalMult"].SetValue(1f);

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
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
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
            myEffect.Parameters["Color2Mult"].SetValue(1f);
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
            float size = 1f;// Utils.GetLerpValue(3400f, 2800f, Projectile.timeLeft, true) * Utils.GetLerpValue(0f, 200f, Projectile.timeLeft, true);
            float start = 1f;// Math.Clamp(1.5f * (float)Math.Pow(progress, 0.5f), 0f, 1f);
            float cap = (float)Math.Cbrt(Utils.GetLerpValue(1f, 0.85f, progress, true));
            return start * size * 50f * cap;// * (1.1f + (float)Math.Cos(timer) * (0.08f - progress * 0.06f));

        }
    }

}