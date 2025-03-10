using Microsoft.Xna.Framework;
using System;

namespace AerovelenceMod.Common.Utilities
{
    /// <summary>
    /// Provides various utilities for converting and manipulating colors between RGB and HSV formats,
    /// along with helpful methods to generate color schemes and gradients.
    /// </summary>
    public static class ColorUtils
    {
        /// <summary>
        /// Converts an RGB Color to an HSV vector (Hue, Saturation, Value).
        /// Hue range: [0, 1], Saturation range: [0, 1], Value range: [0, 1].
        /// </summary>
        /// <param name="rgb">The RGB color to convert.</param>
        /// <returns>A Vector3 representing HSV values.</returns>
        public static Vector3 RgbToHsv(Color rgb)
        {
            float r = rgb.R / 255f;
            float g = rgb.G / 255f;
            float b = rgb.B / 255f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            float h = 0;
            if (delta > 0)
            {
                if (max == r)
                    h = ((g - b) / delta) % 6f;
                else if (max == g)
                    h = ((b - r) / delta) + 2f;
                else if (max == b)
                    h = ((r - g) / delta) + 4f;

                h /= 6f;
                if (h < 0) h += 1f;
            }

            float s = max == 0 ? 0 : delta / max;
            float v = max;

            return new Vector3(h, s, v);
        }

        /// <summary>
        /// Converts an HSV vector to an RGB Color.
        /// </summary>
        /// <param name="hsv">HSV vector with Hue[0-1], Saturation[0-1], Value[0-1]</param>
        /// <returns>The corresponding RGB Color.</returns>
        public static Color HsvToRgb(Vector3 hsv)
        {
            float h = hsv.X * 6f;
            float s = hsv.Y;
            float v = hsv.Z;

            int i = (int)Math.Floor(h);
            float f = h - i;
            float p = v * (1 - s);
            float q = v * (1 - s * f);
            float t = v * (1 - s * (1 - f));

            switch (i % 6)
            {
                case 0: return new Color(v, t, p);
                case 1: return new Color(q, v, p);
                case 2: return new Color(p, v, t);
                case 3: return new Color(p, q, v);
                case 4: return new Color(t, p, v);
                default: return new Color(v, p, q);
            }
        }

        /// <summary>
        /// Shifts the hue of a given Color by a specified amount.
        /// </summary>
        /// <param name="baseColor">Original RGB Color</param>
        /// <param name="hueShift">Amount to shift hue (0 to 1, where 1 is 360 degrees)</param>
        public static Color ShiftHue(Color baseColor, float hueShift)
        {
            Vector3 hsv = RgbToHsv(baseColor);
            hsv.X = (hsv.X + hueShift) % 1f;
            return HsvToRgb(hsv);
        }

        /// <summary>
        /// Adjusts the saturation of a given color by a factor.
        /// </summary>
        /// <param name="baseColor">The original color.</param>
        /// <param name="brightnessFactor">Multiplier to adjust saturation (0-1 decreases, >1 increases).</param>
        public static Color AdjustSaturation(Color baseColor, float saturationFactor)
        {
            Vector3 hsv = RgbToHsv(baseColor);
            hsv.Y = Math.Clamp(hsv.Y * saturationFactor, 0f, 1f);
            return HsvToRgb(hsv);
        }

        /// <summary>
        /// Adjusts the brightness (value) of a given color by a specified factor.
        /// </summary>
        public static Color AdjustBrightness(Color baseColor, float brightnessFactor)
        {
            Vector3 hsv = RgbToHsv(baseColor);
            hsv.Z = Math.Clamp(hsv.Z * brightnessFactor, 0f, 1f);
            return HsvToRgb(hsv);
        }

        /// <summary>
        /// Creates a smooth rainbow color gradient based on a float value between 0 and 1.
        /// </summary>
        public static Color Rainbow(float value)
        {
            return HsvToRgb(new Vector3(value % 1f, 1f, 1f));
        }

        /// <summary>
        /// Calculates the complementary color (oppsite on the color wheel).
        /// </summary>
        public static Color GetComplementary(Color baseColor)
        {
            Vector3 hsv = RgbToHsv(baseColor);
            hsv.X = (hsv.X + 0.5f) % 1f;
            return HsvToRgb(hsv);
        }

        /// <summary>
        /// Generates two analogous colors (adjacent on the color wheel) from the given base color.
        /// </summary>
        public static (Color, Color) GetAnalogous(Color baseColor, float angle = 0.083f)
        {
            Vector3 hsv = RgbToHsv(baseColor);
            Color analog1 = HsvToRgb(new Vector3((hsv.X + angle) % 1f, hsv.Y, hsv.Z));
            Color analog2 = HsvToRgb(new Vector3((hsv.X - angle + 1f) % 1f, hsv.Y, hsv.Z));
            return (analog1, analog2);
        }

        /// <summary>
        /// Generates a triadic color scheme from a base color.
        /// </summary>
        public static (Color, Color) GetTriadic(Color baseColor)
        {
            Vector3 hsv = RgbToHsv(baseColor);
            Color triad1 = HsvToRgb(new Vector3((hsv.X + 1f / 3f) % 1f, hsv.Y, hsv.Z));
            Color triad2 = HsvToRgb(new Vector3((hsv.X + 2f / 3f) % 1f, hsv.Y, hsv.Z));
            return (triad1, triad2);
        }

        /// <summary>
        /// Linearly interpolates between two colors through HSV space to ensure smooth color transitions.
        /// </summary>
        public static Color LerpHSV(Color a, Color b, float amount)
        {
            Vector3 hsvA = RgbToHsv(a);
            Vector3 hsvB = RgbToHsv(b);

            float hueDiff = hsvB.X - hsvA.X;
            if (hueDiff > 0.5f) hsvA.X += 1f;
            else if (hueDiff < -0.5f) hsvB.X += 1f;

            Vector3 result = Vector3.Lerp(hsvA, hsvB, amount);
            result.X %= 1f;

            Color rgbResult = HsvToRgb(result);
            rgbResult.A = (byte)MathHelper.Lerp(a.A, b.A, amount);

            return rgbResult;
        }
    }
}