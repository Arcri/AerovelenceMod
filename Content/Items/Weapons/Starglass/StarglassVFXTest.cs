using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System.Collections.Generic;
using Terraria.Graphics;

namespace AerovelenceMod.Content.Items.Weapons.Starglass
{
    public class StarglassTestVFX : ModProjectile
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

            Projectile.timeLeft = 350;
            Projectile.penetrate = -1;
        }

        TrailInfo trail1 = new TrailInfo();
        TrailInfo trail2 = new TrailInfo();

        int timer = 0;

        public Color trailCol = Main.rand.NextBool() ? new Color(255, 20, 20) : Color.DodgerBlue;

        Vector2 oldPos = Vector2.Zero;

        public override void AI()
        {
            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/FlamesTextureButBlack").Value;
            trail1.trailColor = Color.White * 0.8f;
            trail1.trailPointLimit = 400;
            trail1.trailWidth = (int)(15 * Projectile.scale);
            trail1.trailMaxLength = 600;
            trail1.timesToDraw = 2;

            trail1.trailTime = timer * 0.02f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center;
            trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/LintyTrail").Value;
            trail2.trailColor = trailCol;
            trail2.trailPointLimit = 400;
            trail2.trailWidth = (int)(20 * Projectile.scale);
            trail2.trailMaxLength = 600;
            trail2.timesToDraw = 2;

            trail2.trailTime = timer * 0.04f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center;
            trail2.TrailLogic();

            Vector2 pos = trailCol == new Color(255, 20, 20) ? new Vector2(-200, 0f) : new Vector2(200f, 0f);

            Projectile.velocity = (Main.MouseWorld - Projectile.Center + pos).SafeNormalize(Vector2.UnitX) * 15;

            oldPos = Projectile.Center;

            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            trail1.TrailDrawing(Main.spriteBatch);
            trail2.TrailDrawing(Main.spriteBatch);

            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/flare_12").Value;
            Texture2D Glow = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;

            float scaley = 0.5f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(Glow, oldPos - Main.screenPosition, null, trailCol * 0.3f, Projectile.rotation + timer * -0.05f, Glow.Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(Glow, oldPos - Main.screenPosition, null, trailCol * 0.45f, Projectile.rotation + timer * -0.05f, Glow.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Glow, oldPos - Main.screenPosition, null, Color.White * 0.45f, Projectile.rotation + timer * -0.05f, Glow.Size() / 2, Projectile.scale * 0.35f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Glow, oldPos - Main.screenPosition, null, trailCol * 0.45f, Projectile.rotation + timer * -0.05f, Glow.Size() / 2, Projectile.scale * 0.15f, SpriteEffects.None, 0f);


            Main.spriteBatch.Draw(Ball, oldPos - Main.screenPosition, null, Color.White, Projectile.rotation + timer * -0.05f, Ball.Size() / 2, Projectile.scale * 0.40f * scaley, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ball, oldPos - Main.screenPosition, null, trailCol, Projectile.rotation + timer * 0.02f, Ball.Size() / 2, Projectile.scale * 0.5f * scaley, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Ball, oldPos - Main.screenPosition, null, trailCol, Projectile.rotation + timer * 0.035f, Ball.Size() / 2, Projectile.scale * 0.5f * scaley, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);



            return false;
        }

    }

    public class TrailInfo
    {
        #region variables

        public float trailRot = 0;

        public Vector2 trailPos = Vector2.Zero;

        public int timesToDraw = 1;

        public Color trailColor = Color.White;

        public float trailTime = 0f;

        public Texture2D trailTexture = null;

        public bool gradient = false;

        public Texture2D gradientTexture = null;

        public float gradientTime = 0f;

        public bool shouldScrollColor = true;

        //------------------
        public int trailPointLimit = 60;

        public int trailWidth = 20;

        public float trailMaxLength = 200;

        public float trailCurrentLength;

        public Effect customEffect;

        public List<Vector2> trailPositions;

        public List<float> trailRotations;

        private bool initialized = false;
        public float lengthPercent
        {
            get
            {
                return trailCurrentLength / trailMaxLength;
            }
        }
        #endregion

        #region TrailShapeMethods
        public void Initialize()
        {
            if (!initialized)
            {
                trailPositions = new List<Vector2>();
                trailRotations = new List<float>();

                initialized = true;
            }
        }

        public float CalculateLength()
        {
            float calculatedLength = 0;
            for (int i = 0; i < trailPositions.Count - 2; i++)
            {
                calculatedLength += Vector2.Distance(trailPositions[i], trailPositions[i + 1]);
            }

            return calculatedLength;
        }

        public virtual float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, trailWidth, num) * 1f; // 0.3f 

            /*
            if (progress < 0.5f)
            {
                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.2f, progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, trailWidth, num) * 1f;
            }
            else if (progress >= 0.5)
            {
                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.25f, 1 - progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, trailWidth, num) * 1f;
            }
            return 0;
            */
        }

        public virtual Color ColorFunction(float progress)
        {
            //This only matters if you are using basic effect
            return trailColor;
        }

        #endregion

        public void TrailLogic()
        {
            Initialize();
            trailPositions.Add(trailPos);
            trailRotations.Add(trailRot);

            //Smoothing
            if (trailPositions.Count > 3)
            {
                for (int i = 3; i < trailPositions.Count - 1; i++)
                {
                    trailPositions[i - 2] = (trailPositions[i - 3] + trailPositions[i - 1]) / 2f;
                    trailRotations[i - 2] = Vector2.Lerp(trailRotations[i - 3].ToRotationVector2(), trailRotations[i - 1].ToRotationVector2(), 0.5f).ToRotation();
                }
            }

            trailCurrentLength = CalculateLength();


            //This could be optimized to not require recomputing the length after each removal
            while (trailPositions.Count > trailPointLimit || trailCurrentLength > trailMaxLength)
            {
                trailPositions.RemoveAt(0);
                trailRotations.RemoveAt(0);
                trailCurrentLength = CalculateLength();
            }
        }
        public void TrailDrawing(SpriteBatch sb)
        {
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, customEffect, Main.GameViewMatrix.TransformationMatrix);

            if (gradient)
                customEffect = AerovelenceMod.TrailShaderGradient;
            else
                customEffect = AerovelenceMod.BasicTrailShader;

            customEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            customEffect.Parameters["ColorOne"].SetValue(trailColor.ToVector4());

            int width = Main.graphics.GraphicsDevice.Viewport.Width;
            int height = Main.graphics.GraphicsDevice.Viewport.Height;

            Vector2 zoom = Main.GameViewMatrix.Zoom;
            Matrix view = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                          Matrix.CreateTranslation(width / 2f, height / -2f, 0) * Matrix.CreateRotationZ(MathHelper.Pi) *
                          Matrix.CreateScale(zoom.X, zoom.Y, 1f);

            Matrix projection = Matrix.CreateOrthographic(width, height, 0, 1000);
            customEffect.Parameters["WorldViewProjection"].SetValue(view * projection);
            customEffect.Parameters["progress"].SetValue(trailTime);

            if (gradient)
            {
                customEffect.Parameters["gradientTex"].SetValue(gradientTexture);
                customEffect.Parameters["gradProgress"].SetValue(gradientTime);
                customEffect.Parameters["scrollColor"].SetValue(shouldScrollColor);

            }

            customEffect.CurrentTechnique.Passes["DefaultPass"].Apply();
            customEffect.CurrentTechnique.Passes["MainPS"].Apply();

            VertexStrip vertexStrip = new VertexStrip();
            if (trailPositions != null)
            {
                vertexStrip.PrepareStrip(trailPositions.ToArray(), trailRotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, includeBacksides: true);

                for (int i = 0; i < timesToDraw; i++)
                {
                    vertexStrip.DrawTrail();
                }
            }

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }

    /*
    public class StarglassParticleHandler
    {
        private static List<StarglassParticle> particleInstances;

        //0-1 alpha not 0-255
        public static void CreateParticle(Vector2 position, Vector2 velocity, float scale, Color color, float alpha = 1)
        {
            StarglassParticle newStar = new StarglassParticle(position, velocity, scale, color, alpha);
            particleInstances.Add(newStar);
        }

        public static void UpdateParticles()
        {
            foreach (StarglassParticle particle in particleInstances)
            {
                particle.Update();
            }

            particleInstances.RemoveAll(n => n is null || n.alpha <= 0);
        }

        public static void DrawAllStarglassParticles(SpriteBatch sb)
        {

            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, default, default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
            foreach (StarglassParticle particle in particleInstances)
            {
                particle.DrawStar(sb);
            }
            sb.End();

            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }
    }

    public class StarglassParticle
    {
        public Vector2 Velocity;
        public Vector2 Center;

        public float rotation;
        public Color color;
        public float scale;
        public float alpha;

        public int timer;

        public Texture2D Texture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/GlowStar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public StarglassParticle(Vector2 Center, Vector2 Velocity, float scale, Color color, float alpha)
        {
            this.Center = Center;
            this.Velocity = Velocity;
            this.scale = scale;
            this.color = color;
            this.alpha = alpha;
        }

        public void Update()
        {
            if (timer > 20)
                alpha = MathHelper.Clamp(MathHelper.Lerp(alpha, -0.2f, 0.03f), 0f, 1f);

            Center += Velocity;
            Velocity *= 0.95f;
            rotation += Velocity.X * 0.03f;

            scale *= 0.99f;
            timer++;
        }

        public void DrawStar(SpriteBatch sb)
        {
            sb.Draw(Texture, Center - Main.screenPosition, null, color * alpha, rotation, Texture.Size() / 2, scale * 0.35f, SpriteEffects.None, 0f);
            sb.Draw(Texture, Center - Main.screenPosition, null, color * alpha, rotation, Texture.Size() / 2, scale * 0.35f, SpriteEffects.None, 0f);
        }
    }

    public static class StarglassParticleDetour
    {
        public static void Load()
        {
            Terraria.On_Main.DrawInterface += DrawStarglassParticles;

        }

        public static void Unload()
        {
            //maybe?
            //Terraria.On_Main.DrawInterface -= DrawStarglassParticles;
        }

        private static void DrawStarglassParticles(Terraria.On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, default, default, RasterizerState.CullNone, default, Main.GameViewMatrix.ZoomMatrix);
            StarglassParticleHandler.DrawAllStarglassParticles(Main.spriteBatch);
            //Main.spriteBatch.End();

            orig(self, gameTime);
        }
    }

    internal class StarglassParticleSystem : ModSystem
    {
        public override void PostUpdateEverything()
        {
            StarglassParticleHandler.UpdateParticles();
        }
    }
    */
}