using System;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Terraria;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public partial class CrystalTumbler
    {
        private int edgeChargeCooldown;
        private float volleyAnchorX;
        private float PassiveLeft => LeftInner + NPC.width * 0.5f;
        private float PassiveRight => RightInner - NPC.width * 0.5f;

        private int TargetEdgeDirection()
        {
            if (Target.Center.X < LeftOuter + 200f)
                return -1;
            return Target.Center.X > RightOuter - 200f ? 1 : 0;
        }

        private bool TryBeginEdgeCharge()
        {
            int edge = TargetEdgeDirection();
            if (!IsServer || edgeChargeCooldown > 0 || edge == 0 || phaseTransitionQueued)
                return false;
            ChangeState(TumblerState.CrystalRun);
            storedDirection = edge;
            edgeChargeCooldown = 600;
            distantTimer = 0;
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.ModProjectile is not (TumblerAimLine or TumblerKnifeBall or TumblerStar or TumblerGuidedShard or ElectricBolt or CrystalShard))
                    continue;
                if (projectile.TryGetGlobalProjectile(out TumblerSharedProjectile shared) && !shared.FromEncounter)
                    continue;
                TumblerProjectileRetirement.Begin(projectile);
            }
            NPC.netUpdate = true;
            return true;
        }

        private float FindVolleyAnchor()
        {
            float best = ArenaData.ArenaCenter.X;
            float bestClearance = -1f;
            for (int i = -1; i <= 1; i++)
            {
                float candidate = MathHelper.Clamp(ArenaData.ArenaCenter.X + i * 340f, PassiveLeft, PassiveRight);
                float clearance = VolleyClearance(candidate);
                if (clearance > bestClearance || clearance == bestClearance && Math.Abs(candidate - NPC.Center.X) < Math.Abs(best - NPC.Center.X))
                {
                    best = candidate;
                    bestClearance = clearance;
                }
            }
            return best;
        }

        private static float VolleyClearance(float sourceX)
        {
            float clearance = float.MaxValue;
            foreach (Player player in Main.ActivePlayers)
            {
                if (!player.dead && ArenaData.WorldBounds.Intersects(player.Hitbox))
                    clearance = Math.Min(clearance, Math.Abs(player.Center.X - sourceX));
            }
            return clearance;
        }

        private static bool HasVolleyClearance(Vector2 source, float distance) => VolleyClearance(source.X) >= distance;

        private void SpawnVolleyOrb(Vector2 position, Vector2 velocity, int damage, bool charged)
        {
            if (!IsServer)
                return;
            position.X = MathHelper.Clamp(position.X, PassiveLeft, PassiveRight);
            if (!HasVolleyClearance(position, 280f))
                position.X = FindVolleyAnchor();
            if (!HasVolleyClearance(position, 280f))
                return;
            if (charged)
                SpawnProjectile<TumblerChargedKnifeBall>(position, velocity, damage);
            else
                SpawnProjectile<TumblerKnifeBall>(position, velocity, damage);
        }
    }
}
