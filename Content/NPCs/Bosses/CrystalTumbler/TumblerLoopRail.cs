using System;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerLoopRail : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        private static readonly Vector2[] curve = BuildCurve();
        private static readonly float[] distances = BuildDistances();
        internal const int RideDuration = 135;

        internal static float StartX => ArenaData.ArenaCenter.X - 355f;

        internal static float RideProgress(float ticks)
        {
            float time = MathHelper.Clamp(ticks / RideDuration, 0f, 1f);
            return time * (0.82f + 0.18f * time);
        }

        private static Vector2[] BuildCurve()
        {
            Vector2[] controls =
            [
                Vector2.Zero, new(190f, -18f), new(510f, -65f), new(570f, -205f),
                new(630f, -345f), new(530f, -400f), new(455f, -400f),
                new(380f, -400f), new(355f, -365f), new(355f, -315f)
            ];
            Vector2[] points = new Vector2[193];
            for (int i = 0; i < points.Length; i++)
            {
                int section = Math.Min(i / 64, 2);
                float t = (i - section * 64) / 64f;
                float s = 1f - t;
                int index = section * 3;
                points[i] = controls[index] * (s * s * s) + controls[index + 1] * (3f * s * s * t)
                    + controls[index + 2] * (3f * s * t * t) + controls[index + 3] * (t * t * t);
            }
            return points;
        }

        private static float[] BuildDistances()
        {
            float[] result = new float[curve.Length];
            for (int i = 1; i < result.Length; i++)
                result[i] = result[i - 1] + Vector2.Distance(curve[i - 1], curve[i]);
            return result;
        }

        public static Vector2 Point(Vector2 start, float progress)
        {
            float distance = MathHelper.Clamp(progress, 0f, 1f) * distances[^1];
            int upper = Array.BinarySearch(distances, distance);
            if (upper < 0)
                upper = ~upper;
            upper = Math.Clamp(upper, 1, curve.Length - 1);
            float fraction = (distance - distances[upper - 1]) / (distances[upper] - distances[upper - 1]);
            Vector2 offset = Vector2.Lerp(curve[upper - 1], curve[upper], fraction);
            offset.Y *= MathHelper.Clamp((start.Y - ArenaData.WorldBounds.Top - 80f) / 400f, 0.55f, 1f);
            return start + offset;
        }

        internal static Vector2 Tangent(Vector2 start, float progress)
        {
            return (Point(start, Math.Min(1f, progress + 0.002f)) - Point(start, Math.Max(0f, progress - 0.002f))).SafeNormalize(Vector2.UnitX);
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.timeLeft = 450;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
        }

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1400;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            int bossIndex = (int)Projectile.ai[0];
            if (bossIndex < 0 || bossIndex >= Main.maxNPCs || !Main.npc[bossIndex].active || Main.npc[bossIndex].ModNPC is not CrystalTumbler boss)
            {
                Projectile.Kill();
                return;
            }
            if (boss.LoopRailFinished)
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 75);
            else
                Projectile.ai[1] = boss.LoopRailProgress;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float progress = Projectile.ai[1];
            float opacity = MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(Projectile.timeLeft / 75f, 0f, 1f));
            int segments = Math.Max(2, (int)(progress * 108f));
            Vector2[] outer = new Vector2[segments + 1];
            Vector2[] inner = new Vector2[segments + 1];
            for (int i = 0; i <= segments; i++)
            {
                float step = progress * i / segments;
                Vector2 point = Point(Projectile.Center, step);
                Vector2 tangent = Tangent(Projectile.Center, step);
                Vector2 normal = new(-tangent.Y, tangent.X);
                outer[i] = point + normal * 52f;
                inner[i] = point + normal * 66f;
                if (i > 0 && i % 3 == 0)
                {
                    Vector2[] brace = [outer[i - 3], inner[i], outer[i]];
                    TumblerLightningSystem.DrawPath(brace, TumblerVFX.PhaseColor(0f), opacity * 0.3f, 1f, false);
                }
            }
            TumblerLightningSystem.DrawPath(outer, TumblerVFX.PhaseColor(1f), opacity * 0.9f, 3f);
            TumblerLightningSystem.DrawPath(inner, TumblerVFX.PhaseColor(0f), opacity * 0.4f, 1.5f);
            TumblerVFX.DrawCharge(Main.spriteBatch, outer[^1] - Main.screenPosition, TumblerVFX.PhaseColor(1f), 1f, 13f, Main.GlobalTimeWrappedHourly, opacity);
            return false;
        }
    }
}
