using System;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public partial class CrystalTumbler
    {
        private const float RailLaunchSpeed = 11.5f;
        private float railProgress;
        private float railSpeed;
        internal float CascadeProgress => railProgress;
        internal bool CascadeFinished => State != TumblerState.CascadeRide || substate >= 3;

        private void CascadeRide()
        {
            if (substate == 0)
            {
                float startX = storedDirection > 0 ? LeftInner + 90f : RightInner - 90f;
                MoveHorizontal(startX, 8f, 0.14f);
                if (Math.Abs(NPC.Center.X - startX) > 8f || !OnGround())
                    return;
                rampStart = new Vector2(startX, FloorY - 52f);
                SpawnProjectile<TumblerCascadeRail>(rampStart, Vector2.Zero, 0, 0f, NPC.whoAmI, storedDirection);
                substate = 1;
                StateTimer = 0;
                NPC.netUpdate = true;
            }
            if (substate == 1)
            {
                SpinUp(80, RailLaunchSpeed);
                if (StateTimer < 80 || !OnGround())
                    return;
                substate = 2;
                railSpeed = RailLaunchSpeed;
                StateTimer = 0;
                NPC.netUpdate = true;
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.65f }, NPC.Center);
            }
            if (substate == 2)
            {
                NPC.noGravity = NPC.noTileCollide = true;
                contactDamage = true;
                float previousProgress = railProgress;
                Vector2 destination = TumblerRailMotion.Advance(t => TumblerCascadeRail.Point(rampStart, storedDirection, t), ref railProgress, ref railSpeed);
                NPC.velocity = destination - NPC.Center;
                spinTarget = storedDirection * railSpeed / 52f;
                visualCharge = 0.8f;
                if (previousProgress < 0.5f && railProgress >= 0.5f)
                {
                    KickUpDust(12);
                    SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.6f, Pitch = 0.35f }, NPC.Center);
                }
                if (railProgress >= 1f)
                {
                    substate = 3;
                    StateTimer = 0;
                    NPC.netUpdate = true;
                }
                return;
            }
            if (substate == 3)
            {
                NPC.noGravity = NPC.noTileCollide = false;
                RollTowardPlayer(3.5f, 0.12f);
                if (StateTimer == 1)
                    KickUpDust(12);
                if (StateTimer >= 25)
                    FinishAttack();
            }
        }

        private void RippleSlam()
        {
            if (substate == 0)
            {
                MoveHorizontal(ArenaData.ArenaCenter.X, 7f, 0.12f);
                if (Math.Abs(NPC.Center.X - ArenaData.ArenaCenter.X) > 8f || !OnGround())
                    return;
                rampStart = new Vector2(NPC.Center.X, FloorY - 52f);
                substate = 1;
                StateTimer = 0;
                NPC.netUpdate = true;
            }
            if (substate == 1)
            {
                SpinUp(75, 14f);
                if (StateTimer < 75 || !OnGround())
                    return;
                substate = 2;
                StateTimer = 0;
                NPC.netUpdate = true;
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.6f, Pitch = -0.3f }, NPC.Center);
            }
            if (substate == 2)
            {
                NPC.noGravity = NPC.noTileCollide = true;
                contactDamage = StateTimer >= 30;
                float progress = MathHelper.Clamp((StateTimer + 1f) / 60f, 0f, 1f);
                float height = Math.Min(230f, FloorY - ArenaData.WorldBounds.Top - 170f);
                Vector2 destination = rampStart - new Vector2(0f, MathF.Sin(progress * MathHelper.Pi) * height);
                NPC.velocity = destination - NPC.Center;
                spinTarget = storedDirection * 0.2f;
                if (StateTimer < 59)
                    return;
                NPC.Center = rampStart;
                NPC.velocity = Vector2.Zero;
                impactFlash = 1f;
                KickUpDust(24);
                ScreenShake(13f);
                SpawnAuraPulse(200f, 35, false);
                SpawnProjectile<TumblerFloorRipple>(new Vector2(rampStart.X, FloorY), Vector2.Zero, 0, 0f, NPC.whoAmI);
                SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.95f, Pitch = -0.6f }, NPC.Center);
                FinishAttack();
            }
        }
    }
}
