using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent;
using ReLogic.Content;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.NPCs.Bosses.Rimegeist;
using Terraria.Graphics;

namespace AerovelenceMod.Content.Items
{
	public class TrailTest : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("TrailTest");
			ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 99999999;
		}
		public override void SetDefaults()
		{
			Projectile.tileCollide = false;
			Projectile.damage = 0;
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 4000;
			Projectile.penetrate = -1;
		}

		public int trailPointLimit = 60;

        public int trailWidth = 10;

        public float trailMaxLength = 200;

        public float trailCurrentLength;

        public float fadeOut = 1;

        public float trailYOffset = 0;

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

        public void Initialize()
        {
            if (!initialized)
            {
                trailPositions = new List<Vector2>();
                trailRotations = new List<float>();

                initialized = true;
            }
        }

        public override void AI()
        {
            Initialize();

            Projectile.rotation = Projectile.velocity.ToRotation();
            //Projectile.Center = Main.MouseWorld;

            Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX) * 7;

            //Projectile.Center = hostEntity.Center;

            trailPositions.Add(Projectile.Center);
            trailRotations.Add((Main.MouseWorld - Projectile.Center).ToRotation());



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

        public float CalculateLength()
        {
            float calculatedLength = 0;
            for (int i = 0; i < trailPositions.Count - 2; i++)
            {
                calculatedLength += Vector2.Distance(trailPositions[i], trailPositions[i + 1]);
            }

            return calculatedLength;
        }

        BasicEffect basicEffect;

        public override bool PreDraw(ref Color lightColor) 
		{
            Utils.DrawLine(Main.spriteBatch, Projectile.Center + new Vector2(-10,0).RotatedBy(Projectile.rotation), Main.player[Projectile.owner].Center, Color.White, Color.White, 2);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            if (basicEffect == null)
            {
                basicEffect = new BasicEffect(Main.graphics.GraphicsDevice);
                basicEffect.VertexColorEnabled = true;
                basicEffect.FogEnabled = false;
                basicEffect.View = Main.GameViewMatrix.TransformationMatrix;
                var viewport = Main.instance.GraphicsDevice.Viewport;
                basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, -1, 1);
            }

            basicEffect.World = Matrix.CreateTranslation(-new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0));
            basicEffect.CurrentTechnique.Passes[0].Apply();

            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(trailPositions.ToArray(), trailRotations.ToArray(), ColorFunction, WidthFunction, Vector2.Zero, includeBacksides: true);
            vertexStrip.DrawTrail();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D GooglyEye = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/GooglyEye");

            if (Projectile.timeLeft > 2 && Projectile.timeLeft < 395)
            {
                Main.spriteBatch.Draw(GooglyEye, Projectile.Center + new Vector2(-40, 6.5f).RotatedBy(trailRotations[trailRotations.Count - 5]) - Main.screenPosition,
                    GooglyEye.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, GooglyEye.Size() / 2, 0.75f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(GooglyEye, Projectile.Center + new Vector2(-40, -6.5f).RotatedBy(trailRotations[trailRotations.Count - 5]) - Main.screenPosition,
                    GooglyEye.Frame(1, 1, 0, 0), Color.White, Projectile.rotation, GooglyEye.Size() / 2, 0.75f, SpriteEffects.None, 0f);
            }


            return false;
		}

        public virtual float WidthFunction(float progress)
        {
            if (progress < 0.5f)
            {
                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, 30f, num) * 0.4f + Main.rand.NextFloat(0, 2);
            } 
            else if (progress >= 0.5 && progress < 0.9f)
            {
                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.6f, 1 - progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, 30f, num) * 0.4f + Main.rand.NextFloat(0, 2);
            } else if (progress >= 0.9)
            {
                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.6f, 1 - progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, 30f, num) * 0.2f + Main.rand.NextFloat(0, 2);
            }

            return 0f;

            /*
            if (progress < 0.5f)
            {
                float num = 0.5f;
                float lerpValue = Utils.GetLerpValue(0f, 0.6f, progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, 30f, num);
            } 
            else
            {
                float num = 1f;
                float lerpValue = Utils.GetLerpValue(0f, 0.6f, 1 - progress, clamped: true);
                num *= 1f - (1f - lerpValue) * (1f - lerpValue);
                return MathHelper.Lerp(0f, 30f, num);
            }
            */

            //return 1 + Main.rand.NextFloat(10,20);
            //return trailWidth;
        }

        public virtual Color ColorFunction(float progress)
        {
            return Color.DeepPink;
        }
    }
    /*
    public class WaterTrail : ModProjectile
    {

    }
    */
}

