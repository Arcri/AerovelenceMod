using System;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public partial class CrystalTumbler
    {
        private bool TargetArenaPlayer()
        {
            if (!ArenaData.Valid)
                return false;
            int target = -1;
            float nearest = float.MaxValue;
            foreach (Player player in Main.ActivePlayers)
            {
                if (player.dead || !ArenaData.WorldBounds.Contains(player.Center.ToPoint()) || player.Center.X <= LeftOuter || player.Center.X >= RightOuter || player.Top.Y >= FloorY + 32f)
                    continue;
                float distance = Vector2.DistanceSquared(player.Center, NPC.Center);
                if (distance < nearest)
                {
                    target = player.whoAmI;
                    nearest = distance;
                }
            }
            if (target < 0)
                return false;
            NPC.target = target;
            return true;
        }

        public override bool CheckDead()
        {
            if (State == TumblerState.Death && StateTimer >= 180)
                return true;
            NPC.life = 1;
            NPC.dontTakeDamage = true;
            if (State != TumblerState.Death && IsServer)
            {
                phaseTransitionQueued = false;
                ChangeState(TumblerState.Death);
                ArenaData.ClearEncounterEntities(fadeProjectiles: true);
            }
            return false;
        }

        private void DeathAnimation()
        {
            NPC.dontTakeDamage = true;
            NPC.noGravity = NPC.noTileCollide = true;
            NPC.velocity *= 0.72f;
            spinTarget = 0f;
            NPC.timeLeft = 180;
            float charge = MathHelper.Clamp(StateTimer / 150f, 0f, 1f);
            visualCharge = charge;
            shieldFlash = Math.Max(shieldFlash, charge * 0.5f);
            if (StateTimer == 0)
            {
                SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.85f, Pitch = -0.4f }, NPC.Center);
                ScreenShake(7f);
            }
            int interval = Math.Max(5, 24 - StateTimer / 8);
            if (StateTimer < 150 && StateTimer % interval == 0)
            {
                SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.1f + charge * 0.12f, Pitch = charge * 0.6f, MaxInstances = 1 }, NPC.Center);
                ScreenShake(2f + charge * 5f);
                if (!Main.dedServ)
                    for (int i = 0; i < 7; i++)
                        TumblerVFX.SpawnSpark(NPC.Center + Main.rand.NextVector2Circular(42f, 42f), Main.rand.NextVector2Circular(3f, 3f), Color.Lerp(PhaseColor, Color.White, charge), 0.25f + charge * 0.15f);
            }
            if (StateTimer == 150)
            {
                impactFlash = 1f;
                KickUpDust(36);
                ThrowImpactRubble(20, NPC.Center);
                ScreenShake(15f);
                if (!Main.dedServ)
                {
                    FlashSystem.SetFlashEffect(1f, 18);
                    for (int i = 0; i < 64; i++)
                    {
                        Vector2 direction = (i * MathHelper.TwoPi / 64f).ToRotationVector2();
                        TumblerVFX.SpawnSpark(NPC.Center + direction * 42f, direction * Main.rand.NextFloat(3f, 9f), Color.Lerp(PhaseColor, Color.White, 0.65f), 0.35f);
                        if (i % 2 == 0)
                            Dust.NewDustPerfect(NPC.Center, DustID.Stone, direction * Main.rand.NextFloat(4f, 11f), 0, default, 1.5f);
                    }
                }
                SpawnAuraPulse(300f, 40, false);
                SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = 0.3f, Pitch = -0.3f }, NPC.Center);
                SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.6f, Pitch = -0.4f }, NPC.Center);
            }
            if (StateTimer >= 180 && IsServer)
                NPC.StrikeInstantKill();
        }

        private void DrawDeathElectricity(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (State != TumblerState.Death)
                return;
            float charge = MathHelper.Clamp(StateTimer / 150f, 0f, 1f);
            float opacity = MathHelper.Clamp((180f - StateTimer) / 30f, 0f, 1f);
            Vector2 center = NPC.Center - screenPos;
            Color color = Color.Lerp(PhaseColor, Color.White, charge * 0.85f);
            for (int i = 0; i < 8; i++)
            {
                float angle = i * MathHelper.PiOver4 + MathF.Sin(StateTimer / 3 + i * 19f) * 0.35f;
                Vector2 start = center + angle.ToRotationVector2() * (14f + charge * 12f);
                Vector2 end = center + (angle + 2.3f).ToRotationVector2() * (48f + charge * 18f);
                TumblerVFX.DrawElectricLine(spriteBatch, start, end, color, opacity * (0.3f + charge * 0.6f), 14, i * 37 + StateTimer / 3, 1.5f + charge * 2f);
            }
            TumblerVFX.DrawCorona(spriteBatch, center, 60f + charge * 18f, color, charge * opacity, NPC.whoAmI, 2f);
        }
    }
}
