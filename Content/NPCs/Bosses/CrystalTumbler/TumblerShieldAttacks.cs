using System;
using System.IO;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerShieldStorm : ModProjectile
    {
        private int timer;
        private readonly Vector2[] endpoints = new Vector2[3];
        private readonly bool[] locked = new bool[3];
        private readonly TumblerLightningVisual[] bolts = [new(), new(), new()];
        private int Cycle => timer % 480;
        private static int FireTime(int index) => 360 + index * 45;
        private Vector2 Crystal(int index) => ArenaData.CrystalPositions.Length > index ? ArenaData.CrystalPositions[index] : ArenaData.ArenaCenter - new Vector2((1 - index) * 350f, 360f);
        private NPC Owner
        {
            get
            {
                int index = (int)Projectile.ai[0];
                if (index < 0 || index >= Main.maxNPCs)
                    return null;
                NPC npc = Main.npc[index];
                return npc.active && npc.ModNPC is CrystalTumbler boss && boss.OvershieldActive ? npc : null;
            }
        }

        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Owner != null;

        public override void AI()
        {
            NPC owner = Owner;
            if (owner == null)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Projectile.Center = owner.Center;
            timer++;
            if (Cycle == 0)
                Array.Clear(locked);
            for (int i = 0; i < 3; i++)
            {
                int fire = FireTime(i);
                if (Cycle == fire - 60 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Player player = Main.player[Player.FindClosest(Crystal(i), 1, 1)];
                    Vector2 aim = player.Center + player.velocity * 10f;
                    endpoints[i] = aim + (aim - Crystal(i)).SafeNormalize(Vector2.UnitY) * 180f;
                    locked[i] = true;
                    Projectile.netUpdate = true;
                }
                if (locked[i] && Cycle >= fire && Cycle < fire + 20)
                    bolts[i].Update(Projectile, Crystal(i), endpoints[i], 2f, true);
                if (Cycle == fire)
                {
                    SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.8f, Pitch = -0.25f }, Crystal(i));
                    if (!Main.dedServ)
                        for (int spark = 0; spark < 14; spark++)
                            TumblerVFX.SpawnSpark(Crystal(i), Main.rand.NextVector2Circular(4f, 4f), Color.White, 0.3f);
                }
                if (!Main.dedServ && Cycle < fire && timer % 6 == 0)
                {
                    float charge = MathHelper.Clamp(Cycle / (float)fire, 0f, 1f);
                    Vector2 offset = Main.rand.NextVector2CircularEdge(14f + 24f * (1f - charge), 14f + 24f * (1f - charge));
                    TumblerVFX.SpawnSpark(Crystal(i) + offset, -offset.SafeNormalize(Vector2.UnitY) * (1f + charge * 2f), Color.Lerp(TumblerVFX.PhaseColor(1f), Color.White, charge), 0.13f + charge * 0.16f);
                    Lighting.AddLight(Crystal(i), new Vector3(1f, 0.6f, 0.15f) * charge);
                }
            }
            if (Cycle >= 240 && Cycle < 360 && Cycle % 20 == 0)
                SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.18f, Pitch = (Cycle - 240f) / 180f, MaxInstances = 1 }, owner.Center);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int i = 0; i < 3; i++)
            {
                if (!locked[i] || Cycle < FireTime(i) || Cycle >= FireTime(i) + 16)
                    continue;
                float collision = 0f;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Crystal(i), endpoints[i], 22f, ref collision))
                    return true;
            }
            return false;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 60);
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            for (int i = 0; i < 3; i++)
            {
                writer.Write(locked[i]);
                writer.WriteVector2(endpoints[i]);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            for (int i = 0; i < 3; i++)
            {
                locked[i] = reader.ReadBoolean();
                endpoints[i] = reader.ReadVector2();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            NPC owner = Owner;
            if (owner == null)
                return false;
            Color color = TumblerVFX.PhaseColor(1f);
            for (int i = 0; i < 3; i++)
            {
                int fire = FireTime(i);
                float charge = Cycle >= fire + 20 ? 0f : MathHelper.Clamp(Cycle / (float)fire, 0f, 1f);
                Vector2 tip = Crystal(i) - Main.screenPosition;
                float urgency = charge * charge;
                TumblerVFX.DrawCharge(Main.spriteBatch, tip, Color.Lerp(color, Color.White, urgency * 0.75f), charge, 14f + charge * 32f, timer * (0.012f + urgency * 0.045f), charge);
                if (Cycle < fire)
                {
                    TumblerVFX.DrawElectricLine(Main.spriteBatch, owner.Center - Main.screenPosition, tip, color, charge * 0.5f, 28, i + timer / 8, 1f + charge);
                    TumblerVFX.DrawCorona(Main.spriteBatch, tip, 52f - charge * 30f, Color.Lerp(color, Color.White, urgency), charge * (0.55f + 0.2f * MathF.Sin(timer * (0.04f + urgency * 0.2f))), i);
                }
                if (!locked[i])
                    continue;
                if (Cycle >= fire - 60 && Cycle < fire)
                    TumblerVFX.DrawTelegraph(Main.spriteBatch, tip, endpoints[i] - Main.screenPosition, Color.Lerp(color, Color.White, (Cycle - fire + 60f) / 60f), 0.4f + (Cycle - fire + 60f) / 100f, 24f);
                if (Cycle >= fire && Cycle < fire + 20)
                    bolts[i].Draw(Main.spriteBatch, Color.Lerp(color, Color.White, MathHelper.Clamp((fire + 7f - Cycle) / 7f, 0f, 1f)), (fire + 20f - Cycle) / 20f, 3f);
            }
            return false;
        }
    }

    public class TumblerPulseShield : ModProjectile
    {
        private int timer;
        private int retirement;
        private float phase;
        private readonly TumblerLightningVisual[] zaps = new TumblerLightningVisual[12];
        private readonly Vector2[] zapEnds = new Vector2[12];
        private readonly int[] zapAges = new int[12];
        private NPC Owner => (int)Projectile.ai[0] >= 0 && (int)Projectile.ai[0] < Main.maxNPCs ? Main.npc[(int)Projectile.ai[0]] : null;
        private bool OwnerActive => Owner is { active: true } && Owner.ModNPC is CrystalTumbler && Owner.ai[0] == (float)TumblerState.ElectricPulse;
        private float Growth => MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(timer / 180f, 0f, 1f));
        private float Radius => 58f + Growth * (110f + 14f * MathF.Sin((timer - 60f) * MathHelper.TwoPi / 90f));
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1200;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 260;
            Projectile.netImportant = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => retirement == 0 && OwnerActive && timer >= 60 && timer < 235;
        internal void Retire()
        {
            if (retirement > 0)
                return;
            retirement = 36;
            Projectile.hostile = false;
            Projectile.timeLeft = 40;
            Projectile.netUpdate = true;
        }
        public override void AI()
        {
            if (!OwnerActive)
                Retire();
            if (retirement > 0)
            {
                if (--retirement == 0)
                    Projectile.Kill();
                return;
            }
            phase = Owner.ai[2];
            timer++;
            Projectile.Center = Owner.Center;
            if (!Main.dedServ)
            {
                for (int i = 0; i < zaps.Length; i++)
                {
                    zapAges[i]++;
                    if ((timer + i * 3) % 24 == 0 && timer >= 35 && timer < 235)
                    {
                        zaps[i] = new TumblerLightningVisual();
                        float angle = i * MathHelper.TwoPi / zaps.Length + timer * 0.023f + Main.rand.NextFloat(-0.2f, 0.2f);
                        zapEnds[i] = angle.ToRotationVector2();
                        zapAges[i] = 0;
                        Vector2 tip = Projectile.Center + zapEnds[i] * Radius;
                        TumblerVFX.SpawnSpark(tip, zapEnds[i] * Main.rand.NextFloat(2f, 3.5f), Color.White, 0.22f);
                    }
                    if (zaps[i] != null && zapAges[i] < 12)
                        zaps[i].Update(Projectile, Projectile.Center, Projectile.Center + zapEnds[i] * Radius, 1.1f, true);
                }
            }
            if (timer == 60 || timer == 150)
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.5f, Pitch = 0.15f }, Projectile.Center);
            if (!Main.dedServ && timer % 4 == 0)
            {
                Vector2 direction = Main.rand.NextVector2Unit();
                TumblerVFX.SpawnSpark(Projectile.Center + direction * Radius, direction * (timer < 60 ? -1.5f : 1.4f), TumblerVFX.PhaseColor(Owner.ai[2]), 0.22f);
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => Vector2.DistanceSquared(Projectile.Center, targetHitbox.ClosestPointInRect(Projectile.Center)) <= Radius * Radius;
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(BuffID.Electrified, 45);
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(timer);
            writer.Write(retirement);
            writer.Write(phase);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            timer = reader.ReadInt32();
            retirement = reader.ReadInt32();
            phase = reader.ReadSingle();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (!OwnerActive && retirement == 0)
                return false;
            Vector2 center = Projectile.Center;
            float radius = Radius;
            float opacity = MathHelper.Clamp(timer / 45f, 0f, 1f) * MathHelper.Clamp((260f - timer) / 25f, 0f, 1f);
            if (retirement > 0)
                opacity *= MathHelper.SmoothStep(0f, 1f, retirement / 36f);
            float flash = MathHelper.Clamp((70f - timer) / 10f, 0f, 1f) * (timer >= 60 ? 1f : 0f);
            Color color = Color.Lerp(TumblerVFX.PhaseColor(phase), Color.White, flash);
            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64", AssetRequestMode.ImmediateLoad).Value;
                Main.spriteBatch.Draw(glow, center - Main.screenPosition, null, TumblerVFX.Glow(color, opacity * (0.22f + flash * 0.35f)), 0f, glow.Size() * 0.5f, radius * 2.7f / glow.Width, SpriteEffects.None, 0f);
            });
            TumblerVFX.DrawCorona(Main.spriteBatch, center - Main.screenPosition, radius, color, opacity * (timer < 60 ? 0.45f : 0.9f), Projectile.identity, 2.5f);
            TumblerVFX.DrawCorona(Main.spriteBatch, center - Main.screenPosition, radius - 8f, color, opacity * 0.35f, Projectile.identity + 8, 1f);
            for (int i = 0; i < zaps.Length; i++)
                if (zaps[i] != null && zapAges[i] < 12)
                    zaps[i].Draw(Main.spriteBatch, Color.Lerp(color, Color.White, Math.Max(0f, 1f - zapAges[i] / 5f)), opacity * (1f - zapAges[i] / 12f), 2.4f);
            return false;
        }
    }
}
