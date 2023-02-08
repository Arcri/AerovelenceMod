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

namespace AerovelenceMod.Content.Projectiles
{
	public abstract class TrailProjBase : ModProjectile
	{
        //TODO:
        // - Add presets for WidthFunction
        // - Add more options for shaders
        // - Add more stuff to basic trail shader
        //    - Ability to set how many times you want to draw it (better than calling DrawTrail() multiple times
        //    - Glow effect from GlowMisc
        // - Just make it better in general
        // - Actually understand what im doing instead of just using other people's code

        //Heavily based off of tsorc trail system 

        #region variables
        //Set this to like Projectile.rotation before calling trail logic
        public float trailRot = 0;

        //Same but with Center
        public Vector2 trailPos = Vector2.Zero;

        public Color trailColor = Color.White;

        public float trailTime = 0f;

        public Texture2D trailTexture = null;

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
            return MathHelper.Lerp(0f, 30f, num) * 1f; // 0.3f 
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
            trailPositions.Add(Projectile.Center);
            trailRotations.Add(Projectile.velocity.ToRotation());

            //Smoothing
            if (trailPositions.Count > 3)
            {
                for (int i = 3; i < trailPositions.Count - 2; i++)
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
        public void TrailDrawing()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, customEffect, Main.GameViewMatrix.TransformationMatrix);

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
            customEffect.CurrentTechnique.Passes["DefaultPass"].Apply();
            customEffect.CurrentTechnique.Passes["MainPS"].Apply();

            VertexStrip vertexStrip = new VertexStrip();
            if (trailPositions != null)
            {
                vertexStrip.PrepareStrip(trailPositions.ToArray(), trailRotations.ToArray(), ColorFunction, WidthFunction, -Main.screenPosition, includeBacksides: true);
                vertexStrip.DrawTrail();
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

    }
}