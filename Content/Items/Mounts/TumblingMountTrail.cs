using System;
using System.Collections.Generic;
using System.IO;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Mounts
{
    public class TumblingMountTrail : ModProjectile
    {
        private readonly List<RailPoint> rail = new();
        private readonly TumblerLightningVisual electricity = new();
        private readonly record struct RailPoint(Vector2 Center, Vector2 Normal, ulong FadeAt, int Run, int Sample);
        private bool riding;
        private bool wasRiding;
        private int direction = 1;
        private int run;
        private int sample;
        private int age;
        private int rampAge;
        private float speed;
        public override string Texture => "AerovelenceMod/Blank";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 75;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage()
        {
            Player player = Main.player[Projectile.owner];
            return player.active && !player.dead && player.GetModPlayer<TumblingMountPlayer>().Mounted && Projectile.ai[0] > 0.5f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 nearest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
            return Vector2.DistanceSquared(nearest, Projectile.Center) <= 18f * 18f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            TumblingMountPlayer rider = player.GetModPlayer<TumblingMountPlayer>();
            age++;
            rail.RemoveAll(point => Main.GameUpdateCount >= point.FadeAt + 30);
            if (!player.active || player.dead || !rider.Mounted)
            {
                if (wasRiding)
                    RetireRun();
                riding = wasRiding = false;
                Projectile.ai[0] = Math.Max(0f, Projectile.ai[0] - 0.08f);
                return;
            }
            Projectile.timeLeft = 75;
            Projectile.Center = rider.BallCenter;
            if (Projectile.owner == Main.myPlayer)
            {
                riding = rider.Riding;
                direction = rider.Direction;
                speed = rider.Speed;
                rampAge = rider.RampAge;
                Projectile.ai[0] = rider.Charge;
                Projectile.ai[1] = rider.Angle;
                Projectile.ai[2] = rider.Rotation;
                if (riding != wasRiding || age % 10 == 0)
                    Projectile.netUpdate = true;
            }
            else
            {
                if (riding)
                {
                    float angle = Projectile.ai[1];
                    TumblingRampMotion.Advance(ref angle, ref speed, direction, ++rampAge);
                    Projectile.ai[1] = angle;
                }
                rider.ReceiveRide(riding, Projectile.ai[1], direction, Projectile.ai[0]);
            }
            if (riding)
            {
                if (!wasRiding || rail.Count > 0 && Vector2.DistanceSquared(rail[^1].Center, Projectile.Center) > 100f * 100f)
                {
                    RetireRun();
                    run++;
                    sample = 0;
                }
                if (!Main.dedServ)
                    rail.Add(new RailPoint(Projectile.Center, TumblingRampMotion.Normal(Projectile.ai[1], direction), Main.GameUpdateCount + 35, run, sample++));
                if (!Main.dedServ && age % 4 == 0)
                    TumblerVFX.SpawnSpark(Projectile.Center + TumblingRampMotion.Normal(Projectile.ai[1], direction) * 16f, -player.velocity * 0.15f + Main.rand.NextVector2Circular(1.5f, 1.5f), Color.LightCyan, 0.2f);
            }
            else if (wasRiding)
                RetireRun();
            wasRiding = riding;
            if (!Main.dedServ)
            {
                Vector2 axis = (Main.GlobalTimeWrappedHourly * 3.5f).ToRotationVector2() * 13f;
                electricity.Update(Projectile, Projectile.Center - axis, Projectile.Center + axis, 0.2f);
            }
            Lighting.AddLight(Projectile.Center, TumblerVFX.PhaseColor(0f).ToVector3() * Projectile.ai[0] * 0.45f);
        }

        private void RetireRun()
        {
            for (int i = 0; i < rail.Count; i++)
                if (rail[i].Run == run)
                    rail[i] = rail[i] with { FadeAt = Math.Min(rail[i].FadeAt, Main.GameUpdateCount) };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(riding);
            writer.Write((sbyte)direction);
            writer.Write(speed);
            writer.Write(rampAge);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            riding = reader.ReadBoolean();
            direction = reader.ReadSByte() < 0 ? -1 : 1;
            speed = MathHelper.Clamp(reader.ReadSingle(), TumblingRampMotion.MinimumSpeed, TumblingRampMotion.MaximumSpeed);
            rampAge = Math.Clamp(reader.ReadInt32(), 0, 36000);
            Main.player[Projectile.owner].GetModPlayer<TumblingMountPlayer>().ReceiveRotation(Projectile.ai[2]);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 60);
            if (!Main.dedServ)
                for (int i = 0; i < 6; i++)
                    TumblerVFX.SpawnSpark(Projectile.Center, Main.rand.NextVector2Circular(3.5f, 3.5f), Color.LightCyan, 0.25f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < rail.Count - 1; i++)
            {
                if (rail[i].Sample % 3 != 0)
                    continue;
                int last = i;
                while (last + 1 < rail.Count && last < i + 3 && rail[last + 1].Run == rail[i].Run)
                    last++;
                if (last == i)
                    continue;
                float remaining = (float)rail[i].FadeAt + 30f - Main.GameUpdateCount;
                float opacity = MathHelper.Clamp(remaining / 30f, 0f, 1f);
                int count = last - i + 1;
                Vector2[] upper = new Vector2[count];
                Vector2[] lower = new Vector2[count];
                for (int j = 0; j < count; j++)
                {
                    RailPoint point = rail[i + j];
                    upper[j] = point.Center + point.Normal * 16f;
                    lower[j] = point.Center + point.Normal * 27f;
                }
                DrawRail(upper, lower, opacity);
            }
            if (riding && rail.Count > 0)
            {
                Vector2 center = Projectile.Center;
                float angle = Projectile.ai[1];
                float previewSpeed = speed;
                Vector2[] upper = new Vector2[4];
                Vector2[] lower = new Vector2[4];
                for (int i = 0; i < 4; i++)
                {
                    Vector2 normal = TumblingRampMotion.Normal(angle, direction);
                    upper[i] = center + normal * 16f;
                    lower[i] = center + normal * 27f;
                    center += TumblingRampMotion.Advance(ref angle, ref previewSpeed, direction, rampAge + i + 1);
                }
                DrawRail(upper, lower, 0.45f);
            }
            float charge = Projectile.ai[0];
            if (charge > 0.03f)
            {
                Vector2[] points = new Vector2[17];
                for (int i = 0; i < points.Length; i++)
                {
                    float angle = i / 16f * MathHelper.TwoPi + Main.GlobalTimeWrappedHourly * 2f;
                    float radius = 17f + MathF.Sin(i * 2.7f + Main.GameUpdateCount * 0.3f) * 2f;
                    points[i] = Projectile.Center + angle.ToRotationVector2() * radius;
                }
                TumblerLightningSystem.DrawPath(points, TumblerVFX.PhaseColor(0f), charge * 0.7f, 1f, true, RenderLayer.OverPlayers, 0.4f);
                electricity.Draw(Main.spriteBatch, Color.LightCyan, charge * 0.7f, 1f);
            }
            return false;
        }

        private static void DrawRail(Vector2[] upper, Vector2[] lower, float opacity)
        {
            TumblerLightningSystem.DrawPath(upper, TumblerVFX.PhaseColor(1f), opacity * 0.8f, 2f, false, RenderLayer.UnderNPCs, 0.6f);
            TumblerLightningSystem.DrawPath(lower, TumblerVFX.PhaseColor(0f), opacity * 0.55f, 1f, false, RenderLayer.UnderNPCs, 0.5f);
            TumblerLightningSystem.DrawPath([upper[0], lower[lower.Length / 2], upper[^1]], TumblerVFX.PhaseColor(0f), opacity * 0.3f, 1f, false, RenderLayer.UnderNPCs, 0.3f);
        }
    }
}
