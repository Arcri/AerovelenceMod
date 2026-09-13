using System;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Terraria;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    internal static class TumblerConductiveSequence
    {
        internal const int ChargeEnd = 90;
        internal const int Rest = 180;
        internal const int AimTime = 70;
        internal const int ShotCount = 3;
        internal static int WarningTime(int shot) => ChargeEnd + Rest + shot * 40;
        internal static int Side(int shot) => shot == 1 ? 1 : -1;

        internal static bool Supplying(float timer, int side)
        {
            if (timer >= 45f && timer < ChargeEnd)
                return true;
            for (int shot = 0; shot < ShotCount; shot++)
                if (Side(shot) == side && timer >= WarningTime(shot) && timer < WarningTime(shot) + AimTime)
                    return true;
            return false;
        }

        internal static float Charge(float timer, int side)
        {
            if (timer < ChargeEnd)
                return MathHelper.Clamp((timer - 45f) / 45f, 0f, 1f);
            for (int shot = ShotCount - 1; shot >= 0; shot--)
                if (Side(shot) == side && timer >= WarningTime(shot) && timer < WarningTime(shot) + AimTime)
                    return MathHelper.Lerp(0.35f, 1f, (timer - WarningTime(shot)) / AimTime);
            return timer < WarningTime(ShotCount - 1) + AimTime ? 0.35f : 0f;
        }
    }

    public partial class CrystalTumbler
    {
        private void ConductiveFloorRoll()
        {
            RollTowardPlayer(PhaseTwo ? 7f : 6f, 0.16f);
            if (NPC.Bottom.Y < FloorY - 2f)
            {
                NPC.noTileCollide = true;
                if (NPC.velocity.Y >= 0f && NPC.Bottom.Y + Math.Max(1f, NPC.velocity.Y) >= FloorY)
                {
                    NPC.Bottom = new Vector2(NPC.Center.X, FloorY);
                    NPC.velocity.Y = 0f;
                    NPC.noTileCollide = false;
                }
            }
        }

        private void SpawnFenceUpperBolts()
        {
            float left = LeftOuter + 60f;
            float width = RightOuter - 60f - left;
            for (int lane = 0; lane < 3; lane++)
            {
                float y = FloorY - 125f - lane * 100f;
                if (y < ArenaData.WorldBounds.Top + 100f)
                    continue;
                SpawnProjectile<TumblerLightningBolt>(new Vector2(left, y), new Vector2(width, 0f), ProjectileDamage(17), 0f, 100f, PhaseTwo ? 1f : 0f, -75f);
            }
        }

        private void WarnLoopSlam()
        {
            int rings = Main.expertMode ? 5 : 3;
            float center = ArenaData.ArenaCenter.X;
            for (int ring = 0; ring < rings; ring++)
                for (int side = -1; side <= 1; side += 2)
                {
                    float x = center + side * (80f + ring * (RightInner - center - 100f) / (rings - 1));
                    Vector2 source = new(x, ArenaData.WorldBounds.Top + 120f);
                    SpawnProjectile<TumblerLightningBolt>(source, new Vector2(0f, FloorY - source.Y), ProjectileDamage(19), 0f, 90f, PhaseTwo ? 1f : 0f, NPC.whoAmI + 1);
                }
        }
    }
}
