using System;
using System.IO;
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
    public class TumblerCrystalBud : ModNPC
    {
        private int timer;
        private int hitCount;
        private float growth;
        private float flash;
        private float baseY;

        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/GroundSpike";

        public override void SetStaticDefaults()
        {
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 72;
            NPC.damage = 24;
            NPC.defense = 0;
            NPC.lifeMax = 9999;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.dontCountMe = true;
            NPC.HitSound = SoundID.Tink;
        }

        public override void AI()
        {
            timer++;
            flash *= 0.78f;
            growth = timer < 52
                ? MathHelper.SmoothStep(0.12f, 0.32f, MathHelper.Clamp(timer / 52f, 0f, 1f))
                : MathHelper.SmoothStep(0.32f, 1f, MathHelper.Clamp((timer - 52f) / 20f, 0f, 1f));
            growth *= MathHelper.Clamp((600f - timer) / 30f, 0f, 1f);
            if (baseY == 0f)
                baseY = NPC.Bottom.Y;
            NPC.height = Math.Max(8, (int)(72f * growth));
            NPC.Bottom = new Vector2(NPC.Center.X, baseY);
            NPC.velocity = Vector2.Zero;
            if (timer >= 600 || !NPC.AnyNPCs(ModContent.NPCType<CrystalTumbler>()))
            {
                Deactivate();
                return;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && timer >= 72 && timer < 570)
            {
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && !player.dead && player.Hitbox.Intersects(NPC.Hitbox))
                    {
                        Break(true);
                        return;
                    }
                }
            }
        }

        public override bool CheckActive() => false;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return timer >= 72 && timer < 570;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Electrified, 60);
            Break(true);
        }

        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetMaxDamage(1);
            modifiers.HideCombatText();
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            modifiers.SetMaxDamage(1);
            modifiers.HideCombatText();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            RegisterHit(hit.Damage);
        }

        public override void UpdateLifeRegen(ref int damage)
        {
            NPC.lifeRegen = 0;
            damage = 0;
        }

        private void RegisterHit(int damageDone)
        {
            if (damageDone <= 0)
                return;
            NPC.life = NPC.lifeMax;
            hitCount++;
            flash = 1f;
            if (hitCount >= 2)
                Break(false);
            else
                NPC.netUpdate = true;
        }

        private void Break(bool contact)
        {
            if (!NPC.active)
                return;
            SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.55f, PitchVariance = 0.2f }, NPC.Center);
            if (Main.netMode != NetmodeID.MultiplayerClient)
                Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TumblerAuraPulse>(), contact ? 15 : 0, 0f, Main.myPlayer, 58f, 24f, NPC.ai[0]);
            Deactivate();
        }

        private void Deactivate()
        {
            NPC.active = false;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hitCount);
            writer.Write(timer);
            writer.Write(baseY > 0f ? baseY : NPC.Bottom.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hitCount = reader.ReadInt32();
            timer = reader.ReadInt32();
            baseY = reader.ReadSingle();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Color phaseColor = TumblerVFX.PhaseColor(NPC.ai[0]);
            float opacity = MathHelper.Clamp((600f - timer) / 30f, 0f, 1f);
            Color color = Color.Lerp(drawColor, Color.White, flash) * opacity;
            Vector2 origin = new(texture.Width * 0.5f, texture.Height);
            Vector2 scale = new(NPC.width / (float)texture.Width, NPC.height / (float)texture.Height);
            Main.EntitySpriteDraw(texture, NPC.Bottom - screenPos, null, color, 0f, origin, scale, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, NPC.Bottom - screenPos, null, TumblerVFX.Glow(phaseColor, opacity * 0.3f), 0f, origin, scale * 1.06f, SpriteEffects.None);
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            Vector2 tip = NPC.Top - screenPos + new Vector2(0f, 3f);
            if (timer < 72)
            {
                float charge = timer / 72f;
                TumblerVFX.DrawCharge(spriteBatch, NPC.Bottom - screenPos - new Vector2(0f, 5f), phaseColor, charge, 12f, timer * 0.04f);
                TumblerVFX.DrawLine(spriteBatch, NPC.Bottom - screenPos, NPC.Bottom - screenPos - new Vector2(0f, 72f), TumblerVFX.Glow(phaseColor, 0.15f + charge * 0.15f), 1f);
            }
            else
            {
                float pulse = 0.5f + MathF.Sin(timer * 0.09f) * 0.12f;
                Main.EntitySpriteDraw(star, tip, null, TumblerVFX.Glow(phaseColor, pulse * opacity), 0f, star.Size() * 0.5f, new Vector2(19f, 11f) / star.Size(), SpriteEffects.None);
                TumblerVFX.DrawLine(spriteBatch, tip, NPC.Bottom - screenPos + new Vector2(-4f, -9f), TumblerVFX.Glow(phaseColor, opacity * 0.55f), 1.2f);
            }
            return false;
        }
    }

    public class TumblerConductiveCrystal : ModNPC
    {
        private float glow;
        private float charge;
        private float phase;
        private float connection;
        private int bossIndex = -1;

        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/GroundSpike";

        public override void SetDefaults()
        {
            NPC.width = 42;
            NPC.height = 96;
            NPC.damage = 0;
            NPC.defense = 9999;
            NPC.lifeMax = 9999;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontCountMe = true;
            NPC.dontTakeDamage = true;
        }

        public override void AI()
        {
            glow = 0.5f + MathF.Sin(Main.GlobalTimeWrappedHourly * 5f + NPC.whoAmI) * 0.16f;
            bossIndex = NPC.FindFirstNPC(ModContent.NPCType<CrystalTumbler>());
            if (bossIndex < 0)
            {
                NPC.active = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                return;
            }
            NPC boss = Main.npc[bossIndex];
            phase = boss.ai[2];
            float targetCharge = boss.ai[0] == (float)TumblerState.ConductiveField ? MathHelper.Clamp(boss.ai[1] / 120f, 0f, 1f) : 0f;
            charge = MathHelper.Lerp(charge, targetCharge, 0.08f);
            connection = MathHelper.Lerp(connection, NPC.ai[1] > 0f ? 1f : 0f, 0.05f);
            NPC.velocity.X *= 0.8f;
            NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.35f, 12f);
            if (ArenaData.Valid && NPC.Bottom.Y + NPC.velocity.Y >= ArenaData.FloorY)
            {
                NPC.velocity.Y = ArenaData.FloorY - NPC.Bottom.Y;
                if (NPC.ai[1] == 0f)
                {
                    NPC.ai[1] = 1f;
                    NPC.netUpdate = true;
                    SoundEngine.PlaySound(SoundID.Item70 with { Volume = 0.35f, Pitch = -0.15f }, NPC.Bottom);
                }
            }
            Lighting.AddLight(NPC.Center, TumblerVFX.PhaseColor(phase).ToVector3() * (glow * 0.45f + charge * 0.5f));
        }

        public override bool CheckActive() => false;

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            Texture2D bloom = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64").Value;
            Vector2 origin = new(texture.Width * 0.5f, texture.Height);
            SpriteEffects effects = NPC.ai[0] < 0f ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 root = NPC.Bottom - screenPos;
            Vector2 center = NPC.Center - screenPos;
            Vector2 scale = new(32f / texture.Width, 96f / texture.Height);
            Color color = TumblerVFX.PhaseColor(phase);
            if (connection > 0.02f && ArenaData.CrystalPositions.Length > 0)
            {
                Vector2 source = ArenaData.ClosestCrystal(NPC.Top) - screenPos;
                Vector2 conductorTip = root - new Vector2(0f, 94f);
                Vector2 bend = Vector2.Lerp(source, conductorTip, 0.55f) + new Vector2(NPC.ai[0] * 34f, -14f);
                float filamentStrength = connection * (0.2f + charge * 0.18f);
                TumblerVFX.DrawElectricLine(spriteBatch, source, bend, color, filamentStrength, 12, NPC.whoAmI + 12f, 0.65f);
                TumblerVFX.DrawElectricLine(spriteBatch, bend, conductorTip, color, filamentStrength, 10, NPC.whoAmI + 13f, 0.65f);
                for (int i = 0; i < 3; i++)
                {
                    float progress = (Main.GlobalTimeWrappedHourly * (0.32f + charge * 0.3f) + i / 3f) % 1f;
                    Vector2 spark = progress < 0.55f ? Vector2.Lerp(source, bend, progress / 0.55f) : Vector2.Lerp(bend, conductorTip, (progress - 0.55f) / 0.45f);
                    Main.EntitySpriteDraw(star, spark, null, TumblerVFX.Glow(color, connection * 0.5f), 0f, star.Size() * 0.5f, new Vector2(9f, 5f) / star.Size(), SpriteEffects.None);
                }
                TumblerVFX.DrawCharge(spriteBatch, source, color, 0.18f + charge * 0.5f, 13f, Main.GlobalTimeWrappedHourly);
                if (charge > 0.05f && bossIndex >= 0 && Main.npc[bossIndex].active)
                {
                    NPC boss = Main.npc[bossIndex];
                    if (boss.ai[0] == (float)TumblerState.ConductiveField && boss.ai[1] < 120f)
                        TumblerVFX.DrawElectricLine(spriteBatch, conductorTip, boss.Center - screenPos, color, charge * 0.4f, 18, NPC.whoAmI + 21f, 1f);
                }
            }
            Color stone = Color.Lerp(drawColor, new Color(135, 167, 195), 0.35f);
            Main.EntitySpriteDraw(texture, root + new Vector2(-15f, -2f), null, stone * 0.8f, -0.16f, origin, new Vector2(17f, 59f) / texture.Size(), effects);
            Main.EntitySpriteDraw(texture, root + new Vector2(16f, 0f), null, stone * 0.7f, 0.2f, origin, new Vector2(16f, 49f) / texture.Size(), effects);
            Main.EntitySpriteDraw(texture, root, null, stone, 0f, origin, scale, effects);
            Main.EntitySpriteDraw(texture, root, null, TumblerVFX.Glow(color, 0.16f + charge * 0.22f), 0f, origin, scale, effects);

            Vector2 tip = root - new Vector2(0f, 94f);
            Vector2 left = root + new Vector2(-13f, -51f);
            Vector2 right = root + new Vector2(11f, -33f);
            Vector2 bottom = root - new Vector2(1f, 9f);
            Color seam = TumblerVFX.Glow(Color.Lerp(color, Color.White, 0.25f), 0.4f + charge * 0.5f);
            TumblerVFX.DrawLine(spriteBatch, tip, left, seam, 1.2f);
            TumblerVFX.DrawLine(spriteBatch, tip, right, seam * 0.8f, 1f);
            TumblerVFX.DrawLine(spriteBatch, left, bottom, seam * 0.7f, 1.2f);
            TumblerVFX.DrawLine(spriteBatch, right, bottom, seam, 1.2f);
            TumblerVFX.DrawLine(spriteBatch, left, right, seam * 0.8f, 1f);
            TumblerVFX.DrawLine(spriteBatch, tip, bottom, seam * 0.55f, 1f);
            Main.EntitySpriteDraw(bloom, center, null, TumblerVFX.Glow(color, glow * 0.12f + charge * 0.2f), 0f, bloom.Size() * 0.5f, new Vector2(50f, 86f) / bloom.Size(), SpriteEffects.None);
            Main.EntitySpriteDraw(star, center, null, TumblerVFX.Glow(Color.Lerp(color, Color.White, 0.4f), 0.4f + charge * 0.5f), 0f, star.Size() * 0.5f, new Vector2(30f + charge * 16f, 17f) / star.Size(), SpriteEffects.None);
            Main.EntitySpriteDraw(star, tip, null, TumblerVFX.Glow(color, 0.4f + glow * 0.4f), 0f, star.Size() * 0.5f, 18f / star.Width, SpriteEffects.None);
            if (charge > 0.05f)
                TumblerVFX.DrawCorona(spriteBatch, center, 18f + charge * 7f, color, charge * 0.5f, NPC.whoAmI, 1.2f);
            return false;
        }
    }
}
