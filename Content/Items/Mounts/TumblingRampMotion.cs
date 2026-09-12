using System;
using Microsoft.Xna.Framework;

namespace AerovelenceMod.Content.Items.Mounts
{
    internal static class TumblingRampMotion
    {
        internal const float Radius = 16f;

        internal static Vector2 Advance(ref float angle, ref float speed, int direction, int age)
        {
            float incline = MathF.Sin(angle);
            speed = MathHelper.Clamp(speed - incline * 0.13f + 0.025f, 6f, 12f);
            float bend = MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(age / 42f, 0f, 1f));
            float turn = speed / 112f * bend;
            float midpoint = angle + turn * 0.5f;
            angle = (angle + turn) % MathHelper.TwoPi;
            return Tangent(midpoint, direction) * speed;
        }

        internal static Vector2 Tangent(float angle, int direction) => new(direction * MathF.Cos(angle), -MathF.Sin(angle));

        internal static Vector2 Normal(float angle, int direction) => new(direction * MathF.Sin(angle), MathF.Cos(angle));

        internal static Vector2 Launch(float angle, float speed, int direction)
        {
            return Tangent(angle, direction) * MathHelper.Clamp(speed + 1.5f, 7.5f, 13f) - Normal(angle, direction) * 2.5f;
        }
    }
}
