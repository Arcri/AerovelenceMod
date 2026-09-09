using System;
using System.Collections.Generic;
using System.IO;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Content.Items.BossSummons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler
{
    public class TumblerFilamentRamp : ModProjectile
    {
        private readonly float[] supportHeights = new float[13];
        private int Lifetime => Projectile.ai[1] > 0f ? Math.Max(48, (int)Projectile.ai[1]) : 200;
        private float Age => Math.Max(0f, Lifetime - Projectile.timeLeft);

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 800;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.hide = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.timeLeft = Lifetime;
            for (int i = 0; i < supportHeights.Length; i++)
            {
                float x = Projectile.Center.X + Projectile.velocity.X * i / (supportHeights.Length - 1f);
                supportHeights[i] = ArenaData.Valid ? ArenaData.FindGroundWorldY(x, Projectile.Center.Y - 32f) : Projectile.Center.Y;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.timeLeft);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.timeLeft = reader.ReadInt32();
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;

        public static Vector2 PointOnRamp(Vector2 start, Vector2 rise, float progress)
        {
            progress = MathHelper.Clamp(progress, 0f, 1f);
            return start + new Vector2(rise.X * progress, rise.Y * progress * progress);
        }

        public static Vector2 TangentOnRamp(Vector2 rise, float progress)
        {
            progress = MathHelper.Clamp(progress, 0f, 1f);
            return new Vector2(rise.X, 2f * rise.Y * progress).SafeNormalize(Vector2.UnitX);
        }

        public override void AI()
        {
            int ownerIndex = (int)Projectile.ai[0];
            if (ownerIndex < 0 || ownerIndex >= Main.maxNPCs)
            {
                Projectile.Kill();
                return;
            }

            NPC owner = Main.npc[ownerIndex];
            if (!owner.active || owner.type != ModContent.NPCType<CrystalTumbler>())
            {
                Projectile.Kill();
                return;
            }

            if (owner.ai[0] != (float)TumblerState.ShockDash)
                Projectile.timeLeft = Math.Min(Projectile.timeLeft, 60);

            if (Main.dedServ)
                return;

            float opacity = MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(Age / 35f, 0f, 1f)) * MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(Projectile.timeLeft / 60f, 0f, 1f));
            for (int i = 0; i < 4; i++)
                Lighting.AddLight(PointOnRamp(Projectile.Center, Projectile.velocity, i / 3f), new Vector3(0.5f, 0.28f, 0.07f) * opacity);
            if ((int)Age % 4 == 0 && opacity > 0.2f)
            {
                float progress = Main.rand.NextFloat();
                Vector2 position = PointOnRamp(Projectile.Center, Projectile.velocity, progress);
                Vector2 velocity = TangentOnRamp(Projectile.velocity, progress).RotatedBy(-MathHelper.PiOver2) * Main.rand.NextFloat(0.6f, 1.4f);
                TumblerVFX.SpawnSpark(position, velocity, TumblerVFX.PhaseColor(owner.ai[2]), Main.rand.NextFloat(0.18f, 0.26f));
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(Age / 35f, 0f, 1f)) * MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp(Projectile.timeLeft / 60f, 0f, 1f));
            if (opacity <= 0f)
                return false;

            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderNPCs, () => DrawRamp(opacity));
            return false;
        }

        private void DrawRamp(float opacity)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64").Value;
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 rise = Projectile.velocity;
            float chargeTime = MathHelper.Clamp(Projectile.ai[2], 35f, Lifetime - 24f);
            float charge = MathHelper.SmoothStep(0f, 1f, MathHelper.Clamp((Age - chargeTime + 16f) / 22f, 0f, 1f));
            float time = Age * 0.12f;
            int ownerIndex = (int)Projectile.ai[0];
            Color gold = TumblerVFX.PhaseColor(ownerIndex >= 0 && ownerIndex < Main.maxNPCs ? Main.npc[ownerIndex].ai[2] : 0f);
            Color aqua = new(58, 218, 246);
            Color core = Color.Lerp(gold, Color.White, 0.82f);

            for (int i = 0; i < 24; i++)
            {
                float progress = (i + 0.5f) / 24f;
                Vector2 point = PointOnRamp(start, rise, progress);
                Vector2 tangent = TangentOnRamp(rise, progress);
                float glowPower = opacity * (0.16f + charge * 0.07f);
                spriteBatch.Draw(glow, point, null, Additive(gold, glowPower), tangent.ToRotation(), glow.Size() * 0.5f, new Vector2(30f, 22f + charge * 8f) / glow.Size(), SpriteEffects.None, 0f);
            }

            const int facets = 12;
            for (int i = 0; i < facets; i++)
            {
                float progress = i / (float)facets;
                float next = (i + 1f) / facets;
                Vector2 top = PointOnRamp(start, rise, progress);
                Vector2 nextTop = PointOnRamp(start, rise, next);
                Vector2 lower = Underside(start, rise, progress);
                Vector2 nextLower = Underside(start, rise, next);
                float flicker = 0.74f + 0.26f * MathF.Sin(time - progress * 17f);
                Color latticeColor = Additive(aqua, opacity * (0.26f + charge * 0.16f) * flicker);
                TumblerVFX.DrawLine(spriteBatch, lower, nextLower, latticeColor, 1.4f);
                TumblerVFX.DrawLine(spriteBatch, top, lower, latticeColor * 0.75f, 1f);
                TumblerVFX.DrawLine(spriteBatch, i % 2 == 0 ? top : lower, i % 2 == 0 ? nextLower : nextTop, latticeColor, 1.2f);

                Vector2 node = (top + lower + nextTop + nextLower) * 0.25f;
                float nodeSize = 3.2f + charge * 1.5f;
                spriteBatch.Draw(star, node, null, Additive(aqua, opacity * 0.55f * flicker), MathHelper.PiOver4, star.Size() * 0.5f, nodeSize / star.Width, SpriteEffects.None, 0f);
            }

            const int segments = 32;
            Vector2 previous = start;
            Vector2 previousFilament = start;
            Vector2 previousEcho = start + new Vector2(0f, 6f);
            for (int i = 1; i <= segments; i++)
            {
                float progress = i / (float)segments;
                Vector2 point = PointOnRamp(start, rise, progress);
                float envelope = MathF.Sin(progress * MathHelper.Pi);
                Vector2 normal = TangentOnRamp(rise, progress).RotatedBy(MathHelper.PiOver2);
                float wave = MathF.Sin(progress * 51f - time * 2.2f) + MathF.Sin(progress * 93f + time * 1.4f) * 0.45f;
                Vector2 filament = point + normal * wave * envelope * (1.6f + charge * 2.6f);
                Vector2 echo = point + new Vector2(0f, 6f) + normal * MathF.Sin(progress * 25f - time) * envelope * 2f;
                float flow = 0.7f + 0.3f * MathF.Pow(Math.Max(0f, MathF.Sin(progress * 13f - time * 1.6f)), 4f);
                TumblerVFX.DrawLine(spriteBatch, previousEcho, echo, Additive(aqua, opacity * (0.32f + charge * 0.2f)), 2.2f);
                TumblerVFX.DrawLine(spriteBatch, previous, point, Additive(gold, opacity * 0.85f), 4.5f);
                TumblerVFX.DrawLine(spriteBatch, previous, point, Additive(core, opacity * (0.58f + charge * 0.22f)), 1.5f);
                TumblerVFX.DrawLine(spriteBatch, previousFilament, filament, Additive(core, opacity * (0.3f + charge * 0.5f) * flow), 1.7f);
                previous = point;
                previousFilament = filament;
                previousEcho = echo;
            }

            for (int i = 0; i < 6; i++)
            {
                float progress = (i / 6f + Age * (0.006f + charge * 0.005f)) % 1f;
                Vector2 point = PointOnRamp(start, rise, progress);
                Vector2 tangent = TangentOnRamp(rise, progress);
                float brightness = MathF.Sin(progress * MathHelper.Pi) * opacity;
                spriteBatch.Draw(glow, point, null, Additive(gold, brightness * 0.3f), 0f, glow.Size() * 0.5f, 22f / glow.Width, SpriteEffects.None, 0f);
                spriteBatch.Draw(star, point, null, Additive(core, brightness * 0.9f), tangent.ToRotation(), star.Size() * 0.5f, new Vector2(15f, 7f) / star.Size(), SpriteEffects.None, 0f);
                TumblerVFX.DrawLine(spriteBatch, point - tangent * (13f + charge * 12f), point, Additive(gold, brightness * 0.65f), 2f);
            }

            Vector2 tip = PointOnRamp(start, rise, 1f);
            Vector2 tipTangent = TangentOnRamp(rise, 1f);
            float launchFlash = MathF.Exp(-MathF.Pow((Age - chargeTime - 23f) / 12f, 2f)) * charge;
            float tipPower = opacity * (0.4f + charge * 0.22f + launchFlash * 0.32f);
            spriteBatch.Draw(glow, tip, null, Additive(gold, tipPower * 0.45f), 0f, glow.Size() * 0.5f, (38f + launchFlash * 28f) / glow.Width, SpriteEffects.None, 0f);
            spriteBatch.Draw(star, tip, null, Additive(core, tipPower), tipTangent.ToRotation(), star.Size() * 0.5f, new Vector2(34f + launchFlash * 24f, 16f) / star.Size(), SpriteEffects.None, 0f);

            for (int i = 0; i < 3; i++)
            {
                float progress = (Age * 0.013f + i / 3f) % 1f;
                Vector2 point = tip + tipTangent * (12f + progress * 38f);
                Vector2 wing = tipTangent.RotatedBy(MathHelper.PiOver2) * (4f + progress * 4f);
                Color arrowColor = Additive(gold, opacity * (1f - progress) * (0.32f + charge * 0.26f));
                TumblerVFX.DrawLine(spriteBatch, point - tipTangent * 6f + wing, point, arrowColor, 1.4f);
                TumblerVFX.DrawLine(spriteBatch, point - tipTangent * 6f - wing, point, arrowColor, 1.4f);
            }

        }

        private Vector2 Underside(Vector2 start, Vector2 rise, float progress)
        {
            float sample = MathHelper.Clamp(progress, 0f, 1f) * (supportHeights.Length - 1);
            int index = Math.Min((int)sample, supportHeights.Length - 2);
            float ground = MathHelper.Lerp(supportHeights[index], supportHeights[index + 1], sample - index);
            return new Vector2(start.X + rise.X * progress, ground - Main.screenPosition.Y);
        }

        private static Color Additive(Color color, float opacity)
        {
            color *= opacity;
            color.A = 0;
            return color;
        }
    }
}
