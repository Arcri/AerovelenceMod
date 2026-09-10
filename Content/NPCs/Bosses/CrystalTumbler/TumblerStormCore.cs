using System;
using AerovelenceMod.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    internal static class TumblerStormCore
    {
        internal static void Draw(Vector2 center, float radius, float opacity, float time, float charge, int seed)
        {
            if (opacity <= 0f || Main.dedServ)
                return;
            Color orange = TumblerVFX.PhaseColor(1f);
            Color gold = new(255, 213, 135);
            float pulse = 1f + MathF.Sin(time * 0.13f) * 0.025f;
            PixellationSystem.QueuePixelationAction(() =>
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                Vector2 position = (center - Main.screenPosition) * 0.5f;
                Texture2D bloom = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64", AssetRequestMode.ImmediateLoad).Value;
                Texture2D shell = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/whiteFireEye", AssetRequestMode.ImmediateLoad).Value;
                Texture2D crown = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/ElectricPopD", AssetRequestMode.ImmediateLoad).Value;
                Effect effect = ModContent.Request<Effect>("AerovelenceMod/Effects/Radial/BoFIrisAlt", AssetRequestMode.ImmediateLoad).Value;
                Texture2D noise = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Noise_1", AssetRequestMode.ImmediateLoad).Value;
                Texture2D gradient = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/FireGrad", AssetRequestMode.ImmediateLoad).Value;
                Texture2D distortion = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Noise/Swirl", AssetRequestMode.ImmediateLoad).Value;
                spriteBatch.Draw(bloom, position, null, orange * (opacity * 0.4f), 0f, bloom.Size() * 0.5f, radius * 2.4f / bloom.Width, SpriteEffects.None, 0f);
                spriteBatch.End();
                effect.Parameters["causticTexture"].SetValue(noise);
                effect.Parameters["gradientTexture"].SetValue(gradient);
                effect.Parameters["distortTexture"].SetValue(distortion);
                effect.Parameters["flowSpeed"].SetValue(0.3f);
                effect.Parameters["vignetteSize"].SetValue(0.3f);
                effect.Parameters["vignetteBlend"].SetValue(0.18f);
                effect.Parameters["distortStrength"].SetValue(0.065f);
                effect.Parameters["xOffset"].SetValue(0f);
                effect.Parameters["squashValue"].SetValue(0f);
                effect.Parameters["uTime"].SetValue(time * 0.009f);
                effect.Parameters["colorIntensity"].SetValue(opacity * (0.65f + charge * 0.25f));
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, effect, Matrix.Identity);
                spriteBatch.Draw(bloom, position, null, Color.White, -time * 0.004f, bloom.Size() * 0.5f, radius * 1.15f / bloom.Width, SpriteEffects.None, 0f);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, null, Matrix.Identity);
                spriteBatch.Draw(shell, position, null, orange * (opacity * 0.62f), time * 0.007f, shell.Size() * 0.5f, radius * 1.32f * pulse / shell.Width, SpriteEffects.None, 0f);
                spriteBatch.Draw(shell, position, null, gold * (opacity * 0.33f), -time * 0.011f, shell.Size() * 0.5f, radius * 1.02f / shell.Width, SpriteEffects.FlipHorizontally, 0f);
                spriteBatch.Draw(crown, position, null, orange * (opacity * 0.52f), -time * 0.005f, crown.Size() * 0.5f, radius * 1.7f / crown.Width, SpriteEffects.None, 0f);
                spriteBatch.Draw(bloom, position, null, gold * (opacity * 0.75f), 0f, bloom.Size() * 0.5f, radius * 0.95f / bloom.Width, SpriteEffects.None, 0f);
                spriteBatch.Draw(bloom, position, null, Color.White * (opacity * 0.9f), 0f, bloom.Size() * 0.5f, radius * 0.45f / bloom.Width, SpriteEffects.None, 0f);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Texture, BlendState.Additive, Main.DefaultSamplerState, default, Main.Rasterizer, null, Matrix.Identity);
            }, PixellationSystem.RenderType.Additive);

            for (int strand = 0; strand < 5; strand++)
            {
                Vector2[] points = new Vector2[19];
                float angle = time * (strand % 2 == 0 ? 0.015f : -0.012f) + strand * MathHelper.TwoPi / 5f;
                for (int i = 0; i < points.Length; i++)
                {
                    float p = i / (float)(points.Length - 1);
                    float orbit = angle + p * (2.6f + strand * 0.13f);
                    float distance = radius * (0.12f + p * 0.85f);
                    float jitter = MathF.Sin(i * 8.1f + (int)(time / 3f) + strand * 4f) * 3f * p;
                    points[i] = center + orbit.ToRotationVector2() * (distance + jitter);
                }
                TumblerLightningSystem.DrawPath(points, strand % 2 == 0 ? gold : orange, opacity * 0.7f, 1.6f, true, bloom: 0.15f);
            }
            for (int arc = 0; arc < 3; arc++)
            {
                Vector2[] points = new Vector2[23];
                for (int i = 0; i < points.Length; i++)
                {
                    float angle = time * 0.012f + arc * MathHelper.TwoPi / 3f + i / 22f * 1.2f;
                    float wave = MathF.Sin(i * 4.7f + (int)(time / 3f) + seed) * 3f;
                    points[i] = center + angle.ToRotationVector2() * (radius * 1.14f + wave);
                }
                TumblerLightningSystem.DrawPath(points, gold, opacity * 0.8f, 2f, true, bloom: 0.25f);
            }
        }
    }
}
