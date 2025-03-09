using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using AerovelenceMod.Common.Systems;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using ReLogic.Content;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class WaterGeyser : ModProjectile
    {
        private const int MAX_SEGMENTS = 20;
        private Vector2[] segmentPositions;
        private Vector2 startPoint;
        private Vector2 targetPoint;
        private float timeSinceSpawn = 0f;
        private bool initialized = false;
        private int timer = 0;
        private Texture2D slashTexture;
        private float maxHeight = 200f;

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            slashTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Slash/pixelKennySlash").Value;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            timer++;
            if (!initialized)
            {
                initialized = true;
                startPoint = Projectile.Center;
                targetPoint = startPoint - new Vector2(0, 5);
                segmentPositions = new Vector2[MAX_SEGMENTS];
                for (int i = 0; i < MAX_SEGMENTS; i++)
                {
                    float t = i / (float)(MAX_SEGMENTS - 1);
                    segmentPositions[i] = Vector2.Lerp(startPoint, targetPoint, t);
                }
            }

            timeSinceSpawn += 1f;
            float currentHeight = startPoint.Y - targetPoint.Y;
            if (currentHeight < maxHeight)
            {
                targetPoint.Y -= 1.5f;
            }
            else
            {

            }
            for (int i = 0; i < MAX_SEGMENTS; i++)
            {
                float t = i / (float)(MAX_SEGMENTS - 1);
                Vector2 basePos = Vector2.Lerp(startPoint, targetPoint, t);
                Vector2 direction = targetPoint - startPoint;
                if (direction != Vector2.Zero)
                    direction.Normalize();
                Vector2 perp = new Vector2(-direction.Y, direction.X);
                float offset = (float)Math.Sin(timeSinceSpawn * 0.2f + t * 10f) * 5f;
                segmentPositions[i] = basePos + perp * offset;
            }

            if (Main.rand.NextBool(10))
            {
                int seg = Main.rand.Next(MAX_SEGMENTS);
                Dust.NewDustPerfect(segmentPositions[seg], DustID.Electric, Vector2.Zero, 0, Color.Cyan, 1f).noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/CheapScroll", AssetRequestMode.ImmediateLoad).Value;

            #region Shader Params
            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/RealLightning").Value);

            Color c1 = Color.Aqua * ((255 - Projectile.alpha) / 255f);
            Color c2 = Color.AliceBlue * ((255 - Projectile.alpha) / 255f);

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color1Mult"].SetValue(1f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["totalMult"].SetValue(1f);

            myEffect.Parameters["tex1reps"].SetValue(0.25f);
            myEffect.Parameters["tex2reps"].SetValue(0.25f);
            myEffect.Parameters["satPower"].SetValue(1f);
            myEffect.Parameters["time1Mult"].SetValue(1f);
            myEffect.Parameters["time2Mult"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.018f);
            #endregion

           
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            myEffect.CurrentTechnique.Passes[0].Apply();

            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            var originalBlendState = Main.spriteBatch.GraphicsDevice.BlendState;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, originalBlendState, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            float slashScale = 0.2f + (float)Math.Sin(timer * 0.013f) * 0.1f;
            Vector2 drawPosition = targetPoint - Main.screenPosition;
            slashTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Slash/pixelKennySlash").Value;
            Main.spriteBatch.Draw(slashTexture, drawPosition, null, Color.AliceBlue * 0.8f, 0f, slashTexture.Size() / 2, slashScale, SpriteEffects.None, 0f);

            PixellationSystem.QueuePixelationAction(() =>
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                Rectangle sourceRect = new(0, 0, 1, 1);
                Texture2D pixel = TextureAssets.MagicPixel.Value;
                slashTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Slash/pixelKennySlash").Value;
                for (int i = 0; i < MAX_SEGMENTS - 1; i++)
                {
                    Vector2 start = (segmentPositions[i] - Main.screenPosition) / 2;
                    Vector2 end = (segmentPositions[i + 1] - Main.screenPosition) / 2;
                    Vector2 diff = end - start;
                    float rotation = diff.ToRotation();
                    float length = diff.Length();
                    // Draw the core line
                    spriteBatch.Draw(
                        pixel,
                        start,
                        sourceRect,
                        Color.DarkBlue,
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(length, 1f),
                        SpriteEffects.None,
                        0
                    );

                    spriteBatch.Draw(
                        pixel,
                        start,
                        sourceRect,
                        Color.DarkBlue,
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(length, 2f),
                        SpriteEffects.None,
                        0
                    );

                    spriteBatch.Draw(
                        pixel,
                        start,
                        sourceRect,
                        Color.Blue * 0.5f,
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(length, 5f),
                        SpriteEffects.FlipVertically,
                        0
                    );

                    spriteBatch.Draw(
                        slashTexture,
                        start,
                        sourceRect,
                        Color.Blue * 0.5f,
                        rotation,
                        new Vector2(0, 0.5f),
                        new Vector2(length, 5f),
                        SpriteEffects.FlipVertically,
                        0
                    );

                    float slashScale = 0.8f + (float)Math.Sin(timer * 0.05f) * 0.1f;

                    spriteBatch.Draw(slashTexture, start, sourceRect, Color.AliceBlue * 0.8f, 0f, slashTexture.Size() / 2, slashScale, SpriteEffects.None, 0f);
                }
            }, PixellationSystem.RenderType.Additive);
            return false;
        }
    }
}