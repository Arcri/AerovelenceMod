using System;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public partial class CrystalTumbler
    {
        internal const int EntranceDuration = 600;
        internal Vector2 EntranceStart => new(ArenaData.ArenaCenter.X, FloorY - NPC.height * 0.5f - 850f);
        internal static Vector2 GatewayFocus => new((ArenaData.TileBounds.X + ArenaData.GatewayOffsetX + 7.5f) * 16f, (ArenaData.TileBounds.Y + ArenaData.GatewayOffsetY + 7.5f) * 16f);

        private void Entrance()
        {
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            contactDamage = false;
            Vector2 landing = new(ArenaData.ArenaCenter.X, FloorY - NPC.height * 0.5f);
            if (StateTimer < 455)
            {
                NPC.Center = EntranceStart;
                NPC.velocity = Vector2.Zero;
            }
            else if (StateTimer < 525)
            {
                float progress = MathHelper.Clamp((StateTimer - 454f) / 70f, 0f, 1f);
                Vector2 next = Vector2.Lerp(EntranceStart, landing, progress * progress);
                NPC.velocity = next - NPC.Center;
                spinTarget = MathHelper.Lerp(0.015f, 0.12f, progress);
                visualCharge = 0.25f + progress * 0.3f;
            }
            else
            {
                float bounce = StateTimer < 551 ? MathF.Sin((StateTimer - 525f) / 26f * MathHelper.Pi) * 19f : 0f;
                NPC.velocity = landing - new Vector2(0f, bounce) - NPC.Center;
                spinTarget = 0f;
                visualCharge = MathHelper.Clamp((StateTimer - 535f) / 35f, 0f, 1f);
            }

            if (StateTimer == 330)
                SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.4f, Pitch = -0.4f }, GatewayFocus);
            if (StateTimer == 392 || StateTimer == 415)
                SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/RockCollideBetter") with { Volume = 0.65f, Pitch = -0.5f }, GatewayFocus - new Vector2(0f, 260f));
            if (StateTimer >= 390 && StateTimer < 445 && !Main.dedServ)
            {
                if (ArenaData.WorldBounds.Contains(Main.LocalPlayer.Center.ToPoint()))
                    ScreenShake(2f + (StateTimer - 390f) / 18f);
                if (StateTimer % 3 == 0)
                {
                    Vector2 source = new(GatewayFocus.X + Main.rand.NextFloat(-130f, 130f), Main.screenPosition.Y - 20f);
                    Dust stone = Dust.NewDustPerfect(source, DustID.Stone, new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(3f, 7f)), 0, new Color(150, 155, 165), Main.rand.NextFloat(1.2f, 1.9f));
                    stone.noGravity = false;
                    Dust.NewDustPerfect(source + new Vector2(Main.rand.NextFloat(-25f, 25f), 10f), ModContent.DustType<TumblerRollDust>(), new Vector2(Main.rand.NextFloat(-1f, 1f), 3.5f), 45, new Color(110, 114, 125), Main.rand.NextFloat(0.16f, 0.24f));
                }
            }
            if (StateTimer >= 455 && StateTimer < 525 && StateTimer % 4 == 0 && !Main.dedServ)
            {
                Dust.NewDustPerfect(NPC.Top + Main.rand.NextVector2Circular(32f, 10f), ModContent.DustType<TumblerRollDust>(), new Vector2(Main.rand.NextFloat(-1f, 1f), -1.5f), 45, new Color(100, 108, 122), 0.18f);
            }
            if (StateTimer == 525)
            {
                impactFlash = 1f;
                KickUpDust(22);
                ScreenShake(12f);
                SpawnAuraPulse(230f, 38, false);
                SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/HardRockSlam") with { Volume = 0.95f, Pitch = -0.25f }, landing);
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 24; i++)
                        Dust.NewDustPerfect(NPC.Bottom, DustID.Stone, new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, -2f)), 0, new Color(150, 160, 175), Main.rand.NextFloat(1.1f, 1.8f));
                }
                NPC.netUpdate = true;
            }
            if (StateTimer == 555)
            {
                shieldFlash = 1f;
                SpawnAuraPulse(145f, 36, false);
                SoundEngine.PlaySound(SoundID.Roar with { Volume = 0.85f, Pitch = -0.3f }, NPC.Center);
                SoundEngine.PlaySound(new SoundStyle("AerovelenceMod/Sounds/Effects/GiantElectricityShot") with { Volume = 0.3f, Pitch = -0.2f }, NPC.Center);
                if (!Main.dedServ)
                {
                    for (int i = 0; i < 18; i++)
                    {
                        Vector2 direction = (i * MathHelper.TwoPi / 18f).ToRotationVector2();
                        TumblerVFX.SpawnSpark(NPC.Center + direction * 59f, direction * 2.8f, PhaseColor, 0.25f);
                    }
                }
            }
            if (StateTimer >= EntranceDuration)
            {
                NPC.dontTakeDamage = false;
                NPC.noGravity = false;
                NPC.noTileCollide = false;
                NPC.velocity = Vector2.Zero;
                afterimagePositions.Clear();
                afterimageRotations.Clear();
                ChangeState(TumblerState.Idle);
            }
        }

        private void DrawEntrance(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (State != TumblerState.Spawn)
                return;
            if (StateTimer >= 455 && StateTimer < 525)
            {
                Vector2 ground = new(ArenaData.ArenaCenter.X, FloorY);
                float progress = (StateTimer - 455f) / 70f;
                TumblerVFX.DrawCharge(spriteBatch, ground - screenPos, PhaseColor, progress, 75f - progress * 20f, StateTimer * 0.025f);
            }
            if (StateTimer >= 540)
            {
                float charge = MathHelper.Clamp((StateTimer - 540f) / 25f, 0f, 1f);
                TumblerVFX.DrawCorona(spriteBatch, NPC.Center - screenPos, 72f, Color.Lerp(PhaseColor, Color.White, shieldFlash), charge * 0.8f, NPC.whoAmI, 2f);
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => State == TumblerState.Spawn ? false : null;
        public override bool CheckActive() => State == TumblerState.Despawn;
        public override void BossHeadSlot(ref int index)
        {
            if (State == TumblerState.Spawn && StateTimer < 430)
                index = -1;
        }
    }
}
