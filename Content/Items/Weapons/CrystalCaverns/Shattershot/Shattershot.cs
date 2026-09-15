using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Natural;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns
{
    public class Shattershot : TranslatableModItem
    {
        internal const string GunTexture = "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/Shattershot/Shattershot";
        private const string EnglishTooltip = "Blasts four heavy pieces of flint and gravel in a wide spread\nConsumes one stone block or cavern stone per blast\nCavern stone fires glowing gravel and sometimes an extra white-hot crystal fragment";
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/Shattershot/Shattershot";

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Shattershot", EnglishTooltip)
                .AddSkillStrike(Language.Default, "The white-hot crystal Skill Strikes")
                .AddName(Language.Spanish, "Romperrocas")
                .AddTooltip(Language.Spanish, "Dispara cuatro trozos pesados de pedernal y grava en un amplio abanico\nConsume un bloque de piedra o piedra cavernosa por disparo\nLa piedra cavernosa dispara grava luminosa y, a veces, un fragmento adicional de cristal incandescente");
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
            Item.width = 54;
            Item.height = 22;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ShattershotPellet>();
            Item.shootSpeed = 11f;
            Item.knockBack = 3f;
        }

        private static int FindStone(Player player)
        {
            for (int i = 0; i < 58; i++)
                if (player.inventory[i].stack > 0 && (player.inventory[i].type == ItemID.StoneBlock || player.inventory[i].type == ModContent.ItemType<CavernStoneItem>()))
                    return i;
            return -1;
        }

        public override bool CanUseItem(Player player) => FindStone(player) >= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int slot = FindStone(player);
            if (slot < 0)
                return false;
            bool crystal = player.inventory[slot].type == ModContent.ItemType<CavernStoneItem>();
            if (--player.inventory[slot].stack <= 0)
                player.inventory[slot].TurnToAir();
            Vector2 aim = velocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 muzzle = player.MountedCenter + aim * 40f;
            if (!Collision.CanHitLine(player.Center, 1, 1, muzzle, 1, 1))
                muzzle = player.Center;
            for (int i = 0; i < 4; i++)
                Projectile.NewProjectile(source, muzzle, velocity.RotatedBy((i - 1.5f) * 0.13f + Main.rand.NextFloat(-0.025f, 0.025f)) * Main.rand.NextFloat(0.85f, 1.12f),
                    type, damage, knockback, player.whoAmI, crystal ? 1f : 0f);
            bool fragment = crystal && Main.rand.NextBool(3);
            if (fragment)
                Projectile.NewProjectile(source, muzzle, velocity.RotatedBy(Main.rand.NextFloat(-0.08f, 0.08f)) * 1.2f,
                    type, damage, knockback, player.whoAmI, 2f);
            Projectile.NewProjectile(source, player.MountedCenter, aim, ModContent.ProjectileType<ShattershotHeld>(), 0, 0f, player.whoAmI, player.itemAnimationMax, crystal ? 1f : 0f);
            Projectile.NewProjectile(source, muzzle, aim, ModContent.ProjectileType<ShattershotMuzzle>(), 0, 0f, player.whoAmI, crystal ? 1f : 0f, fragment ? 1f : 0f);
            return false;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<CavernStoneItem>(35).AddIngredient<CavernCrystalItem>(8).AddRecipeGroup(RecipeGroupID.IronBar, 8).AddTile(TileID.Anvils).Register();
    }

    public class ShattershotPellet : ModProjectile
    {
        private static readonly Rectangle[] GravelFrames = [new(0, 0, 12, 22), new(16, 0, 18, 22), new(38, 0, 26, 22)];
        public override string Texture => "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/Magnetic_Platform_Debris";
        private bool IsFragment => Projectile.ai[0] == 2f;
        private bool IsCrystal => Projectile.ai[0] > 0f;
        private Color Tint => Projectile.identity % 2 == 0 ? ShattershotVFX.Aqua : new Color(150, 150, 255);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 100;
            Projectile.penetrate = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage() => Projectile.timeLeft <= 10 ? false : null;

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (IsFragment)
                SkillStrikeUtil.setSkillStrike(Projectile, 1.6f, 1, 0.3f, 0.5f);
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            Projectile.velocity.Y = Math.Min(16f, Projectile.velocity.Y + (IsFragment ? 0.22f : 0.34f));
            Projectile.velocity.X *= 0.992f;
            Projectile.rotation = IsFragment ? Projectile.velocity.ToRotation() + MathHelper.PiOver2 : Projectile.rotation + Projectile.velocity.X * 0.065f;
            if (IsCrystal)
            {
                Lighting.AddLight(Projectile.Center, Tint.ToVector3() * 0.25f);
                if (Projectile.ai[1] % (IsFragment ? 2 : 5) == 0)
                    ShattershotVFX.Spark(Projectile.Center, -Projectile.velocity * 0.08f, IsFragment ? 0.19f : 0.12f, IsFragment ? Color.White : Tint);
            }
            else if (Projectile.ai[1] <= 12 && Projectile.ai[1] % 4 == 0)
                ShattershotVFX.Smoke(Projectile.Center, -Projectile.velocity * 0.06f, 14f, new Color(110, 115, 130));
        }

        public override void OnKill(int timeLeft)
        {
            if (timeLeft <= 0 || Main.dedServ)
                return;
            ShattershotVFX.Rubble(Projectile.Center, IsFragment ? 4 : 2, 2f);
            ShattershotVFX.Smoke(Projectile.Center, -Projectile.velocity * 0.08f, 24f, IsCrystal ? Tint * 0.7f : new Color(135, 130, 120));
            if (IsCrystal)
                ShattershotVFX.Burst(Projectile.Center, IsFragment ? 9 : 4, IsFragment ? 3.8f : 2f);
            if (IsFragment)
            {
                SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.22f, Pitch = 0.25f, MaxInstances = 3 }, Projectile.Center);
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<ShattershotImpact>(), 0, 0f, Projectile.owner, 0.55f, Projectile.velocity.ToRotation());
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float opacity = Math.Min(1f, Projectile.timeLeft / 10f);
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Rectangle frame = GravelFrames[Projectile.identity % GravelFrames.Length];
            float scale = (11f + Projectile.identity % 3) / Math.Max(frame.Width, frame.Height);
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                float fade = (1f - i / (float)Projectile.oldPos.Length) * opacity;
                Vector2 center = Projectile.oldPos[i] + Projectile.Size * 0.5f;
                if (IsFragment)
                    ShattershotVFX.Sprite(ShattershotVFX.CrystalTexture, center, new Vector2(5f, 17f) * (0.6f + fade * 0.4f),
                        ShattershotVFX.Additive(Tint, fade * 0.35f), Projectile.oldRot[i]);
                else
                    Main.EntitySpriteDraw(texture, center - Main.screenPosition, frame, IsCrystal ? ShattershotVFX.Additive(Tint, fade * 0.24f) : lightColor * (fade * 0.15f),
                        Projectile.oldRot[i], frame.Size() * 0.5f, scale * (0.6f + fade * 0.4f), SpriteEffects.None);
            }
            if (IsFragment)
            {
                ShattershotVFX.Glow(Projectile.Center, new Vector2(32f), Tint, opacity * 0.6f);
                ShattershotVFX.Crystal(Projectile.Center, Projectile.rotation, new Vector2(7f, 20f), opacity, 0.8f);
                ShattershotVFX.Flare(Projectile.Center, 28f, opacity * 0.7f, Projectile.rotation);
            }
            else
            {
                if (IsCrystal)
                    ShattershotVFX.Glow(Projectile.Center, new Vector2(22f), Tint, opacity * 0.35f);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame, (IsCrystal ? Color.Lerp(Tint, Color.White, 0.45f) : lightColor) * opacity,
                    Projectile.rotation, frame.Size() * 0.5f, scale, SpriteEffects.None);
                if (IsCrystal)
                    Main.EntitySpriteDraw(ModContent.Request<Texture2D>(Texture + "_Glowmask").Value, Projectile.Center - Main.screenPosition, frame,
                        ShattershotVFX.Additive(Tint, opacity * 0.9f), Projectile.rotation, frame.Size() * 0.5f, scale, SpriteEffects.None);
            }
            return false;
        }
    }

    public class ShattershotHeld : ModProjectile
    {
        public override string Texture => Shattershot.GunTexture;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float duration = Math.Max(8f, Projectile.ai[0]);
            if (!player.active || player.dead || player.CCed || player.noItems || player.HeldItem.type != ModContent.ItemType<Shattershot>() || ++Projectile.ai[2] >= duration)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            Vector2 aim = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            player.ChangeDir(aim.X >= 0f ? 1 : -1);
            float recoil = MathF.Exp(-Projectile.ai[2] / Math.Min(5f, duration * 0.15f));
            Projectile.rotation = aim.ToRotation() - player.direction * 0.2f * recoil;
            Projectile.Center = player.MountedCenter + aim * (14f - 10f * recoil);
            player.heldProj = Projectile.whoAmI;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, Projectile.rotation - MathHelper.PiOver2);
            if (Projectile.ai[2] == (int)(duration * 0.58f))
                SoundEngine.PlaySound(SoundID.Item149 with { Volume = 0.25f, Pitch = -0.4f }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glow = texture;
            SpriteEffects flip = Projectile.velocity.X < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;
            float charge = MathF.Exp(-Projectile.ai[2] / 10f);
            Vector2 center = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, center, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, 0.95f, flip);
            Main.EntitySpriteDraw(glow, center, null, ShattershotVFX.Additive(Projectile.ai[1] > 0f ? ShattershotVFX.Aqua : Color.LightSteelBlue, charge * 0.3f),
                Projectile.rotation, texture.Size() * 0.5f, 0.95f, flip);
            return false;
        }
    }

    public class ShattershotMuzzle : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/MuzzleFlashes/WhitePixelMuzzleFlash";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 12;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            if (Projectile.localAI[0]++ != 0f || Main.dedServ)
                return;
            Vector2 aim = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Color tint = Projectile.ai[0] > 0f ? ShattershotVFX.Aqua : new Color(180, 190, 210);
            SoundEngine.PlaySound(SoundID.Item36 with { Volume = 0.65f, Pitch = -0.3f, PitchVariance = 0.08f }, Projectile.Center);
            if (Projectile.ai[1] > 0f)
                SoundEngine.PlaySound(SoundID.Item27 with { Volume = 0.3f, Pitch = 0.3f }, Projectile.Center);
            if (Projectile.owner == Main.myPlayer)
                Main.LocalPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower = Math.Max(Main.LocalPlayer.GetModPlayer<AeroPlayer>().ScreenShakePower, 1.5f);
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = aim.RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(1f, 4f);
                ShattershotVFX.Smoke(Projectile.Center, velocity, Main.rand.NextFloat(24f, 42f), Color.Lerp(new Color(100, 110, 130), tint, i / 10f));
                ShattershotVFX.Spark(Projectile.Center, velocity * 2f, Main.rand.NextFloat(0.12f, 0.22f), Projectile.ai[1] > 0f ? Color.White : tint);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float age = 12f - Projectile.timeLeft;
            float fade = Math.Max(0f, 1f - age / 8f);
            float angle = Projectile.velocity.ToRotation();
            Vector2 center = Projectile.Center + Projectile.velocity * (13f + age);
            Color tint = Projectile.ai[0] > 0f ? ShattershotVFX.Aqua : new Color(210, 215, 235);
            Vector2 size = new(48f + age * 2f, 30f - age);
            ShattershotVFX.Sprite(Texture + "Glow", center, size * 1.4f, ShattershotVFX.Additive(tint, fade * 0.55f), angle);
            ShattershotVFX.Sprite(Texture, center, size, ShattershotVFX.Additive(Color.White, fade), angle);
            float smokeFade = Projectile.timeLeft / 12f;
            ShattershotVFX.Glow(Projectile.Center, new Vector2(65f), tint, smokeFade * smokeFade * 0.35f);
            return false;
        }
    }

    public class ShattershotImpact : ModProjectile
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
            ShattershotVFX.Glow(Projectile.Center, new Vector2(size * 1.5f), ShattershotVFX.Violet, fade * 0.5f);
            ShattershotVFX.Ring(Projectile.Center, new Vector2(size, size * 0.7f), ShattershotVFX.Aqua, fade * 0.65f, Projectile.ai[1]);
            ShattershotVFX.Sprite(Texture, Projectile.Center, new Vector2(size * 0.6f), ShattershotVFX.Additive(ShattershotVFX.Aqua, fade * 0.4f), Projectile.identity * 2.3f);
            ShattershotVFX.Flare(Projectile.Center, size * 1.3f, Math.Max(0f, 1f - progress * 3f), Projectile.ai[1]);
            return false;
        }
    }

    public class ShattershotDebris : ModDust
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
            Main.EntitySpriteDraw(glow, position, dust.frame, ShattershotVFX.Additive(ShattershotVFX.Aqua, opacity * 0.7f), dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return false;
        }
    }

    internal static class ShattershotVFX
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
                Dust.NewDustPerfect(center, ModContent.DustType<ShattershotDebris>(), new Vector2(Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed * 1.3f, -1f)), Scale: Main.rand.NextFloat(0.35f, 0.75f));
        }
    }
}
