using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Buffs;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.BossSummons;
using AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.CrystalTumbler
{
    public class Geomagnetic : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/BossDrops/CrystalTumbler/Geomagnetic/Geomagnetic";
        private const string Description = "Fires a traveling bolt of geomagnetic lightning\nAt or below 25% mana, casts a hovering sphere that releases three bolts\nThe sphere electrifies enemies on contact";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Geomagnetic", Description).AddSkillStrike(Language.Default, "Strike an enemy with the low-mana sphere itself");
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(t => t.Mod == "Terraria" && t.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", Description));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 20;
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 12;
            Item.useTime = Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.knockBack = 3;
            Item.shootSpeed = 7;
            Item.shoot = ModContent.ProjectileType<GeomagneticBolt>();
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
            Item.UseSound = SoundID.Item93 with { Volume = .45f };
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 aim = velocity.SafeNormalize(Vector2.UnitX * player.direction);
            if (player.statMana <= player.statManaMax2 * .25f)
                Projectile.NewProjectile(source, player.MountedCenter, new Vector2(aim.X * 5, -4), ModContent.ProjectileType<GeomagneticSphere>(), damage, knockback, player.whoAmI, aim.ToRotation());
            else
                Projectile.NewProjectile(source, player.MountedCenter, aim * 7f, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class GeomagneticBolt : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/Pixel/CrispStarPMA";
        private int age;
        private readonly GeomagneticLightningVisual lightning = new();
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            age++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Main.dedServ)
                return;
            Color color = GeomagneticVFX.PhaseColor(0);
            Lighting.AddLight(Projectile.Center, color.ToVector3() * 0.65f);
            if (age % 8 == 0)
            {
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Vector2 sparkVelocity = -direction.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * Main.rand.NextFloat(1.2f, 2.8f);
                GeomagneticVFX.SpawnSpark(Projectile.Center - direction * 12f, sparkVelocity, color, Main.rand.NextFloat(0.16f, 0.24f));
            }
            Vector2 tail = Projectile.Center - Projectile.velocity * Math.Min(age - 1, 15);
            lightning.Update(Projectile, tail, Projectile.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color color = GeomagneticVFX.PhaseColor(0);
            Vector2 tip = Projectile.Center - Main.screenPosition;
            float fade = Math.Min(1f, Projectile.timeLeft / 16f);
            lightning.Draw(color, 0.9f * fade, 1.4f);
            Texture2D star = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(star, tip, null, GeomagneticVFX.Glow(color, fade), Projectile.rotation, star.Size() * .5f, new Vector2(.58f, .29f), SpriteEffects.None);
            Main.EntitySpriteDraw(star, tip, null, GeomagneticVFX.Glow(Color.White, fade), Projectile.rotation, star.Size() * .5f, new Vector2(.27f, .14f), SpriteEffects.None);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
                GeomagneticVFX.SpawnSpark(Projectile.Center - Projectile.velocity * Main.rand.NextFloat(0f, Math.Min(age, 10)), Main.rand.NextVector2Circular(2.5f, 2.5f), GeomagneticVFX.PhaseColor(0), .2f);
        }
    }

    public class GeomagneticSphere : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/Orbs/SoftGlow";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        public override void AI()
        {
            Projectile.velocity *= .93f;
            Projectile.rotation += .075f;
            Projectile.ai[1]++;
            Lighting.AddLight(Projectile.Center, .2f, .5f, .7f);
            if (Projectile.ai[1] == 60)
            {
                SoundEngine.PlaySound(SoundID.Item93 with { Volume = .4f, Pitch = .2f }, Projectile.Center);
                if (Projectile.owner == Main.myPlayer)
                    for (int i = -1; i <= 1; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (Projectile.ai[0] + i * .22f).ToRotationVector2() * 7f, ModContent.ProjectileType<GeomagneticBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            if (Main.rand.NextBool(3)) GeomagneticVFX.SpawnSpark(Projectile.Center + Main.rand.NextVector2CircularEdge(20, 20), Main.rand.NextVector2Circular(1, 1), GeomagneticVFX.PhaseColor(0));
        }
        public override bool OnTileCollide(Vector2 oldVelocity) { Projectile.velocity = Vector2.Zero; return false; }
        public override bool? CanDamage() => Projectile.ai[1] < 65 ? null : false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => SkillStrikeUtil.setSkillStrike(Projectile, 1.5f);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.Electrified, 180);
        public override bool PreDraw(ref Color lightColor)
        {
            Color color = GeomagneticVFX.PhaseColor(0);
            float fade = Math.Min(1, Projectile.timeLeft / 25f) * Math.Min(1, Projectile.ai[1] / 8f);
            Vector2 center = Projectile.Center - Main.screenPosition;
            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/TumblerOrb").Value;
            Rectangle frame = texture.Frame(1, 4, 0, (int)(Projectile.ai[1] / 7) % 4);
            Main.EntitySpriteDraw(texture, center, frame, GeomagneticVFX.Glow(Color.White, fade * 0.8f), Projectile.rotation, frame.Size() * 0.5f, 0.42f, SpriteEffects.None);
            GeomagneticVFX.DrawCharge(Main.spriteBatch, center, color, Math.Min(1f, Projectile.ai[1] / 60f), 18f, -Projectile.rotation, fade);
            if (Projectile.ai[1] >= 25f && Projectile.ai[1] < 60f)
            {
                float warning = MathHelper.Clamp((Projectile.ai[1] - 25f) / 6f, 0f, 1f) * 0.65f;
                for (int i = -1; i <= 1; i++)
                    GeomagneticVFX.DrawTelegraph(Main.spriteBatch, center, center + (Projectile.ai[0] + i * 0.22f).ToRotationVector2() * 240f, color, warning * fade);
            }
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++) GeomagneticVFX.SpawnSpark(Projectile.Center, Main.rand.NextVector2Circular(4, 4), GeomagneticVFX.PhaseColor(0));
        }
    }

    internal sealed class GeomagneticLightningVisual
    {
        private LightningUtils.LightningData lightning;
        private int timer;
        private Vector2 previousStart;
        private Vector2 previousEnd;

        public void Update(Projectile owner, Vector2 start, Vector2 end, float intensity = 0.45f)
        {
            if (Main.dedServ || Vector2.DistanceSquared(start, end) < 1f)
                return;
            LightningUtils.LightningStyle style = LightningUtils.LightningStyle.Jagged;
            if (lightning == null || lightning.Style != style)
            {
                lightning = new LightningUtils.LightningData(owner, style)
                {
                    MaxSegments = Math.Clamp((int)(Vector2.Distance(start, end) / 20f), 12, 56),
                    MaxBranches = 3,
                    BranchChance = 0.45f,
                    NoiseFrequency = 1.7f
                };
            }
            timer++;
            if (lightning.Initialized && timer % 3 != 0 && Vector2.DistanceSquared(start, previousStart) < 16f && Vector2.DistanceSquared(end, previousEnd) < 16f)
                return;
            if (Vector2.DistanceSquared(start, previousStart) > 24f * 24f)
                lightning.Branches?.Clear();
            lightning.DisplacementIntensity = intensity;
            LightningUtils.InitializeBetweenPoints(lightning, start, end, style);
            LightningUtils.UpdateSegments(lightning);
            LightningUtils.UpdateBranches(lightning);
            previousStart = start;
            previousEnd = end;
        }

        public void Draw(Color color, float opacity, float width)
        {
            if (lightning?.SegmentPositions == null || opacity <= 0f)
                return;
            TumblerLightningSystem.DrawPath(lightning.SegmentPositions, color, opacity, width);
            foreach (LightningUtils.Branch branch in lightning.Branches)
                TumblerLightningSystem.DrawPath(branch.Positions, color, opacity * branch.Alpha * 0.55f, Math.Max(1f, width * 0.55f));
        }
    }

    internal static class GeomagneticVFX
    {
        public static void SpawnSpark(Vector2 position, Vector2 velocity, Color color, float scale = 0.2f)
        {
            if (Main.dedServ)
                return;
            Dust spark = Dust.NewDustPerfect(position, ModContent.DustType<ElectricSparkGlow>(), velocity, 0, color, scale);
            spark.customData = new ElectricSparkBehavior(FadeAlphaPower: 0.89f, FadeScalePower: 0.98f, FadeVelPower: 0.92f, TimeBetweenFrames: 3, KillEarlyTime: 24, Pixelize: true, UnderGlowPower: 1.4f, WhiteLayerPower: 0.8f);
        }

        public static Color PhaseColor(float phase)
        {
            return phase >= 1f ? new Color(255, 177, 45) : new Color(45, 226, 255);
        }

        public static Color Glow(Color color, float opacity = 1f)
        {
            color *= MathHelper.Clamp(opacity, 0f, 1f);
            color.A = 0;
            return color;
        }

        public static void DrawElectricLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float opacity, int segments = 18, float seed = 0f, float width = 2f)
        {
            Vector2 fullDelta = end - start;
            if (fullDelta.LengthSquared() < 0.01f || opacity <= 0f)
                return;
            float time = (float)(Main.GameUpdateCount / 3) * 0.83f;
            Vector2 normal = fullDelta.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
            segments = Math.Clamp(segments, 2, 64);
            Vector2[] points = new Vector2[segments + 1];
            points[0] = start + Main.screenPosition;
            for (int i = 1; i <= segments; i++)
            {
                float progress = i / (float)segments;
                Vector2 current = Vector2.Lerp(start, end, progress);
                if (i < segments)
                {
                    float envelope = MathF.Sin(progress * MathHelper.Pi);
                    float wave = MathF.Sin(seed * 3.17f + i * 8.31f + time) + MathF.Sin(seed + i * 3.73f - time * 1.7f) * 0.45f;
                    current += normal * wave * 5f * envelope;
                }
                points[i] = current + Main.screenPosition;
                if (width > 1.5f && i > 2 && i < segments - 1 && i % 5 == 0)
                {
                    float branchSide = MathF.Sin(seed * 2.31f + i * 4.7f) >= 0f ? 1f : -1f;
                    Vector2 branch = (points[i] - points[i - 1]).SafeNormalize(Vector2.UnitX).RotatedBy(branchSide * 0.78f) * (12f + 5f * MathF.Abs(MathF.Sin(seed + i)));
                    DrawPath(new[] { points[i], points[i] + branch * 0.5f + normal * 2f, points[i] + branch }, color, opacity * 0.55f, Math.Max(1f, width * 0.55f));
                }
            }
            DrawPath(points, color, opacity, width);
        }

        public static void DrawTelegraph(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float opacity, float spacing = 46f)
        {
            if (opacity <= 0f)
                return;
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            float distance = Vector2.Distance(start, end);
            int count = Math.Clamp((int)(distance / 28f), 4, 60);
            for (int i = 0; i < count; i++)
            {
                float progress = (i + 0.5f) / count;
                float fade = (1f - progress) * (1f - progress);
                Vector2 segmentStart = Vector2.Lerp(start, end, i / (float)count);
                Vector2 segmentEnd = Vector2.Lerp(start, end, (i + 1f) / count);
                DrawLine(spriteBatch, segmentStart, segmentEnd, Glow(color, opacity * fade * 0.14f), 4f);
                DrawLine(spriteBatch, segmentStart, segmentEnd, Glow(Color.Lerp(color, Color.White, 0.2f), opacity * fade * 0.82f), 1f);
            }
            float lockGlow = MathHelper.Clamp((opacity - 0.45f) / 0.4f, 0f, 1f);
            spriteBatch.Draw(star, start, null, Glow(Color.Lerp(color, Color.White, lockGlow * 0.65f), opacity), (end - start).ToRotation(), star.Size() * 0.5f, new Vector2(0.22f + lockGlow * 0.13f, 0.1f + lockGlow * 0.08f), SpriteEffects.None, 0f);
        }

        public static void DrawCorona(SpriteBatch spriteBatch, Vector2 center, float radius, Color color, float opacity, float seed = 0f, float width = 1.5f)
        {
            if (opacity <= 0f || radius <= 0f)
                return;
            float time = (float)(Main.GameUpdateCount / 3) * 0.65f;
            int segments = Math.Clamp((int)(radius * 0.8f), 12, 48);
            Vector2[] points = new Vector2[segments + 1];
            points[0] = center + Main.screenPosition + new Vector2(radius, 0f);
            for (int i = 1; i <= segments; i++)
            {
                float angle = i / (float)segments * MathHelper.TwoPi;
                float jitter = MathF.Sin(i * 8.17f + seed + time) * 2.4f;
                Vector2 current = center + angle.ToRotationVector2() * (radius + (i == segments ? 0f : jitter));
                points[i] = current + Main.screenPosition;
            }
            DrawPath(points, color, opacity * 0.8f, width);
        }

        public static void DrawCharge(SpriteBatch spriteBatch, Vector2 center, Color color, float progress, float radius, float rotation = 0f, float opacity = 1f)
        {
            if (radius <= 0f || opacity <= 0f)
                return;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            Texture2D star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/CrispStarPMA").Value;
            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64").Value;
            spriteBatch.Draw(glow, center, null, Glow(color, (0.12f + progress * 0.22f) * opacity), rotation, glow.Size() * 0.5f, radius * 3f / glow.Width, SpriteEffects.None, 0f);
            DrawCorona(spriteBatch, center, radius * (1.5f - progress * 0.5f), color, (0.2f + progress * 0.55f) * opacity, rotation);
            spriteBatch.Draw(star, center, null, Glow(color, (0.6f + progress * 0.4f) * opacity), rotation, star.Size() * 0.5f, radius * 2f / star.Width, SpriteEffects.None, 0f);
            spriteBatch.Draw(star, center, null, Glow(Color.White, (0.55f + progress * 0.45f) * opacity), -rotation, star.Size() * 0.5f, radius / star.Width, SpriteEffects.None, 0f);
            for (int i = 0; i < 3; i++)
            {
                float angle = rotation + i * MathHelper.TwoPi / 3f;
                Vector2 tip = center + angle.ToRotationVector2() * radius * (1.5f - progress * 0.5f);
                spriteBatch.Draw(star, tip, null, Glow(color, (0.45f + progress * 0.4f) * opacity), -angle, star.Size() * 0.5f, Math.Min(0.16f, radius / 75f), SpriteEffects.None, 0f);
            }
        }
        private static void DrawPath(Vector2[] points, Color color, float opacity, float width)
        {
            TumblerLightningSystem.DrawPath(points, color, opacity, width);
        }

        public static void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width)
        {
            Vector2 delta = end - start;
            if (delta.LengthSquared() < 0.01f)
                return;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, start, new Rectangle(0, 0, 1, 1), color, delta.ToRotation(), new Vector2(0f, 0.5f), new Vector2(delta.Length(), Math.Max(0.5f, width)), SpriteEffects.None, 0f);
        }
    }
}
