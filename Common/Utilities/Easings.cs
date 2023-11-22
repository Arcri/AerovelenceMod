using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace AerovelenceMod.Common.Utilities
{
    internal static class Easings
    {
        //https://easings.net/ my beloved

        //Sine
        public static float easeInSine(float progress)
        {
            float toReturn = 0f;
            toReturn = 1f - MathF.Cos((progress * MathF.PI) / 2f);
            return toReturn;
        }

        public static float easeOutSine(float progress)
        {
            float toReturn = 0f;
            toReturn = MathF.Sin((progress * MathF.PI) / 2f);
            return toReturn;
        }

        public static float easeInOutSine(float progress)
        {
            float toReturn = 0f;
            toReturn = -(MathF.Cos(MathF.PI * progress) - 1f) / 2f;
            return toReturn;
        }

        //Quad
        public static float easeInQuad(float progress)
        {
            float toReturn = 0f;
            toReturn = progress * progress;
            return toReturn;
        }

        public static float easeOutQuad(float progress)
        {
            float toReturn = 0f;
            toReturn = 1f - (1f - progress) * (1f - progress);
            return toReturn;
        }

        public static float easeInOutQuad(float progress)
        {
            float toReturn = 0f;
            toReturn = progress < 0.5f ? 2f * progress * progress 
                : 1f - MathF.Pow(-2f * progress + 2f, 2f) / 2f;
            return toReturn;
        }

        //Cubic
        public static float easeInCubic(float progress)
        {
            float toReturn = 0f;
            toReturn = progress * progress * progress;
            return toReturn;
        }

        public static float easeOutCubic(float progress)
        {
            float toReturn = 0f;
            toReturn = 1f - MathF.Pow(1f - progress, 3f);
            return toReturn;
        }

        public static float easeInOutCubic(float progress)
        {
            //something feels off here
            float toReturn = 0f;

            toReturn = progress < 0.5f ? 4f * progress * progress * progress 
                : 1f - MathF.Pow(-2f * progress + 2f, 3f) / 2f;
            return toReturn;
        }


        //Quart
        public static float easeInQuart(float progress)
        {
            float toReturn = 0f;
            toReturn = progress * progress * progress * progress;
            return toReturn;
        }

        public static float easeOutQuart(float progress)
        {
            float toReturn = 0f;
            toReturn = 1f - MathF.Pow(1f - progress, 4f);
            return toReturn;
        }

        public static float easeInOutQuart(float progress)
        {
            float toReturn = 0f;

            toReturn = progress < 0.5f ? 8f * progress * progress * progress * progress
                : 1f - MathF.Pow(-2f * progress + 2f, 4f) / 2f;
            return toReturn;
        }

        //Quint
        public static float easeInQuint(float progress)
        {
            float toReturn = 0f;
            toReturn = MathF.Pow(progress, 5f);
            return toReturn;
        }

        public static float easeOutQuint(float progress)
        {
            float toReturn = 0f;
            toReturn = 1f - MathF.Pow(1f - progress, 5f);
            return toReturn;
        }

        public static float easeInOutQuint(float progress)
        {
            float toReturn = 0f;
            toReturn = progress < 0.5 ? 16 * MathF.Pow(progress, 5f) : 
                1f - MathF.Pow(-2f * progress + 2, 5) / 2;
            return toReturn;
        }

        //Expo
        public static float easeInExpo(float progress)
        {
            float toReturn = 0f;
            toReturn = progress == 0f ? 0f : MathF.Pow(2f, 10f * progress - 10f);
            return toReturn;
        }

        public static float easeOutExpo(float progress)
        {
            float toReturn = 0f;
            toReturn = progress == 1f ? 1 : 1f - MathF.Pow(2f, -10f * progress);
            return toReturn;
        }

        public static float easeInOutExpo(float progress)
        {
            float toReturn = 0f;
            toReturn = progress == 0f ? 0f
                : progress == 1f ? 1f
                : progress < 0.5f ? MathF.Pow(2f, 20f * progress - 10f) / 2f
                : (2f - MathF.Pow(2f, -20f * progress + 10)) / 2f;
            return toReturn;
        }

        //Circ
        public static float easeInCirc(float progress)
        {
            float toReturn = 0f;
            toReturn = 1f - MathF.Sqrt(1f - MathF.Pow(progress , 2f));
            return toReturn;
        }

        public static float easeOutCirc(float progress)
        {
            float toReturn = 0f;
            toReturn = MathF.Sqrt(1f - MathF.Pow(progress - 1f, 2f));
            return toReturn;
        }

        public static float easeInOutCirc(float progress)
        {
            float toReturn = 0f;
            toReturn = progress < 0.5f ? (1f - MathF.Sqrt(1f - MathF.Pow(2f * progress, 2f))) / 2f 
                : (MathF.Sqrt(1f - MathF.Pow(-2f * progress + 2f, 2f)) + 1f) / 2f;
            return toReturn;
        }

        //Back
        const float c1 = 1.70158f;
        const float c2 = 1.70158f * 1.525f;
        const float c3 = 1.70158f + 1f;
        public static float easeInBack(float progress)
        {
            float toReturn = 0f;
            toReturn = c3 * progress * progress * progress - c1 * progress * progress;
            return toReturn;
        }

        public static float easeOutBack(float progress)
        {
            float toReturn = 0f;
            toReturn = 1f + c3 * MathF.Pow(progress - 1f, 3f) + c1 * MathF.Pow(progress - 1f, 2f);
            return toReturn;
        }

        public static float easeInOutBack(float progress)
        {
            float toReturn = 0f;
            toReturn = progress < 0.5f
              ? (MathF.Pow(2f * progress, 2f) * ((c2 + 1f) * 2f * progress - c2)) / 2f
              : (MathF.Pow(2f * progress - 2f, 2f) * ((c2 + 1f) * (progress * 2f - 2f) + c2) + 2f) / 2f;
            return toReturn;
        }

        //Misc
        public static float easeInOutHarsh(float progress)
        {
            float toReturn = 0f;
            toReturn = progress * progress / (2f * (progress * progress - progress) + 1f);
            return toReturn;
        }
    }
}
