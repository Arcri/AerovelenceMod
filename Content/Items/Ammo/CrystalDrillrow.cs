using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Ammo
{
    public class CrystalDrillrow : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Ammo/CrystalDrillrow";
        private const string EnglishTooltip = "Drills through one continuous section of terrain, up to 12 tiles thick\nEmerges with an energized crystal tip that deals 15% more damage\nThe worn drill breaks against the next wall";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Crystal Drillrow", EnglishTooltip)
                .AddName(Language.Spanish, "Flecha Taladro de Cristal")
                .AddTooltip(Language.Spanish, "Perfora una sección continua de terreno de hasta 12 bloques de grosor\nSale con la punta de cristal energizada y causa un 15% más de daño\nEl taladro desgastado se rompe contra la siguiente pared");
            base.SetStaticDefaults();
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", EnglishTooltip));
            base.ModifyTooltips(tooltips);
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 32;
            Item.height = 12;
            Item.rare = ItemRarityID.Blue;
            Item.damage = 7;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 2f;
            Item.shootSpeed = 3.5f;
            Item.shoot = ModContent.ProjectileType<CrystalDrillrowShot>();
            Item.ammo = AmmoID.Arrow;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(copper: 2);
        }


        public override void AddRecipes() => CreateRecipe(100).AddIngredient<CavernStoneItem>(5).AddIngredient<CavernCrystalItem>(1).AddIngredient(ItemID.WoodenArrow, 100).AddTile(TileID.Anvils).Register();
    }

    internal static class DrillrowArt
    {
        internal const string ShaftTexture = "AerovelenceMod/Content/Items/Ammo/CrystalDrillrow";
        internal static void Draw(SpriteBatch spriteBatch, Vector2 center, float angle, float spin, float energy, Color light, float opacity = 1f, float scale = 1f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(ShaftTexture).Value;
            float rotation = angle - MathHelper.PiOver2;
            Vector2 size = new Vector2(0.94f + MathF.Sin(spin) * 0.06f, 1f) * scale;
            spriteBatch.Draw(texture, center, null, light * opacity, rotation, texture.Size() * 0.5f, size, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, center, null, CrystalDrillrowVFX.Additive(CrystalDrillrowVFX.Aqua, energy * opacity * 0.6f), rotation, texture.Size() * 0.5f, size, SpriteEffects.None, 0f);
            if (energy > 0f)
            {
                Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow").Value;
                Vector2 point = center + angle.ToRotationVector2() * 12f * scale;
                spriteBatch.Draw(glow, point, null, CrystalDrillrowVFX.Additive(CrystalDrillrowVFX.Aqua, energy * opacity * 0.45f), 0f, glow.Size() * 0.5f, 38f * scale / glow.Width, SpriteEffects.None, 0f);
            }
        }
    }

    public class CrystalDrillrowShot : ModProjectile
    {
        private Vector2 entryPoint;
        private bool hasEntry;
        private float exitFlash;
        private bool broken;
        public override string Texture => DrillrowArt.ShaftTexture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.arrow = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage()
            => Projectile.ai[0] != 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height) ? null : false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] == 2f)
                modifiers.SourceDamage *= 1.15f;
        }
        public override void AI()
        {
            Projectile.ai[2]++;
            exitFlash *= 0.82f;
            if (Projectile.ai[0] != 1f && Projectile.ai[2] > 20f)
                Projectile.velocity.Y = Math.Min(16f, Projectile.velocity.Y + 0.05f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 axis = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 origin = Projectile.Center;
            float length = Projectile.velocity.Length();
            int steps = Math.Max(1, (int)MathF.Ceiling(length));
            float distance = length / steps;
            for (int step = 0; step <= steps; step++)
            {
                Vector2 center = origin + Projectile.velocity * (step / (float)steps);
                Vector2 point = center + axis * 12f;
                if (point.X < 16f || point.Y < 16f || point.X >= Main.maxTilesX * 16f - 16f || point.Y >= Main.maxTilesY * 16f - 16f)
                {
                    Projectile.Kill();
                    return;
                }
                bool solid = Collision.SolidCollision(point, 1, 1);
                DrillrowEvent transition = DrillrowTerrain.Sample(ref Projectile.ai[0], ref Projectile.ai[1], solid, step == 0 ? 0f : distance);
                Projectile.Center = center;
                if (transition == DrillrowEvent.Break)
                {
                    broken = true;
                    Projectile.Kill();
                    return;
                }
                if (transition == DrillrowEvent.Enter)
                {
                    entryPoint = point - axis * 3f;
                    hasEntry = true;
                    Array.Clear(Projectile.oldPos);
                    CrystalDrillrowVFX.Rubble(entryPoint, 4, 2.5f);
                    CrystalDrillrowVFX.Smoke(entryPoint, -axis, 40f, new Color(140, 160, 185));
                    SoundEngine.PlaySound(SoundID.Item23 with { Volume = 0.2f, Pitch = 0.3f, MaxInstances = 3 }, point);
                    Projectile.netUpdate = true;
                }
                if (transition == DrillrowEvent.Emerge)
                {
                    exitFlash = 1f;
                    Array.Clear(Projectile.oldPos);
                    CrystalDrillrowVFX.Rubble(point, 4, 3.5f);
                    CrystalDrillrowVFX.Burst(point, 7, 3f);
                    CrystalDrillrowVFX.Smoke(point, axis * 2f, 60f, CrystalDrillrowVFX.Aqua);
                    SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.25f, Pitch = 0.4f, MaxInstances = 3 }, point);
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), point, Vector2.Zero, ModContent.ProjectileType<CrystalDrillrowImpact>(), 0, 0f, Projectile.owner, 0.55f, Projectile.rotation);
                    Projectile.netUpdate = true;
                }
            }
            if (Projectile.ai[0] == 1f && hasEntry && Projectile.ai[2] % 6f == 0f)
            {
                CrystalDrillrowVFX.Spark(entryPoint, -axis * 1.5f, 0.1f);
                CrystalDrillrowVFX.Smoke(entryPoint, -axis * 0.5f, 30f, CrystalDrillrowVFX.Violet);
            }
            if (Projectile.ai[0] == 2f)
            {
                Lighting.AddLight(Projectile.Center, CrystalDrillrowVFX.Aqua.ToVector3() * 0.35f);
                if (Projectile.ai[2] % 3f == 0f)
                    CrystalDrillrowVFX.Spark(Projectile.Center, -axis, 0.13f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => broken = true;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == 1f)
                return false;
            float fade = Math.Min(1f, Projectile.timeLeft / 20f);
            float energy = Projectile.ai[0] == 2f ? 1f : 0f;
            if (energy > 0f)
                for (int i = Projectile.oldPos.Length - 1; i >= 1; i--)
                {
                    if (Projectile.oldPos[i] == Vector2.Zero)
                        continue;
                    float tail = 1f - i / (float)Projectile.oldPos.Length;
                    CrystalDrillrowVFX.Glow(Projectile.oldPos[i] + Projectile.Size * 0.5f, new Vector2(18f * tail), CrystalDrillrowVFX.Aqua, tail * fade * 0.35f);
                }
            DrillrowArt.Draw(Main.spriteBatch, Projectile.Center - Main.screenPosition, Projectile.rotation, Projectile.ai[2] * 1.1f, energy, lightColor, fade);
            if (exitFlash > 0.02f)
                CrystalDrillrowVFX.Flare(Projectile.Center + Projectile.rotation.ToRotationVector2() * 12f, 65f, exitFlash * fade, Projectile.rotation + MathHelper.PiOver2);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (!broken)
                return;
            Vector2 point = Projectile.Center + Projectile.rotation.ToRotationVector2() * 12f;
            CrystalDrillrowVFX.Rubble(point, 4, 3f);
            CrystalDrillrowVFX.Burst(point, Projectile.ai[0] == 2f ? 7 : 3, 2.5f);
            SoundEngine.PlaySound(SoundID.Tink with { Volume = 0.25f, PitchVariance = 0.15f, MaxInstances = 3 }, point);
        }
    }

    internal enum DrillrowEvent { None, Enter, Emerge, Break }

    internal static class DrillrowTerrain
    {
        internal const float MaximumDepth = 192f;
        internal static DrillrowEvent Sample(ref float phase, ref float depth, bool solid, float distance)
        {
            if (solid)
            {
                if (phase == 2f)
                    return DrillrowEvent.Break;
                if (phase == 0f)
                {
                    phase = 1f;
                    return DrillrowEvent.Enter;
                }
                depth += Math.Max(0f, distance);
                if (depth > MaximumDepth + 0.001f)
                    return DrillrowEvent.Break;
            }
            else if (phase == 1f)
            {
                phase = 2f;
                return DrillrowEvent.Emerge;
            }
            return DrillrowEvent.None;
        }
    }

    public class CrystalDrillrowImpact : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/ImpactTextures/Burst_09";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 20;
            Projectile.penetrate = -1;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            float progress = 1f - Projectile.timeLeft / 20f;
            float power = Math.Clamp(Projectile.ai[0], 0.5f, 2f);
            float fade = (1f - progress) * (1f - progress);
            float size = (25f + MathF.Sqrt(progress) * 70f) * power;
            CrystalDrillrowVFX.Glow(Projectile.Center, new Vector2(size * 1.5f), CrystalDrillrowVFX.Violet, fade * 0.5f);
            CrystalDrillrowVFX.Ring(Projectile.Center, new Vector2(size, size * 0.7f), CrystalDrillrowVFX.Aqua, fade * 0.65f, Projectile.ai[1]);
            CrystalDrillrowVFX.Sprite(Texture, Projectile.Center, new Vector2(size * 0.6f), CrystalDrillrowVFX.Additive(CrystalDrillrowVFX.Aqua, fade * 0.4f), Projectile.identity * 2.3f);
            CrystalDrillrowVFX.Flare(Projectile.Center, size * 1.3f, Math.Max(0f, 1f - progress * 3f), Projectile.ai[1]);
            return false;
        }
    }

    public class CrystalDrillrowDebris : ModDust
    {
        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/Magnetic_Platform_Debris";
        private static readonly Rectangle[] Frames = [new(0, 0, 12, 22), new(16, 0, 18, 22), new(38, 0, 26, 22)];

        public override void OnSpawn(Dust dust)
        {
            dust.frame = Frames[Main.rand.Next(Frames.Length)];
            dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
            dust.fadeIn = Main.rand.NextFloat(-0.12f, 0.12f);
            dust.noGravity = dust.noLight = true;
            dust.customData = 0;
        }

        public override bool Update(Dust dust)
        {
            int age = (int)dust.customData + 1;
            dust.customData = age;
            dust.velocity.Y = Math.Min(12f, dust.velocity.Y + 0.22f);
            Vector2 movement = Collision.TileCollision(dust.position - Vector2.One * 2f, dust.velocity, 4, 4);
            dust.position += movement;
            if (movement.Y != dust.velocity.Y)
            {
                dust.velocity.Y *= -0.3f;
                dust.velocity.X *= 0.7f;
                dust.fadeIn *= 0.6f;
            }
            if (movement.X != dust.velocity.X)
                dust.velocity.X *= -0.3f;
            dust.rotation += dust.fadeIn;
            if (age > 26)
                dust.alpha += 9;
            if (dust.alpha >= 255)
                dust.active = false;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glow = ModContent.Request<Texture2D>(Texture + "_Glowmask").Value;
            float opacity = 1f - dust.alpha / 255f;
            Vector2 position = dust.position - Main.screenPosition;
            Color light = Lighting.GetColor(dust.position.ToTileCoordinates());
            Main.EntitySpriteDraw(texture, position, dust.frame, Color.Lerp(light, Color.White, 0.25f) * opacity, dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(glow, position, dust.frame, CrystalDrillrowVFX.Additive(CrystalDrillrowVFX.Aqua, opacity * 0.7f), dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return false;
        }
    }

    internal static class CrystalDrillrowVFX
    {
        internal const string CrystalTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalShard";
        internal static readonly Color Aqua = new(85, 218, 255);
        internal static readonly Color Violet = new(115, 105, 235);

        internal static Color Additive(Color color, float opacity)
            => color with { A = 0 } * MathHelper.Clamp(opacity, 0f, 1f);

        internal static void Sprite(string asset, Vector2 center, Vector2 size, Color color, float rotation = 0f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(asset).Value;
            Main.EntitySpriteDraw(texture, center - Main.screenPosition, null, color, rotation, texture.Size() * 0.5f, size / texture.Size(), SpriteEffects.None);
        }

        internal static void Glow(Vector2 center, Vector2 size, Color color, float opacity)
            => Sprite("AerovelenceMod/Assets/Orbs/SoftGlow", center, size, Additive(color, opacity));

        internal static void Ring(Vector2 center, Vector2 size, Color color, float opacity, float rotation = 0f)
            => Sprite("AerovelenceMod/Assets/Ring/GlowRing", center, size, Additive(color, opacity), rotation);

        internal static void Flare(Vector2 center, float size, float opacity, float rotation = 0f)
            => Sprite("AerovelenceMod/Assets/ImpactTextures/Spike", center, new Vector2(size * 0.65f, size), Additive(Color.White, opacity), rotation);

        internal static void Crystal(Vector2 center, float rotation, Vector2 size, float opacity = 1f, float charge = 0.3f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(CrystalTexture).Value;
            Vector2 screen = center - Main.screenPosition;
            Vector2 scale = size / texture.Size();
            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = (i * MathHelper.PiOver2 + rotation).ToRotationVector2() * (1f + charge);
                Main.EntitySpriteDraw(texture, screen + offset, null, Additive(Aqua, opacity * charge * 0.45f), rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            }
            Main.EntitySpriteDraw(texture, screen, null, Color.White * opacity, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
            Main.EntitySpriteDraw(texture, screen, null, Additive(Color.White, opacity * charge), rotation, texture.Size() * 0.5f, scale, SpriteEffects.None);
        }

        internal static void Spark(Vector2 center, Vector2 velocity, float scale = 0.2f, Color? color = null)
        {
            if (Main.dedServ)
                return;
            Dust dust = Dust.NewDustPerfect(center, ModContent.DustType<GlowPixelCross>(), velocity, newColor: color ?? Aqua, Scale: scale);
            dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.1f, preSlowPower: 0.95f,
                timeBeforeSlow: 6, postSlowPower: 0.86f, velToBeginShrink: 1f, fadePower: 0.86f, shouldFadeColor: false);
        }

        internal static void Burst(Vector2 center, int count, float speed)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < count; i++)
                Spark(center, Main.rand.NextVector2CircularEdge(speed, speed) * Main.rand.NextFloat(0.4f, 1f), Main.rand.NextFloat(0.13f, 0.28f));
        }

        internal static void Smoke(Vector2 center, Vector2 velocity, float size, Color color)
        {
            if (Main.dedServ)
                return;
            Dust dust = Dust.NewDustPerfect(center, ModContent.DustType<HighResSmoke>(), velocity, newColor: color, Scale: size / 128f);
            dust.customData = new HighResSmokeBehavior
            {
                randomSmokeNumber = 1,
                frameToStartFade = 8,
                fadeDuration = 28,
                velSlowAmount = 0.96f,
                drawSoftGlowUnder = false,
                overallAlpha = 0.8f
            };
        }

        internal static void Rubble(Vector2 center, int count, float speed = 4f)
        {
            if (Main.dedServ)
                return;
            for (int i = 0; i < count; i++)
                Dust.NewDustPerfect(center, ModContent.DustType<CrystalDrillrowDebris>(), new Vector2(Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed * 1.3f, -1f)), Scale: Main.rand.NextFloat(0.35f, 0.75f));
        }
    }
}
