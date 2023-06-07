using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace AerovelenceMod.Common.Utilities
{
    internal static class Easings
    {
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




        public static float easeOutQuint(float progress)
        {
            float toReturn = 0f;
            toReturn = 1f - MathF.Pow(1f - progress, 5f);
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


    }
}
