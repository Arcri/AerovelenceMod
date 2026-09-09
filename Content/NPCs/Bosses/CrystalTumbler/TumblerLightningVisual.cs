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
        private Vector2 previousStart;
        private Vector2 previousEnd;

        public void Update(Projectile owner, Vector2 start, Vector2 end, float intensity = 0.55f)
        {
            if (Main.dedServ || Vector2.DistanceSquared(start, end) < 1f)
                return;
            lightning ??= new LightningUtils.LightningData(owner, LightningUtils.LightningStyle.Jagged)
            {
                MaxSegments = Math.Clamp((int)(Vector2.Distance(start, end) / 20f), 12, 56),
                MaxBranches = 3,
                BranchChance = 0.45f,
                NoiseFrequency = 1.7f
            };
            timer++;
            if (lightning.Initialized && timer % 3 != 0 && Vector2.DistanceSquared(start, previousStart) < 16f && Vector2.DistanceSquared(end, previousEnd) < 16f)
                return;
            if (Vector2.DistanceSquared(start, previousStart) > 24f * 24f)
                lightning.Branches?.Clear();
            lightning.DisplacementIntensity = intensity;
            LightningUtils.InitializeBetweenPoints(lightning, start, end, LightningUtils.LightningStyle.Jagged);
            LightningUtils.UpdateSegments(lightning);
            LightningUtils.UpdateBranches(lightning);
            previousStart = start;
            previousEnd = end;
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
