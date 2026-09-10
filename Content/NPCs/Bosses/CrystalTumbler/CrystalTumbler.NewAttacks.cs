using System;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public partial class CrystalTumbler
    {
        private Vector2 teleportOrigin;
        private int distantTimer;
        internal float LoopRailProgress => substate < 2 ? MathHelper.Clamp(StateTimer / 90f, 0f, 1f) * 0.35f : substate == 2 ? MathHelper.Clamp(TumblerLoopRail.RideProgress(StateTimer) + 0.35f, 0f, 1f) : 1f;
        internal bool LoopRailFinished => State != TumblerState.LoopSlam || substate >= 5;

        private void PressureDistantPlayer()
        {
            if (State is not (TumblerState.Idle or TumblerState.BoltVolley or TumblerState.StarCircuit) || Math.Abs(Target.Center.X - NPC.Center.X) < 650f)
            {
                distantTimer = Math.Max(0, distantTimer - 2);
                return;
            }
            if (++distantTimer < 180)
                return;
            distantTimer = 0;
            Vector2 source = ArenaData.ClosestCrystal(Target.Center);
            SpawnProjectile<TumblerAimLine>(source, (Target.Center - source).SafeNormalize(Vector2.UnitY), ProjectileDamage(15), 0f, PhaseTwo ? 1f : 0f);
        }

        private void LoopSlam()
        {
            float centerX = ArenaData.ArenaCenter.X;
            if (substate == 0)
            {
                MoveHorizontal(TumblerLoopRail.StartX, 9f, 0.16f);
                if (Math.Abs(NPC.Center.X - TumblerLoopRail.StartX) < 6f && Math.Abs(NPC.velocity.X) < 2f && OnGround())
                {
                    NPC.velocity.X *= 0.5f;
                    rampStart = new Vector2(NPC.Center.X, FloorY - NPC.height * 0.5f);
                    SpawnProjectile<TumblerLoopRail>(rampStart, Vector2.Zero, 0, 0f, NPC.whoAmI);
                    substate = 1;
                    StateTimer = 0;
                    NPC.netUpdate = true;
                }
                return;
            }
            if (substate == 1)
            {
                SpinUp(90, 17f);
                if (StateTimer < 90 || !OnGround())
                    return;
                substate = 2;
                StateTimer = 0;
                NPC.netUpdate = true;
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.6f, Pitch = -0.1f }, NPC.Center);
            }
            if (substate == 2)
            {
                NPC.noGravity = NPC.noTileCollide = true;
                contactDamage = true;
                float progress = TumblerLoopRail.RideProgress(StateTimer + 1f);
                NPC.velocity = TumblerLoopRail.Point(rampStart, progress) - NPC.Center;
                spinTarget = NPC.velocity.Length() / 52f;
                visualCharge = 0.9f;
                if (StateTimer >= TumblerLoopRail.RideDuration - 1)
                {
                    substate = 4;
                    StateTimer = 0;
                    NPC.netUpdate = true;
                }
                return;
            }
            if (substate == 4)
            {
                NPC.noGravity = NPC.noTileCollide = true;
                contactDamage = true;
                NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.9f, 21f);
                if (NPC.Bottom.Y + NPC.velocity.Y < FloorY)
                    return;
                NPC.Bottom = new Vector2(NPC.Center.X, FloorY);
                NPC.velocity = Vector2.Zero;
                impactFlash = 1f;
                KickUpDust(18);
                ScreenShake(11f);
                SpawnAuraPulse(160f, 30, false);
                SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.8f, Pitch = -0.4f }, NPC.Center);
                if (!TumblerMagneticPlatform.CollapseAll(NPC))
                    EnsureMagneticPlatforms();
                EnsureConductiveCrystals();
                if (Main.expertMode)
                {
                    for (int ring = 0; ring < 5; ring++)
                    {
                        for (int side = -1; side <= 1; side += 2)
                        {
                            float x = centerX + side * (80f + ring * (RightInner - centerX - 100f) / 4f);
                            Vector2 source = new(x, ArenaData.WorldBounds.Top + 120f);
                            SpawnProjectile<TumblerLightningBolt>(source, new Vector2(0f, FloorY - source.Y), ProjectileDamage(19), 0f, 60f + ring * 16f, 1f);
                        }
                    }
                }
                substate = 5;
                StateTimer = 0;
                NPC.netUpdate = true;
            }
            if (substate == 5 && StateTimer >= (Main.expertMode ? 150 : 65))
                FinishAttack();
        }

        private void GroundRaze()
        {
            GroundRoll(1.8f, 0.1f, 220f);
            visualCharge = MathHelper.Clamp(StateTimer / 120f, 0f, 1f);
            if (StateTimer == 1)
            {
                int side = Target.Center.X < ArenaData.ArenaCenter.X ? -1 : 1;
                SpawnProjectile<TumblerRazeBeam>(new Vector2(ArenaData.ArenaCenter.X, FloorY), Vector2.Zero, ProjectileDamage(21), 0f, NPC.whoAmI, side);
            }
            if (StateTimer >= 430)
                FinishAttack();
        }

        private void CrystalConvergence()
        {
            GroundRoll(0.9f, 0.06f, 180f);
            visualCharge = MathHelper.Clamp(StateTimer / 160f, 0f, 1f);
            if (StateTimer == 1)
                SpawnProjectile<TumblerConvergenceOrb>(TumblerConvergenceOrb.Anchor, Vector2.Zero, ProjectileDamage(22), 0f, NPC.whoAmI);
            if (StateTimer >= 610)
                FinishAttack();
        }

        private void DrawNewAttackEffects(SpriteBatch spriteBatch, Vector2 screenPos, Texture2D texture, Rectangle frame, Vector2 origin)
        {
            if (State == TumblerState.Teleport && StateTimer >= 50 && StateTimer < 108)
            {
                Vector2 start = StateTimer < 70 ? NPC.Center : teleportOrigin;
                float progress = MathHelper.Clamp((StateTimer - 50f) / 36f, 0f, 1f);
                float fade = MathHelper.Clamp((108f - StateTimer) / 28f, 0f, 1f);
                for (int i = 0; i < 12; i++)
                {
                    float position = i / 11f;
                    float brightness = Math.Max(0f, 1f - Math.Abs(position - progress) * 4f) * fade;
                    Vector2 point = Vector2.Lerp(start, teleportDestination, position) - screenPos;
                    spriteBatch.Draw(texture, point, frame, TumblerVFX.Glow(PhaseColor, brightness * 0.45f), NPC.rotation - position * 2f, origin, NPC.scale * (0.8f + brightness * 0.2f), SpriteEffects.None, 0f);
                }
                TumblerVFX.DrawElectricLine(spriteBatch, start - screenPos, teleportDestination - screenPos, PhaseColor, fade * 0.32f, 32, NPC.whoAmI);
            }
            if (State == TumblerState.LoopSlam && substate is 2 or 4)
            {
                Vector2 tip = TumblerLoopRail.Point(rampStart, 1f);
                Vector2 ground = new(tip.X, FloorY);
                float strength = substate == 4 ? 0.8f : 0.35f;
                TumblerVFX.DrawTelegraph(spriteBatch, ground - screenPos, tip - screenPos, PhaseColor, strength, 60f);
                TumblerVFX.DrawCharge(spriteBatch, ground - screenPos, PhaseColor, substate == 4 ? 1f : StateTimer / (float)TumblerLoopRail.RideDuration, 45f, StateTimer * 0.035f);
            }
            if (State == TumblerState.PhaseTransition)
            {
                Vector2 position = NPC.Top - screenPos - new Vector2(0f, 40f);
                Utils.DrawBorderString(spriteBatch, shieldHits + "!", position, Color.Lerp(PhaseColor, Color.White, shieldFlash), 1.2f + shieldFlash * 0.22f, 0.5f, 0.5f);
            }
        }
    }
}
