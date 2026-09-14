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
        private const string Description = "Fires a crackling beam of geomagnetic lightning\nAt or below 25% mana, casts a hovering sphere that releases three bolts\nThe sphere electrifies enemies on contact";
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
            Item.shootSpeed = 1;
            Item.shoot = ModContent.ProjectileType<GeomagneticBeam>();
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
                Projectile.NewProjectile(source, player.MountedCenter, aim, type, damage, knockback, player.whoAmI);
            return false;
        }
    }

    public class GeomagneticBeam : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/Orbs/SoftGlow";
        private float length;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 20;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            if (length == 0)
            {
                float[] samples = new float[3];
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Collision.LaserScan(Projectile.Center, Projectile.velocity, 4, 720, samples);
                length = Math.Max(1, Math.Min(samples[0], Math.Min(samples[1], samples[2])));
            }
            Lighting.AddLight(Projectile.Center, .15f, .3f, .5f);
        }
        public override bool? CanDamage() => Projectile.timeLeft >= 14 ? null : false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * length, 12, ref point);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float fade = Projectile.timeLeft / 20f;
            GeomagneticVFX.DrawElectricLine(Main.spriteBatch, Projectile.Center - Main.screenPosition, Projectile.Center + Projectile.velocity * length - Main.screenPosition, GeomagneticVFX.PhaseColor(0), fade, 26, Projectile.identity * 17, 2.5f);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 9; i++)
                GeomagneticVFX.SpawnSpark(Projectile.Center + Projectile.velocity * length * Main.rand.NextFloat(), Main.rand.NextVector2Circular(2, 2), GeomagneticVFX.PhaseColor(0), .18f);
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
            Projectile.rotation += .07f;
            Projectile.ai[1]++;
            Lighting.AddLight(Projectile.Center, .2f, .5f, .7f);
            if (Projectile.ai[1] == 60)
            {
                SoundEngine.PlaySound(SoundID.Item93 with { Volume = .4f, Pitch = .2f }, Projectile.Center);
                if (Projectile.owner == Main.myPlayer)
                    for (int i = -1; i <= 1; i++)
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (Projectile.ai[0] + i * .22f).ToRotationVector2(), ModContent.ProjectileType<GeomagneticBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
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
            Rectangle frame = texture.Frame(1, 4, 0, (int)(Projectile.ai[1] / 5) % 4);
            Main.EntitySpriteDraw(texture, center, frame, GeomagneticVFX.Glow(color, fade), Projectile.rotation, frame.Size() / 2, 38f / frame.Width, SpriteEffects.None);
            GeomagneticVFX.DrawCharge(Main.spriteBatch, center, color, Math.Min(1, Projectile.ai[1] / 60), 23, Projectile.rotation, fade);
            for (int i = 0; i < 3; i++)
                GeomagneticVFX.DrawElectricLine(Main.spriteBatch, center, center + (Projectile.rotation + i * MathHelper.TwoPi / 3).ToRotationVector2() * 20, color, fade, 8, i * 19 + Projectile.identity, 1.5f);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++) GeomagneticVFX.SpawnSpark(Projectile.Center, Main.rand.NextVector2Circular(4, 4), GeomagneticVFX.PhaseColor(0));
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
            private static ulong dustTick;
        private static int dustBudget;
        private static void DrawPath(Vector2[] points, Color color, float opacity, float width)
        {
            if (Main.dedServ || points.Length < 2 || opacity <= 0f)
                return;
            Vector2[] snapshot = (Vector2[])points.Clone();
            float alpha = MathHelper.Clamp(opacity, 0f, 1f);
            PixellationSystem.QueuePixelationAction(() =>
            {
                float thickness = Math.Max(2f, width) * 0.5f;
                Color core = Color.Lerp(color, Color.White, 0.9f) * alpha;
                Color middle = color * (alpha * 0.55f);
                Color outer = color * (alpha * 0.26f);
                float pulse = 0.85f + 0.15f * MathF.Sin(Main.GameUpdateCount * 0.2f);
                Color bloom = color * (alpha * 0.1f * pulse);
                core.A = middle.A = outer.A = bloom.A = 255;
                for (int i = 1; i < snapshot.Length; i++)
                {
                    Vector2 start = (snapshot[i - 1] - Main.screenPosition) * 0.5f;
                    Vector2 end = (snapshot[i] - Main.screenPosition) * 0.5f;
                    for (int halo = 4; halo >= 1; halo--)
                    {
                        Color tint = bloom * ((5f - halo) / 5f);
                        tint.A = 255;
                        DrawLine(Main.spriteBatch, start, end, tint, thickness + halo * 2f);
                    }
                    DrawLine(Main.spriteBatch, start, end, outer, thickness + 3f);
                    DrawLine(Main.spriteBatch, start, end, middle, thickness + 1.5f);
                    DrawLine(Main.spriteBatch, start, end, core, thickness);
                }
            }, PixellationSystem.RenderType.Additive);
            if (dustTick != Main.GameUpdateCount)
            {
                dustTick = Main.GameUpdateCount;
                dustBudget = 6;
            }
            if (!Main.gamePaused && dustBudget > 0 && Main.rand.NextBool(3))
            {
                int segment = Main.rand.Next(1, snapshot.Length);
                Vector2 point = Vector2.Lerp(snapshot[segment - 1], snapshot[segment], Main.rand.NextFloat());
                SpawnSpark(point, Main.rand.NextVector2Circular(1.5f, 1.5f), Color.Lerp(color, Color.White, 0.5f), 0.18f * alpha);
                dustBudget--;
            }
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
