using System;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public sealed class TumblerLightningVisual
    {
        private LightningUtils.LightningData lightning;
        private int timer;
        private int nextStormRefresh;
        private Vector2 previousStart;
        private Vector2 previousEnd;

        public void Update(Projectile owner, Vector2 start, Vector2 end, float intensity = 0.55f, bool staffStyle = false)
        {
            if (Main.dedServ || Vector2.DistanceSquared(start, end) < 1f)
                return;
            if (staffStyle)
            {
                UpdateStorm(owner, start, end, intensity);
                return;
            }
            LightningUtils.LightningStyle style = LightningUtils.LightningStyle.Jagged;
            if (lightning == null || lightning.Style != style)
            {
                lightning = new LightningUtils.LightningData(owner, style)
                {
                    MaxSegments = Math.Clamp((int)(Vector2.Distance(start, end) / 20f), 12, 56),
                    MaxBranches = 3,
                    BranchChance = 0.45f,
                    NoiseFrequency = 1.7f
                };
            }
            timer++;
            if (lightning.Initialized && timer % 3 != 0 && Vector2.DistanceSquared(start, previousStart) < 16f && Vector2.DistanceSquared(end, previousEnd) < 16f)
                return;
            if (Vector2.DistanceSquared(start, previousStart) > 24f * 24f)
                lightning.Branches?.Clear();
            lightning.DisplacementIntensity = intensity;
            LightningUtils.InitializeBetweenPoints(lightning, start, end, style);
            LightningUtils.UpdateSegments(lightning);
            LightningUtils.UpdateBranches(lightning);
            previousStart = start;
            previousEnd = end;
        }

        private void UpdateStorm(Projectile owner, Vector2 start, Vector2 end, float intensity)
        {
            if (lightning == null || lightning.Style != LightningUtils.LightningStyle.Chaotic)
                lightning = new LightningUtils.LightningData(owner, LightningUtils.LightningStyle.Chaotic) { MaxSegments = 65 };
            timer++;
            if (lightning.Initialized && timer < nextStormRefresh && Vector2.DistanceSquared(start, previousStart) < 10000f && Vector2.DistanceSquared(end, previousEnd) < 10000f)
            {
                Vector2 startDelta = start - previousStart;
                Vector2 endDelta = end - previousEnd;
                for (int i = 0; i < lightning.SegmentPositions.Length; i++)
                    lightning.SegmentPositions[i] += Vector2.Lerp(startDelta, endDelta, i / (float)(lightning.SegmentPositions.Length - 1));
                Vector2 direction = previousEnd - previousStart;
                foreach (LightningUtils.Branch branch in lightning.Branches)
                {
                    float progress = MathHelper.Clamp(Vector2.Dot(branch.Positions[0] - previousStart, direction) / Math.Max(1f, direction.LengthSquared()), 0f, 1f);
                    Vector2 offset = Vector2.Lerp(startDelta, endDelta, progress);
                    for (int i = 0; i < branch.Positions.Length; i++)
                        branch.Positions[i] += offset;
                }
                LightningUtils.UpdateBranches(lightning);
            }
            else
            {
                nextStormRefresh = timer + Main.rand.Next(2, 5);
                LightningUtils.InitializeBetweenPoints(lightning, start, end, LightningUtils.LightningStyle.Chaotic);
                lightning.Branches.Clear();
                Vector2 direction = (end - start).SafeNormalize(Vector2.UnitY);
                Vector2 normal = new(-direction.Y, direction.X);
                float displacement = Math.Min(34f, Vector2.Distance(start, end) * 0.075f) * intensity * 0.5f;
                DisplaceMidpoints(lightning.SegmentPositions, 0, lightning.SegmentPositions.Length - 1, normal, displacement);
                int branches = Main.rand.Next(7, 13);
                for (int i = 0; i < branches; i++)
                {
                    int root = Main.rand.Next(3, lightning.SegmentPositions.Length - 4);
                    int count = Main.rand.Next(4, 8);
                    Vector2[] points = new Vector2[count];
                    points[0] = lightning.SegmentPositions[root];
                    Vector2 fork = direction.RotatedBy(Main.rand.NextFloat(0.5f, 1.3f) * (Main.rand.NextBool() ? -1f : 1f));
                    Vector2 forkNormal = new(-fork.Y, fork.X);
                    for (int j = 1; j < count; j++)
                        points[j] = points[j - 1] + fork * Main.rand.NextFloat(5f, 11f) + forkNormal * Main.rand.NextFloat(-7f, 7f);
                    lightning.Branches.Add(new LightningUtils.Branch
                    {
                        Positions = points,
                        Offsets = new float[count],
                        Alpha = Main.rand.NextFloat(0.65f, 1f),
                        LifeTime = 10
                    });
                }
            }
            previousStart = start;
            previousEnd = end;
        }

        private static void DisplaceMidpoints(Vector2[] points, int first, int last, Vector2 normal, float displacement)
        {
            if (last - first < 2)
                return;
            int middle = (first + last) / 2;
            points[middle] = Vector2.Lerp(points[first], points[last], 0.5f) + normal * Main.rand.NextFloat(-displacement, displacement);
            DisplaceMidpoints(points, first, middle, normal, displacement * 0.58f);
            DisplaceMidpoints(points, middle, last, normal, displacement * 0.58f);
        }

        public void Draw(SpriteBatch spriteBatch, Color color, float opacity, float width = 2f)
        {
            if (lightning?.SegmentPositions == null || opacity <= 0f)
                return;
            TumblerLightningSystem.DrawPath(lightning.SegmentPositions, color, opacity, width);
            foreach (LightningUtils.Branch branch in lightning.Branches)
            {
                TumblerLightningSystem.DrawPath(branch.Positions, color, opacity * branch.Alpha * 0.55f, Math.Max(1f, width * 0.55f));
            }
        }
    }
}
