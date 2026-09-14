using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Ammo
{
    public class StoneSlug : StoneSlugSlugItem
    {
        protected override StoneSlugSlugMaterial Material => StoneSlugSlugMaterial.Stone;
        protected override string EnglishTooltip => "A heavy sling stone with a steep, short-range arc\nWell-timed throws deal 2.25x Skill Strike damage";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Stone Slug", EnglishTooltip)
                .AddName(Language.Spanish, "Proyectil de Piedra")
                .AddTooltip(Language.Spanish, "Una piedra pesada para honda con una trayectoria corta y pronunciada\nLos lanzamientos bien sincronizados infligen 2,25 veces el daño con un Golpe de Habilidad");
            base.SetStaticDefaults();
        }
        public override void AddRecipes() => CreateRecipe(50).AddIngredient(ItemID.StoneBlock, 5).AddTile(TileID.WorkBenches).Register();
    }
    public class StoneSlugShot : StoneSlugSlugProjectile
    {
        protected override StoneSlugSlugMaterial Material => StoneSlugSlugMaterial.Stone;
    }

    public enum StoneSlugSlugMaterial { Stone, Wood, Crystal }

    public abstract class StoneSlugSlugItem : TranslatableModItem
    {
        protected abstract StoneSlugSlugMaterial Material { get; }
        protected abstract string EnglishTooltip { get; }
        public override string Texture => "AerovelenceMod/Content/Items/Ammo/StoneSlug";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(copper: 2);
            Item.damage = Material == StoneSlugSlugMaterial.Stone ? 5 : Material == StoneSlugSlugMaterial.Wood ? 2 : 4;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = Material == StoneSlugSlugMaterial.Stone ? 4f : Material == StoneSlugSlugMaterial.Wood ? 2f : 3f;
            Item.ammo = ModContent.ItemType<StoneSlug>();
            Item.shoot = Material == StoneSlugSlugMaterial.Stone ? ModContent.ProjectileType<StoneSlugShot>() :
                Material == StoneSlugSlugMaterial.Wood ? ModContent.ProjectileType<WoodSlugShot>() : ModContent.ProjectileType<CrystalStoneSlugShot>();
            Item.shootSpeed = 1f;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", EnglishTooltip));
            base.ModifyTooltips(tooltips);
        }


    }

    public abstract class StoneSlugSlugProjectile : ModProjectile
    {
        protected abstract StoneSlugSlugMaterial Material { get; }
        protected bool SkillStrike => Projectile.ai[0] == 1f;
        private Vector2 previousCenter;
        private float Opacity => Math.Min(1f, Projectile.timeLeft / 12f);
        public override string Texture => "AerovelenceMod/Content/Items/Ammo/StoneSlug";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 180;
            Projectile.penetrate = 1;
        }
        public override bool? CanDamage() => Projectile.timeLeft <= 12 ? false : null;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.ai[1] == 0f)
                return projHitbox.Intersects(targetHitbox);
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), previousCenter, Projectile.Center, 10f, ref point);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (SkillStrike)
                SkillStrikeUtil.setSkillStrike(Projectile, Material == StoneSlugSlugMaterial.Stone ? 2.25f : Material == StoneSlugSlugMaterial.Wood ? 1.5f : 2f, 1, 0.35f, 0.65f);
        }
        public override void AI()
        {
            previousCenter = Projectile.Center;
            if (Projectile.ai[1]++ == 0f)
            {
                Projectile.velocity *= Material == StoneSlugSlugMaterial.Stone ? 0.9f : Material == StoneSlugSlugMaterial.Wood ? 1.25f : 1f;
                SoundEngine.PlaySound(SoundID.Item1 with { Volume = 0.55f, Pitch = SkillStrike ? 0.55f : 0.1f }, Projectile.Center);
                if (SkillStrike)
                {
                    StoneSlugVFX.Burst(Projectile.Center, 10, 4f);
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Main.LocalPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = Math.Max(Main.LocalPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower, 1.2f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StoneSlugImpact>(),
                            0, 0f, Projectile.owner, 0.5f, Projectile.velocity.ToRotation());
                    }
                }
            }
            float gravity = Material == StoneSlugSlugMaterial.Stone ? 0.36f : Material == StoneSlugSlugMaterial.Wood ? 0.1f : 0.23f;
            Projectile.velocity.Y = Math.Min(18f, Projectile.velocity.Y + gravity);
            Projectile.velocity.X *= Material == StoneSlugSlugMaterial.Wood ? 0.999f : 0.997f;
            Projectile.rotation += Projectile.velocity.X * 0.06f;
            if ((SkillStrike || Material == StoneSlugSlugMaterial.Crystal) && Projectile.ai[1] % 3f == 0f)
                StoneSlugVFX.Spark(Projectile.Center, -Projectile.velocity * 0.06f, 0.17f * Opacity, SkillStrike ? Color.White : StoneSlugSlugArt.Tint(Material));
            if (Material == StoneSlugSlugMaterial.Stone && Projectile.ai[1] < 15f && Projectile.ai[1] % 4f == 0f)
                StoneSlugVFX.Smoke(Projectile.Center, -Projectile.velocity * 0.06f, 18f, new Color(125, 140, 160));
        }
        public override void OnKill(int timeLeft)
        {
            if (timeLeft <= 0 || Main.dedServ)
                return;
            Color tint = StoneSlugSlugArt.Tint(Material);
            StoneSlugVFX.Smoke(Projectile.Center, -Projectile.velocity * 0.05f, SkillStrike ? 55f : 32f, tint * 0.65f);
            if (Material == StoneSlugSlugMaterial.Wood)
            {
                for (int i = 0; i < 7; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.WoodFurniture, Main.rand.NextVector2Circular(3.5f, 3.5f), Scale: 0.9f);
            }
            else
                StoneSlugVFX.Rubble(Projectile.Center, SkillStrike ? 7 : 4, SkillStrike ? 4f : 2.5f);
            if (SkillStrike || Material == StoneSlugSlugMaterial.Crystal)
                StoneSlugVFX.Burst(Projectile.Center, SkillStrike ? 12 : 7, SkillStrike ? 4f : 2.5f);
            SoundEngine.PlaySound(Material == StoneSlugSlugMaterial.Crystal ? SoundID.Shatter with { Volume = 0.3f, Pitch = 0.15f } :
                SoundID.Dig with { Volume = SkillStrike ? 0.6f : 0.4f, Pitch = Material == StoneSlugSlugMaterial.Stone ? -0.25f : 0.15f }, Projectile.Center);
            if (SkillStrike && Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StoneSlugImpact>(), 0, 0f,
                    Projectile.owner, 0.7f, Projectile.velocity.ToRotation());
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color tint = StoneSlugSlugArt.Tint(Material);
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                float fade = (1f - i / (float)Projectile.oldPos.Length) * Opacity;
                StoneSlugSlugArt.Draw(Main.spriteBatch, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition, Material,
                    Projectile.oldRot[i], 12f * (0.7f + fade * 0.3f), SkillStrike ? StoneSlugVFX.Additive(tint, fade * 0.4f) : lightColor * (fade * 0.15f), 0f, fade);
            }
            if (SkillStrike)
                StoneSlugVFX.Glow(Projectile.Center, new Vector2(36f), tint, Opacity * 0.45f);
            StoneSlugSlugArt.Draw(Main.spriteBatch, Projectile.Center - Main.screenPosition, Material, Projectile.rotation, 14f,
                lightColor, SkillStrike ? 0.65f : 0f, Opacity);
            return false;
        }
    }

    internal static class StoneSlugSlugArt
    {
        internal static Color Tint(StoneSlugSlugMaterial material) => material switch
        {
            StoneSlugSlugMaterial.Wood => new Color(220, 177, 115),
            StoneSlugSlugMaterial.Crystal => StoneSlugVFX.Aqua,
            _ => new Color(155, 180, 218)
        };
        internal static void Draw(SpriteBatch spriteBatch, Vector2 center, StoneSlugSlugMaterial material, float angle, float size, Color light, float charge, float opacity = 1f)
        {
            string asset = "AerovelenceMod/Content/Items/Ammo/StoneSlug";
            Texture2D texture = ModContent.Request<Texture2D>(asset).Value;
            float scale = size / Math.Max(texture.Width, texture.Height);
            spriteBatch.Draw(texture, center, null, light * opacity, angle, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            if (charge > 0f)
                spriteBatch.Draw(texture, center, null, StoneSlugVFX.Additive(Color.White, charge * opacity * 0.6f), angle, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }

    public class StoneSlugImpact : ModProjectile
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
            StoneSlugVFX.Glow(Projectile.Center, new Vector2(size * 1.5f), StoneSlugVFX.Violet, fade * 0.5f);
            StoneSlugVFX.Ring(Projectile.Center, new Vector2(size, size * 0.7f), StoneSlugVFX.Aqua, fade * 0.65f, Projectile.ai[1]);
            StoneSlugVFX.Sprite(Texture, Projectile.Center, new Vector2(size * 0.6f), StoneSlugVFX.Additive(StoneSlugVFX.Aqua, fade * 0.4f), Projectile.identity * 2.3f);
            StoneSlugVFX.Flare(Projectile.Center, size * 1.3f, Math.Max(0f, 1f - progress * 3f), Projectile.ai[1]);
            return false;
        }
    }

    public class StoneSlugDebris : ModDust
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
            Main.EntitySpriteDraw(glow, position, dust.frame, StoneSlugVFX.Additive(StoneSlugVFX.Aqua, opacity * 0.7f), dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return false;
        }
    }

    internal static class StoneSlugVFX
    {
        internal const string CrystalTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalShard";
        internal const string RockTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/ChargedStoneProjectile";
        internal static readonly Color Aqua = new(85, 218, 255);
        internal static readonly Color Violet = new(115, 105, 235);
        internal static readonly Rectangle RockFrame = new(4, 6, 54, 48);

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
                Dust.NewDustPerfect(center, ModContent.DustType<StoneSlugDebris>(), new Vector2(Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed * 1.3f, -1f)), Scale: Main.rand.NextFloat(0.35f, 0.75f));
        }
    }
}
