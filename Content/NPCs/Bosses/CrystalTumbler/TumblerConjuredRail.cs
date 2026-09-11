using System;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Terraria;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    internal static class TumblerRailMotion
    {
        internal static Vector2 Advance(Func<float, Vector2> point, ref float progress, ref float speed, float drive = 0f)
        {
            Vector2 current = point(progress);
            float sample = Math.Min(1f, progress + 0.002f);
            Vector2 delta = point(sample) - current;
            float distance = delta.Length();
            if (distance <= 0.001f)
            {
                progress = 1f;
                return point(1f);
            }
            float acceleration = delta.Y / distance * 0.12f + drive;
            speed = MathHelper.Clamp(speed + acceleration, 7f, 15f);
            float step = speed * (sample - progress) / distance;
            for (int i = 0; i < 2; i++)
            {
                float next = Math.Min(1f, progress + step);
                float traveled = Vector2.Distance(current, point(next));
                if (traveled <= 0.001f || next >= 1f)
                    break;
                step *= speed / traveled;
            }
            progress = Math.Min(1f, progress + step);
            Vector2 destination = point(progress);
            if (progress >= 1f)
            {
                Vector2 exit = destination - current;
                destination += exit.SafeNormalize(Vector2.UnitX) * Math.Max(0f, speed - exit.Length());
            }
            return destination;
        }
    }

    internal sealed class TumblerConjuredRail
    {
        private const int Sections = 48;
        private readonly ulong[] built = new ulong[Sections];
        private readonly ulong[] consumed = new ulong[Sections];
        private float lastProgress;

        internal void Update(Func<float, Vector2> point, float progress, bool finished)
        {
            lastProgress = Math.Max(lastProgress, MathHelper.Clamp(progress, 0f, 1f));
            float lookAhead = 0f;
            if (!finished)
            {
                float end = Math.Min(1f, lastProgress + 0.01f);
                float distance = Vector2.Distance(point(lastProgress), point(end));
                lookAhead = distance > 0.01f ? 85f * (end - lastProgress) / distance : 0f;
            }
            ulong now = Main.GameUpdateCount + 1;
            for (int i = 0; i < Sections; i++)
            {
                if (built[i] == 0 && !finished && i / (float)Sections <= lastProgress + lookAhead)
                    built[i] = now;
                if (built[i] != 0 && consumed[i] == 0 && (finished || (i + 1f) / Sections <= lastProgress))
                    consumed[i] = now;
            }
        }

        internal void Draw(Func<float, Vector2> point, int direction, float opacity, bool supports)
        {
            ulong now = Main.GameUpdateCount + 1;
            for (int section = 0; section < Sections; section++)
            {
                if (built[section] == 0)
                    continue;
                float age = now - built[section];
                float fade = consumed[section] == 0 ? 1f : MathHelper.Clamp((75f - (now - consumed[section])) / 30f, 0f, 1f);
                float strength = opacity * MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(age / 5f, 0f, 1f)) * fade;
                if (strength < 0.005f)
                    continue;
                Vector2[] upper = new Vector2[5];
                Vector2[] lower = new Vector2[5];
                for (int i = 0; i < upper.Length; i++)
                {
                    float t = (section + i / 4f) / Sections;
                    Vector2 position = point(t);
                    Vector2 tangent = (point(Math.Min(1f, t + 0.001f)) - point(Math.Max(0f, t - 0.001f))).SafeNormalize(Vector2.UnitX);
                    Vector2 normal = new(-tangent.Y * direction, tangent.X * direction);
                    upper[i] = position + normal * 52f;
                    lower[i] = position + normal * 68f;
                }
                TumblerLightningSystem.DrawPath([upper[0], lower[2], upper[4]], TumblerVFX.PhaseColor(0f), strength * 0.35f, 1f, false);
                TumblerLightningSystem.DrawPath(upper, TumblerVFX.PhaseColor(1f), strength * 0.9f, 3f);
                TumblerLightningSystem.DrawPath(lower, TumblerVFX.PhaseColor(0f), strength * 0.45f, 1.5f);
                if (supports && section % 6 == 3)
                {
                    Vector2 anchor = new(lower[2].X, ArenaData.FloorY);
                    TumblerLightningSystem.DrawPath([lower[2], anchor], TumblerVFX.PhaseColor(0f), strength * 0.16f, 1f, false);
                }
                if (age < 10f)
                    TumblerVFX.DrawCharge(Main.spriteBatch, upper[2] - Main.screenPosition, Color.White, 1f, 9f, Main.GlobalTimeWrappedHourly, strength * (1f - age / 10f));
            }
        }
    }
}
