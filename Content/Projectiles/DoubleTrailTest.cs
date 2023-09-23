using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Renderers;
using System.Collections.Generic;
using AerovelenceMod.Content.NPCs.Bosses.Cyvercry;
using System.Collections;
using Terraria.Graphics;

namespace AerovelenceMod.Content.Projectiles
{
	public class DoubleTrailTest : ModProjectile
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

            Projectile.extraUpdates = 2;
        }

        TrailInfo trail1 = new TrailInfo();
        TrailInfo trail2 = new TrailInfo();

        int timer = 0;

        bool chase = false;
        public override void AI()
        {
            //Trail1 Info Dump
            trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value;
            trail1.trailColor = Color.White * 1f;
            trail1.trailPointLimit = 800;
            trail1.trailWidth = 30;
            trail1.trailMaxLength = 1200;
            trail1.timesToDraw = 1;
            trail1.usePinchedWidth = true;


            trail1.trailTime = timer * 0.007f;
            trail1.trailRot = Projectile.velocity.ToRotation();
            trail1.trailPos = Projectile.Center;
            trail1.TrailLogic();

            //Trail2 Info Dump
            trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value;
            trail2.trailColor = Color.Wheat;
            trail2.trailPointLimit = 800;
            trail2.trailWidth = 90;
            trail2.trailMaxLength = 1200;
            trail2.timesToDraw = 2;
            trail2.usePinchedWidth = true;

            trail2.gradient = true;
            trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad2").Value;
            trail2.shouldScrollColor = true;
            trail2.gradientTime = timer * 0.01f;

            trail2.trailTime = timer * 0.013f;
            trail2.trailRot = Projectile.velocity.ToRotation();
            trail2.trailPos = Projectile.Center;
            trail2.TrailLogic();

            //Projectile.velocity.X -= 0.02f;
            //Projectile.velocity.Y -= 0.02f;

            //if (timer < 60)
                //Projectile.velocity *= 1.05f;

            Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 8;

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

        public bool usePinchedWidth = false;

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

            if (!usePinchedWidth)
            {
                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, trailWidth, num) * 1f; // 0.3f 
            }
            else
            {
                if (progress < 0.5f)
                {
                    float num = 1f;
                    float lerpValue = Utils.GetLerpValue(0f, 0.3f, progress, clamped: true);
                    num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                    return MathHelper.Lerp(0f, trailWidth, num) * 1f;
                }
                else if (progress >= 0.5)
                {
                    float num = 1f;
                    float lerpValue = Utils.GetLerpValue(0f, 0.3f, 1 - progress, clamped: true);
                    num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                    return MathHelper.Lerp(0f, trailWidth, num) * 1f;
                }
            }

            return 1f;
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

   //ball7
   //orange grad
   //noise
   //Trail2
   //Rotating in opposite directions
   //Core in center, 2 black in back, one solid in middle and one bigger but faded
}