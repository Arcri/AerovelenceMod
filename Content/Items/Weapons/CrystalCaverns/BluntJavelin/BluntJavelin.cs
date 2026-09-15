using AerovelenceMod.Common.Globals.SkillStrikes;
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
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns
{
    public class BluntJavelin : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/BluntJavelin/BluntJavelin";
        private const string EnglishTooltip = "A heavy cavern-stone javelin that shatters on impact";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Blunt Javelin", EnglishTooltip)
                .AddSkillStrike(Language.Default, "Skill Strikes while falling steeply downwards")
                .AddName(Language.Spanish, "Jabalina Roma");
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
            Item.width = Item.height = 30;
            Item.rare = ItemRarityID.Blue;
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = Item.noUseGraphic = Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BluntJavelinShot>();
            Item.shootSpeed = 10.5f;
            Item.knockBack = 7f;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(copper: 4);
            Item.UseSound = SoundID.Item1 with { Volume = 0.45f, Pitch = -0.15f };
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 aim = velocity.SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.NewProjectile(source, player.MountedCenter, aim, ModContent.ProjectileType<BluntJavelinHeld>(), 0, 0f, player.whoAmI);
            return true;
        }


        public override void AddRecipes() => CreateRecipe(25).AddIngredient<CavernStoneItem>(2).AddTile(TileID.WorkBenches).Register();
    }

    internal static class BluntJavelinArt
    {
        internal const string ShaftTexture = "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/BluntJavelin/BluntJavelin";
        internal static readonly Rectangle ShaftFrame = new(0, 24, 76, 76);
        internal static void Draw(SpriteBatch spriteBatch, Vector2 center, float angle, float scale, Color light, float charge, float opacity = 1f)
        {
            Texture2D texture = ModContent.Request<Texture2D>(ShaftTexture).Value;
            spriteBatch.Draw(texture, center, null, light * opacity, angle + MathHelper.PiOver4, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, center, null, BluntJavelinVFX.Additive(Color.White, charge * opacity * 0.6f), angle + MathHelper.PiOver4, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }

    public class BluntJavelinHeld : ModProjectile
    {
        public override string Texture => BluntJavelinArt.ShaftTexture;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 9;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
                return;
            }
            Vector2 aim = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);
            player.ChangeDir(aim.X < 0f ? -1 : 1);
            player.heldProj = Projectile.whoAmI;
            float progress = 1f - Projectile.timeLeft / 9f;
            Projectile.Center = player.MountedCenter + new Vector2(0f, -10f) + aim * (8f + progress * 14f);
            Projectile.rotation = aim.ToRotation();
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, aim.ToRotation() - MathHelper.PiOver2 - player.direction * 0.4f * (1f - progress));
        }
        public override bool PreDraw(ref Color lightColor) => false;
    }

    public class BluntJavelinShot : ModProjectile
    {
        private bool impacted;
        private float charge;
        public override string Texture => BluntJavelinArt.ShaftTexture;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 240;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
            if (++Projectile.ai[0] > 10f)
            {
                Projectile.velocity.Y = BluntJavelinWeaponMotion.JavelinGravity(Projectile.velocity.Y);
                Projectile.velocity.X *= 0.994f;
            }
            bool slam = BluntJavelinWeaponMotion.JavelinCanSlam(Projectile.velocity.X, Projectile.velocity.Y);
            Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;
            if (slam)
                SkillStrikeUtil.setSkillStrike(Projectile, 1.5f, 1, 0.35f, 0.6f);
            if (slam && Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                BluntJavelinVFX.Burst(Projectile.Center, 5, 1.8f);
            }
            charge = MathHelper.Lerp(charge, slam ? 1f : 0f, 0.3f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (slam && Projectile.ai[0] % 3f == 0f)
                BluntJavelinVFX.Spark(Projectile.Center, -Projectile.velocity * 0.12f, 0.17f, Color.White);
            if (slam)
                Lighting.AddLight(Projectile.Center, BluntJavelinVFX.Aqua.ToVector3() * 0.25f);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            impacted = true;
            return true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => impacted = true;
        public override bool PreDraw(ref Color lightColor)
        {
            float fade = Math.Min(1f, Projectile.timeLeft / 20f);
            for (int i = Projectile.oldPos.Length - 1; i >= 2; i -= 2)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;
                float trail = (1f - i / (float)Projectile.oldPos.Length) * charge * fade;
                BluntJavelinArt.Draw(Main.spriteBatch, Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition,
                    Projectile.rotation, 1f, BluntJavelinVFX.Additive(BluntJavelinVFX.Aqua, 0.22f), 0f, trail);
            }
            BluntJavelinArt.Draw(Main.spriteBatch, Projectile.Center - Main.screenPosition, Projectile.rotation, 1f, lightColor, charge, fade);
            if (charge > 0.1f)
                BluntJavelinVFX.Flare(Projectile.Center + Projectile.rotation.ToRotationVector2() * 17f, 50f, charge * fade * 0.6f, Projectile.rotation + MathHelper.PiOver2);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (!impacted)
                return;
            bool slam = Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike;
            Vector2 point = Projectile.Center;
            BluntJavelinVFX.Rubble(point, slam ? 7 : 4, slam ? 4.5f : 3f);
            BluntJavelinVFX.Burst(point, slam ? 12 : 4, slam ? 4f : 2f);
            BluntJavelinVFX.Smoke(point, -Vector2.UnitY, slam ? 80f : 45f, slam ? BluntJavelinVFX.Aqua : new Color(145, 155, 175));
            SoundEngine.PlaySound(SoundID.Tink with { Volume = 0.35f, Pitch = -0.2f, PitchVariance = 0.15f }, point);
            if (slam)
                SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.3f, Pitch = 0.1f }, point);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 3; i++)
                    Projectile.NewProjectile(Projectile.GetSource_Death(), point + Projectile.rotation.ToRotationVector2() * (i * 9f - 9f), Main.rand.NextVector2Circular(2.5f, 2.5f) - Vector2.UnitY * 2f,
                        ModContent.ProjectileType<BluntJavelinFragment>(), 0, 0f, Projectile.owner, Projectile.rotation, i);
                if (slam)
                    Projectile.NewProjectile(Projectile.GetSource_Death(), point, Vector2.Zero, ModContent.ProjectileType<BluntJavelinImpact>(), 0, 0f, Projectile.owner, 0.9f, Projectile.rotation);
            }
        }
    }

    public class BluntJavelinFragment : ModProjectile
    {
        public override string Texture => BluntJavelinArt.ShaftTexture;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 4;
            Projectile.timeLeft = 28;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Projectile.velocity.Y = Math.Min(10f, Projectile.velocity.Y + 0.24f);
            Projectile.ai[0] += Projectile.velocity.X * 0.05f;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y != Projectile.velocity.Y)
                Projectile.velocity = new Vector2(Projectile.velocity.X * 0.6f, -oldVelocity.Y * 0.3f);
            if (oldVelocity.X != Projectile.velocity.X && Projectile.velocity.X == 0f)
                Projectile.velocity.X = -oldVelocity.X * 0.3f;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D shaft = ModContent.Request<Texture2D>(Texture).Value;
            int segment = Math.Clamp((int)Projectile.ai[1], 0, 2);
            int top = segment * shaft.Height / 3;
            Rectangle frame = new(0, top, shaft.Width, (segment + 1) * shaft.Height / 3 - top);
            Main.EntitySpriteDraw(shaft, Projectile.Center - Main.screenPosition, frame, lightColor * Math.Min(1f, Projectile.timeLeft / 14f),
                Projectile.ai[0] + MathHelper.PiOver4, frame.Size() * 0.5f, 1f, SpriteEffects.None);
            return false;
        }
    }

    internal static class BluntJavelinWeaponMotion
    {
        internal static bool JavelinCanSlam(float x, float y) => y > 5f && y > Math.Abs(x) * 1.5f;
        internal static float JavelinGravity(float y) => Math.Min(16f, y + 0.32f);
    }

    public class BluntJavelinImpact : ModProjectile
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
            BluntJavelinVFX.Glow(Projectile.Center, new Vector2(size * 1.5f), BluntJavelinVFX.Violet, fade * 0.5f);
            BluntJavelinVFX.Ring(Projectile.Center, new Vector2(size, size * 0.7f), BluntJavelinVFX.Aqua, fade * 0.65f, Projectile.ai[1]);
            BluntJavelinVFX.Sprite(Texture, Projectile.Center, new Vector2(size * 0.6f), BluntJavelinVFX.Additive(BluntJavelinVFX.Aqua, fade * 0.4f), Projectile.identity * 2.3f);
            BluntJavelinVFX.Flare(Projectile.Center, size * 1.3f, Math.Max(0f, 1f - progress * 3f), Projectile.ai[1]);
            return false;
        }
    }

    public class BluntJavelinDebris : ModDust
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
            Main.EntitySpriteDraw(glow, position, dust.frame, BluntJavelinVFX.Additive(BluntJavelinVFX.Aqua, opacity * 0.7f), dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return false;
        }
    }

    internal static class BluntJavelinVFX
    {
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
                Dust.NewDustPerfect(center, ModContent.DustType<BluntJavelinDebris>(), new Vector2(Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed * 1.3f, -1f)), Scale: Main.rand.NextFloat(0.35f, 0.75f));
        }
    }
}
