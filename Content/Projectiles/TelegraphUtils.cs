using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Projectiles
{
    /// <summary>
    /// Utility class for managing and drawing telegraph lines, commonly used for attack indicators.
    /// Supports static or dynamic positioning, rotation easing, fading, and glow effects.
    /// </summary>
    public static class TelegraphUtility
    {
        /// <summary>
        /// Stores all active telegraphs with unique IDs.
        /// </summary>
        private static Dictionary<int, TelegraphData> activeTelegraphs = new Dictionary<int, TelegraphData>();

        /// <summary>
        /// Counter for generating unique telegraph IDs.
        /// </summary>
        private static int nextTelegraphID = 0;

        /// <summary>
        /// Creates and registers a new telegraph line.
        /// </summary>
        /// <param name="getStartPosition">A function that returns the start position of the telegraph. Allows dynamic positioning.</param>
        /// <param name="length">The maximum length of the telegraph.</param>
        /// <param name="direction">The initial direction the telegraph should face.</param>
        /// <param name="fadeStrength">Initial opacity strength of the telegraph.</param>
        /// <param name="rotationEasing">Angle variation for rotation easing (in degrees).</param>
        /// <param name="focus">If true, the rotation will gradually adjust toward the target direction.</param>
        /// <param name="aimAtTarget">If true, the telegraph's rotation will dynamically update towards the target.</param>
        /// <param name="getTargetPosition">Gets the targeted position from the direction.</param>
        /// <param name="color">The color of the telegraph.</param>
        /// <param name="extraGlowFX">If true, an additional glow effect will be drawn with a hue shift.</param>
        /// <param name="isStatic">If true, the telegraph will not update its position.</param>
        /// <param name="lifetime">Duration of the telegraph in ticks (1 tick = 1/60 second).</param>
        /// <returns>A unique ID representing the telegraph.</returns>
        public static int DrawTelegraph(Func<Vector2> getStartPosition, float length, Vector2 direction, float fadeStrength, float rotationEasing, bool focus, bool aimAtTarget, Func<Vector2>? getTargetPosition, Color color, bool extraGlowFX, bool isStatic, float lifetime = 60f)
        {
            int id = nextTelegraphID++;
            activeTelegraphs[id] = new TelegraphData(getStartPosition, length, direction, fadeStrength, rotationEasing, focus, aimAtTarget, getTargetPosition, color, extraGlowFX, isStatic, lifetime);
            return id;
        }

        /// <summary>
        /// Updates all active telegraphs, handling position updates, fade-out effects, and rotation easing.
        /// </summary>
        public static void UpdateTelegraphs()
        {
            List<int> toRemove = new List<int>();

            foreach (var kvp in activeTelegraphs)
            {
                var id = kvp.Key;
                var telegraph = kvp.Value;

                telegraph.Lifetime -= 1f;
                if (telegraph.Lifetime <= 0)
                {
                    toRemove.Add(id);
                    continue;
                }

                if (telegraph.AimAtTarget && telegraph.GetTargetPosition != null)
                {
                    Vector2 targetPos = telegraph.GetTargetPosition();
                    if (targetPos != Vector2.Zero)
                    {
                        telegraph.Direction = Vector2.Normalize(targetPos - telegraph.Start);
                    }
                }
                if (!telegraph.IsStatic)
                {
                    try
                    {
                        telegraph.Start = telegraph.GetStartPosition();
                    }
                    catch (Exception ex)
                    {
                        Main.NewText($"[Aerovelence] Failed to update position: {ex.Message}. Please report this!", Color.Red);
                        telegraph.Start = Vector2.Zero;
                    }
                }
                telegraph.FadeOut = MathHelper.Lerp(telegraph.FadeStrength, 0f, 1f - (telegraph.Lifetime / telegraph.MaxLifetime));
                if (telegraph.Focus)
                {
                    telegraph.RotationOffset = MathHelper.Lerp(telegraph.RotationOffset, 0f, 0.05f);
                }

                activeTelegraphs[id] = telegraph;
            }

            foreach (int id in toRemove)
            {
                activeTelegraphs.Remove(id);
            }
        }

        /// <summary>
        /// Draws all active telegraphs.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch instance used for drawing.</param>
        public static void DrawAllTelegraphs(SpriteBatch spriteBatch)
        {
            Texture2D lineTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/Medusa_Gray").Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/AnotherLineGlow").Value;

            foreach (var telegraph in activeTelegraphs.Values)
            {
                float fade = telegraph.FadeOut;
                Vector2 end = telegraph.Start + (telegraph.Direction.RotatedBy(telegraph.RotationOffset) * telegraph.Length);
                DrawTelegraphLine(spriteBatch, telegraph.Start, end, lineTexture, telegraph.Color * fade, telegraph.Length);
                if (telegraph.ExtraGlowFX)
                {
                    Color glowColor = ShiftHue(telegraph.Color, 20) * fade;
                    DrawTelegraphLine(spriteBatch, telegraph.Start, end, glowTexture, glowColor, telegraph.Length * 0.5f);
                }
            }
        }

        /// <summary>
        /// Draws a telegraph line between two points.
        /// </summary>
        private static void DrawTelegraphLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Texture2D texture, Color color, float length)
        {
            Vector2 direction = end - start;
            float rotation = (float)Math.Atan2(direction.Y, direction.X);
            Vector2 scale = new Vector2(length / texture.Width, 1f);

            spriteBatch.Draw(texture, start - Main.screenPosition, null, color, rotation, new Vector2(0, texture.Height / 2f), scale, SpriteEffects.None, 0f);
        }

        /// <summary>
        /// Shifts the hue of a color by a given amount.
        /// </summary>
        private static Color ShiftHue(Color color, float hueShift)
        {
            float h, s, v;
            ToHSV(color, out h, out s, out v);
            h = (h + hueShift / 360f) % 1f;
            return FromHSV(h, s, v);
        }

        private static void ToHSV(Color color, out float h, out float s, out float v)
        {
            float r = color.R / 255f;
            float g = color.G / 255f;
            float b = color.B / 255f;
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            v = max;

            float delta = max - min;
            s = max == 0 ? 0 : delta / max;
            if (max == min) { h = 0; }
            else
            {
                if (max == r) h = (g - b) / delta + (g < b ? 6 : 0);
                else if (max == g) h = (b - r) / delta + 2;
                else h = (r - g) / delta + 4;
                h /= 6;
            }
        }

        private static Color FromHSV(float h, float s, float v)
        {
            int hi = (int)(h * 6) % 6;
            float f = h * 6 - hi;
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);

            return hi switch
            {
                0 => new Color(v, t, p),
                1 => new Color(q, v, p),
                2 => new Color(p, v, t),
                3 => new Color(p, q, v),
                4 => new Color(t, p, v),
                _ => new Color(v, p, q),
            };
        }

        /// <summary>
        /// Stores data related to a telegraph line.
        /// </summary>
        private class TelegraphData(Func<Vector2> getStartPosition, float length, Vector2 direction, float fadeStrength, float rotationEasing, bool focus, bool aimAtTarget, Func<Vector2>? getTargetPosition, Color color, bool extraGlowFX, bool isStatic, float lifetime)
        {
            public Vector2 Start = getStartPosition();
            public float Length = length;
            public Vector2 Direction = direction;
            public float FadeStrength = fadeStrength;
            public float RotationOffset = MathHelper.ToRadians(Main.rand.NextFloat(-rotationEasing, rotationEasing));
            public bool Focus = focus;
            public Func<Vector2>? GetTargetPosition { get; private set; } = getTargetPosition;
            public bool AimAtTarget { get; private set; } = aimAtTarget;
            public Color Color = color;
            public bool ExtraGlowFX = extraGlowFX;
            public float Lifetime = lifetime;
            public float MaxLifetime = lifetime;
            public float FadeOut = fadeStrength;
            public bool IsStatic = isStatic;
            public Func<Vector2> GetStartPosition = getStartPosition;
        }
    }
}