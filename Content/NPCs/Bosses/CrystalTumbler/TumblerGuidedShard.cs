using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerGuidedShard : ModProjectile
    {
        private int timer;
        private Vector2 launchVelocity;
        private Vector2 lockPosition;
        private float phase;
        private int Age => timer - Math.Max(0, (int)Projectile.ai[1]);
        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalShard";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 20;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.timeLeft = 340;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }
        public override bool ShouldUpdatePosition() => Age >= 96;
        public override bool? CanDamage() => Age >= 96;
        public override void AI()
        {
            int index = (int)Projectile.ai[0];
            if (index < 0 || index >= Main.maxNPCs || !Main.npc[index].active || Main.npc[index].ModNPC is not CrystalTumbler)
            {
                TumblerProjectileRetirement.Begin(Projectile);
                return;
            }
            NPC boss = Main.npc[index];
            if (timer == 0 && Projectile.ai[1] < 0f)
                timer = 49;
            timer++;
            phase = boss.ai[2];
            if (Age < 50)
            {
                float progress = MathHelper.Clamp(Age / 50f, 0f, 1f);
                float angle = -MathHelper.PiOver2 + Projectile.ai[1] * 0.023f + (1f - progress) * 3f;
                Projectile.Center = boss.Center + angle.ToRotationVector2() * MathHelper.Lerp(34f, 115f, progress);
                Projectile.rotation = angle + MathHelper.PiOver2;
                Projectile.scale = MathHelper.SmoothStep(0.1f, 1.25f, progress);
            }
            if (Age == 50)
            {
                lockPosition = Projectile.Center;
                Projectile.scale = 1.25f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Player player = Main.player[Player.FindClosest(Projectile.Center, 1, 1)];
                    Vector2 displacement = player.Center + player.velocity * 8f - Projectile.Center;
                    float flightTime = MathHelper.Clamp(displacement.Length() / 8.5f, 22f, 65f);
                    launchVelocity = displacement / flightTime - new Vector2(0f, 0.11f * (flightTime + 1f));
                    Projectile.netUpdate = true;
                }
            }
            if (Age >= 50 && Age < 96)
            {
                Projectile.Center = lockPosition;
                Projectile.rotation = Utils.AngleLerp(Projectile.rotation, launchVelocity.ToRotation() + MathHelper.PiOver2, 0.16f);
            }
            if (Age == 96)
            {
                Projectile.velocity = launchVelocity;
                Projectile.hostile = true;
                Projectile.tileCollide = true;
                SoundEngine.PlaySound(SoundID.Item17 with { Volume = 0.4f, Pitch = 0.25f }, Projectile.Center);
            }
            if (Age >= 96)
            {
                Projectile.velocity.Y += 0.22f;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.WriteVector2(launchVelocity);
            writer.WriteVector2(lockPosition);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            launchVelocity = reader.ReadVector2();
            lockPosition = reader.ReadVector2();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Age <= 0)
                return false;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Color color = TumblerVFX.PhaseColor(phase);
            Main.EntitySpriteDraw(texture, position, null, Color.Lerp(lightColor, Color.White, 0.6f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, position, null, TumblerVFX.Glow(color, 0.45f), Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None);
            if (Age >= 50 && Age < 96)
            {
                Vector2 previous = position;
                for (int step = 1; step <= 12; step++)
                {
                    float ticks = step * 3f;
                    Vector2 next = position + launchVelocity * ticks + new Vector2(0f, 0.11f * ticks * (ticks + 1f));
                    TumblerVFX.DrawLine(Main.spriteBatch, previous, next, TumblerVFX.Glow(color, 0.8f * (1f - step / 14f)), 1.6f);
                    previous = next;
                }
                TumblerVFX.DrawCharge(Main.spriteBatch, position, color, (Age - 50f) / 46f, 12f, timer * 0.06f);
            }
            return false;
        }
    }

    public class TumblerCarapaceShard : ModNPC
    {
        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/GroundSpike";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers { Hide = true });
        }
        public override void SetDefaults()
        {
            NPC.width = 24;
            NPC.height = 34;
            NPC.lifeMax = 1000;
            NPC.damage = 0;
            NPC.dontTakeDamage = true;
            NPC.knockBackResist = 0f;
            NPC.noGravity = NPC.noTileCollide = true;
            NPC.dontCountMe = true;
            NPC.aiStyle = -1;
        }
        public override bool CheckActive() => false;
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;
        public override void AI()
        {
            int index = (int)NPC.ai[0];
            if (index < 0 || index >= Main.maxNPCs || !Main.npc[index].active || Main.npc[index].ModNPC is not CrystalTumbler || NPC.ai[3] >= 360f)
            {
                NPC.active = false;
                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
                return;
            }
            NPC.ai[3]++;
            NPC.dontTakeDamage = NPC.ai[3] < 60f;
            NPC.scale = MathHelper.SmoothStep(0.15f, 1f, MathHelper.Clamp(NPC.ai[3] / 60f, 0f, 1f));
            NPC boss = Main.npc[index];
            float angle = boss.rotation + NPC.ai[1];
            NPC.Center = boss.Center + angle.ToRotationVector2() * (52f + NPC.scale * 16f);
            NPC.rotation = angle + MathHelper.PiOver2;
            NPC.velocity = Vector2.Zero;
        }
        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers) => modifiers.SetMaxDamage(1);
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.dontTakeDamage || hit.Damage <= 0 || !NPC.active || Main.netMode == NetmodeID.MultiplayerClient)
                return;
            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<TumblerGuidedShard>(), (int)NPC.ai[2], 0f, Main.myPlayer, NPC.ai[0], -1f);
            SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.55f }, NPC.Center);
            NPC.active = false;
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 scale = new Vector2(24f, 48f) / texture.Size() * NPC.scale;
            spriteBatch.Draw(texture, NPC.Center - screenPos, null, drawColor, NPC.rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            if (!NPC.dontTakeDamage)
            {
                float pulse = 0.65f + MathF.Sin(NPC.ai[3] * 0.18f) * 0.25f;
                spriteBatch.Draw(texture, NPC.Center - screenPos, null, TumblerVFX.Glow(Color.White, pulse), NPC.rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            }
            return false;
        }
    }
}
