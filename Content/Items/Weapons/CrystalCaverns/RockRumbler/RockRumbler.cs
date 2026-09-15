using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
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
    public class RockRumbler : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/RockRumbler/RockRumbler";
        private const string EnglishTooltip = "Shoulder-fpires rocky energy bombs that shatter in a crystal blast\nThe white-hot launch burst marks the direct-hit Skill Strike window\nBombs lose their thrust and drop heavily after the burst\nDoes not consume ammo";

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Rock Rumbler", EnglishTooltip)
                .AddSkillStrike(Language.Default, "Direct hits during powered burst Skill Strike before gravity begins")
                .AddName(Language.Spanish, "Retumbador de Rocas");
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
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 27;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 42;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<RumblerBomb>();
            Item.shootSpeed = 14f;
            Item.knockBack = 5f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 direction = velocity.SafeNormalize(Vector2.UnitX * player.direction);
            int facing = direction.X < 0f ? -1 : 1;
            Vector2 shoulder = player.MountedCenter + new Vector2(-facing * 5f, -12f);
            Vector2 muzzle = shoulder + direction * 34f;
            if (!Collision.CanHitLine(player.Center, 1, 1, muzzle, 1, 1))
                muzzle = player.Center;
            Projectile.NewProjectile(source, muzzle, velocity, type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, shoulder, direction, ModContent.ProjectileType<RumblerHeld>(), 0, 0f, player.whoAmI, player.itemAnimationMax);
            return false;
        }
    }

    public class RumblerBomb : ModProjectile
    {
        internal const int BurstTicks = 12;
        private int directTarget = -1;
        public override string Texture => RockRumblerVFX.RockTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            bool powered = ++Projectile.ai[0] <= BurstTicks;
            Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;
            if (powered)
                SkillStrikeUtil.setSkillStrike(Projectile, 1.5f, 1, 0.4f, 0.65f);
            else
            {
                Projectile.velocity.Y = Math.Min(18f, Projectile.velocity.Y + 0.6f);
                Projectile.velocity.X *= 0.987f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.035f;
            Lighting.AddLight(Projectile.Center, RockRumblerVFX.Aqua.ToVector3() * (powered ? 0.65f : 0.2f));
            if (Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(SoundID.Item61 with { Volume = 0.5f, Pitch = -0.35f }, Projectile.Center);
                RockRumblerVFX.Burst(Projectile.Center, 10, 3.5f);
            }
            if (Projectile.ai[0] % 2 == 0)
                RockRumblerVFX.Smoke(Projectile.Center, -Projectile.velocity * 0.1f, powered ? 44f : 30f, powered ? RockRumblerVFX.Aqua : new Color(110, 120, 140));
            if (powered)
                RockRumblerVFX.Spark(Projectile.Center, -Projectile.velocity * 0.18f, 0.18f, Color.White);
            if (Projectile.ai[0] == BurstTicks + 1)
                RockRumblerVFX.Burst(Projectile.Center, 5, 1.8f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => directTarget = target.whoAmI;

        public override void OnKill(int timeLeft)
        {
            RockRumblerVFX.Rubble(Projectile.Center, 9, 5f);
            RockRumblerVFX.Burst(Projectile.Center, 18, 6f);
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.55f, Pitch = -0.25f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.3f, Pitch = 0.1f }, Projectile.Center);
            if (!Main.dedServ)
                for (int i = 0; i < 7; i++)
                    RockRumblerVFX.Smoke(Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), Main.rand.NextFloat(70f, 110f), i % 2 == 0 ? RockRumblerVFX.Aqua : RockRumblerVFX.Violet);
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<RumblerBurst>(), Projectile.damage, Projectile.knockBack, Projectile.owner, directTarget + 1);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float powered = MathHelper.Clamp((BurstTicks + 3f - Projectile.ai[0]) / 3f, 0f, 1f);
            for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                float fade = 1f - i / (float)Projectile.oldPos.Length;
                RockRumblerVFX.Glow(Projectile.oldPos[i] + Projectile.Size * 0.5f, new Vector2(26f * fade), RockRumblerVFX.Aqua, fade * 0.35f * powered);
            }
            RockRumblerVFX.Glow(Projectile.Center, new Vector2(65f), RockRumblerVFX.Aqua, 0.3f + powered * 0.25f);
            RockRumblerVFX.Rock(Projectile.Center, Projectile.rotation, 27f, lightColor);
            for (int i = 0; i < 4; i++)
            {
                float angle = Projectile.rotation + i * MathHelper.PiOver2;
                RockRumblerVFX.Crystal(Projectile.Center + angle.ToRotationVector2() * 8f, angle + MathHelper.PiOver2, new Vector2(5f, 12f), 1f, 0.25f + powered * 0.5f);
            }
            if (powered > 0f)
                RockRumblerVFX.Flare(Projectile.Center, 48f, powered * 0.8f, Projectile.velocity.ToRotation() + MathHelper.PiOver2);
            return false;
        }
    }

    public class RumblerBurst : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Assets/ImpactTextures/Burst_09";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 192;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 26;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool? CanDamage() => Projectile.timeLeft >= 20 ? null : false;
        public override bool? CanHitNPC(NPC target) => target.whoAmI == (int)Projectile.ai[0] - 1 ? false : null;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 nearest = Vector2.Clamp(Projectile.Center, targetHitbox.TopLeft(), targetHitbox.BottomRight());
            return Vector2.DistanceSquared(nearest, Projectile.Center) <= 96f * 96f;
        }

        public override void AI() => Lighting.AddLight(Projectile.Center, RockRumblerVFX.Aqua.ToVector3() * Projectile.timeLeft / 26f);

        public override bool PreDraw(ref Color lightColor)
        {
            float progress = 1f - Projectile.timeLeft / 26f;
            float fade = (1f - progress) * (1f - progress);
            float size = MathHelper.Lerp(35f, 190f, MathF.Sqrt(progress));
            RockRumblerVFX.Glow(Projectile.Center, new Vector2(size * 1.4f), RockRumblerVFX.Violet, fade * 0.55f);
            RockRumblerVFX.Ring(Projectile.Center, new Vector2(size), RockRumblerVFX.Aqua, fade * 0.85f);
            RockRumblerVFX.Sprite(Texture, Projectile.Center, new Vector2(size * 0.7f), RockRumblerVFX.Additive(RockRumblerVFX.Aqua, fade * 0.45f), Projectile.identity * 1.7f);
            RockRumblerVFX.Flare(Projectile.Center, 130f + progress * 70f, MathHelper.Clamp(1f - progress * 4f, 0f, 1f), MathHelper.PiOver4);
            return false;
        }
    }

    public class RumblerHeld : ModProjectile
    {
        public override string Texture => RockRumblerVFX.LauncherTexture;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.noItems || player.CCed || player.HeldItem.type != ModContent.ItemType<RockRumbler>())
            {
                Projectile.Kill();
                return;
            }
            if (++Projectile.ai[1] >= Math.Max(12f, Projectile.ai[0]))
            {
                Projectile.Kill();
                return;
            }
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            player.ChangeDir(direction.X < 0f ? -1 : 1);
            float recoil = MathF.Exp(-Projectile.ai[1] / 6f);
            Projectile.rotation = direction.ToRotation() - player.direction * 0.16f * recoil;
            Projectile.Center = player.MountedCenter + new Vector2(-5f * player.direction, -12f) - direction * recoil * 9f;
            player.heldProj = Projectile.whoAmI;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Quarter, Projectile.rotation - MathHelper.PiOver2);
            if (Projectile.ai[1] <= 5)
                RockRumblerVFX.Smoke(Projectile.Center - direction * 23f, -direction * 3f, 48f, new Color(150, 170, 200));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            int facing = Main.player[Projectile.owner].direction;
            SpriteEffects flip = facing < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation,
                texture.Size() * new Vector2(0.43f, 0.5f), 0.8f, flip);
            Vector2 axis = Projectile.rotation.ToRotationVector2();
            float flash = MathHelper.Clamp(1f - Projectile.ai[1] / 9f, 0f, 1f);
            RockRumblerVFX.Flare(Projectile.Center + axis * 34f, 100f * flash, flash, Projectile.rotation + MathHelper.PiOver2);
            return false;
        }
    }

    public class RockRumblerDebris : ModDust
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
            Main.EntitySpriteDraw(glow, position, dust.frame, RockRumblerVFX.Additive(RockRumblerVFX.Aqua, opacity * 0.7f), dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return false;
        }
    }

    internal static class RockRumblerVFX
    {
        internal const string CrystalTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/CrystalShard";
        internal const string RockTexture = "AerovelenceMod/Content/NPCs/Bosses/CrystalTumbler/ChargedStoneProjectile";
        internal const string LauncherTexture = "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/RockRumbler/RockRumbler";
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

        internal static void Rock(Vector2 center, float rotation, float size, Color light, float opacity = 1f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(RockTexture).Value;
            Main.EntitySpriteDraw(texture, center - Main.screenPosition, RockFrame, Color.Lerp(light, Color.White, 0.2f) * opacity,
                rotation, RockFrame.Size() * 0.5f, size / RockFrame.Width, SpriteEffects.None);
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
                Dust.NewDustPerfect(center, ModContent.DustType<RockRumblerDebris>(), new Vector2(Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed * 1.3f, -1f)), Scale: Main.rand.NextFloat(0.35f, 0.75f));
        }
    }
}
