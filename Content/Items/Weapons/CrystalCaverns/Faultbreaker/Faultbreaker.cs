using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles;
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
    public class Faultbreaker : TranslatableModItem
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/Faultbreaker/Faultbreaker";
        private const string EnglishTooltip = "A weighty sledgehammer with a crushing overhead swing";
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Faultbreaker", EnglishTooltip)
                .AddSkillStrike(Language.Default, "Skill Strikes against Crystal Caverns enemies (this is sort of boring)")
                .AddName(Language.Spanish, "Rompefallas");
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
            Item.width = Item.height = 32;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 60);
            Item.damage = 24;
            Item.DamageType = DamageClass.Melee;
            Item.useTime = Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<FaultbreakerSwing>();
            Item.shootSpeed = 1f;
            Item.knockBack = 8f;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int direction = velocity.X < 0f ? -1 : 1;
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(direction, 0f), type, damage, knockback, player.whoAmI, 0f, Math.Max(12, player.itemAnimationMax));
            return false;
        }
        public override void AddRecipes() => CreateRecipe().AddIngredient<CavernStoneItem>(40).AddIngredient<CavernCrystalItem>(8).AddRecipeGroup(RecipeGroupID.IronBar, 6).AddTile(TileID.Anvils).Register();
    }

    public class FaultbreakerSwing : ModProjectile
    {
        private readonly BaseTrailInfo ribbon = new();
        private readonly BaseTrailInfo edge = new();
        private float trailOpacity;
        private float angle;
        private float previousAngle;
        private bool impactPauseUsed;
        private bool terrainImpact;
        private float impactGlow;
        private int Direction => Projectile.velocity.X < 0f ? -1 : 1;
        private float Duration => Math.Max(12f, Projectile.ai[1]);
        private float Progress => Projectile.ai[0] / Duration;
        private Vector2 Hand => Main.player[Projectile.owner].RotatedRelativePoint(Main.player[Projectile.owner].MountedCenter);
        private float Reach => 52f * Main.player[Projectile.owner].GetAdjustedItemScale(Main.player[Projectile.owner].HeldItem);
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/Faultbreaker/Faultbreaker";
        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 100;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Projectile.ai[2] <= 0f && FaultbreakerWeaponMotion.HammerCanHit(Progress) ? null : false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.noItems || player.CCed || player.HeldItem.type != ModContent.ItemType<Faultbreaker>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 180;
            player.ChangeDir(Direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            impactGlow *= 0.84f;
            if (Projectile.ai[0] == 0f)
                angle = FaultbreakerWeaponMotion.HammerAngle(0f, Direction);
            previousAngle = angle;
            float previousProgress = Progress;
            if (Projectile.ai[2] > 0f)
                Projectile.ai[2]--;
            else
                Projectile.ai[0]++;
            angle = FaultbreakerWeaponMotion.HammerAngle(Progress, Direction);
            Projectile.Center = Hand + angle.ToRotationVector2() * Reach;
            Projectile.rotation = angle;
            if (!Main.dedServ && Progress >= 0.3f)
            {
                bool moving = Progress < 0.72f && (angle - previousAngle) * Direction > 0.002f;
                trailOpacity = moving ? MathHelper.Lerp(trailOpacity, 1f, 0.5f) : trailOpacity * 0.7f;
                if (moving)
                {
                    for (int sample = 1; sample <= 4; sample++)
                    {
                        float sampleAngle = MathHelper.Lerp(previousAngle, angle, sample / 4f);
                        Vector2 point = Hand + sampleAngle.ToRotationVector2() * Reach - player.Center;
                        UpdateTrail(ribbon, point, sampleAngle);
                        UpdateTrail(edge, point, sampleAngle);
                    }
                }
                else
                {
                    TrimTrail(ribbon);
                    TrimTrail(edge);
                }
            }
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, angle - MathHelper.PiOver2);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.ThreeQuarters, angle - MathHelper.PiOver2 + Direction * 0.2f);
            if (previousProgress < 0.34f && Progress >= 0.34f)
                SoundEngine.PlaySound(SoundID.Item1 with { Volume = 0.65f, Pitch = -0.5f }, Hand);
            if (Progress is > 0.38f and < 0.72f && Projectile.ai[2] <= 0f)
            {
                Vector2 tangent = angle.ToRotationVector2().RotatedBy(Direction * MathHelper.PiOver2);
                FaultbreakerVFX.Spark(Projectile.Center, tangent * 1.4f, 0.15f);
                if ((int)Projectile.ai[0] % 3 == 0)
                    FaultbreakerVFX.Smoke(Projectile.Center, -tangent, 40f, FaultbreakerVFX.Violet);
            }
            if (!terrainImpact && Progress >= 0.65f && Collision.SolidCollision(Projectile.Center - new Vector2(10f), 20, 20))
            {
                terrainImpact = true;
                FaultbreakerVFX.Rubble(Projectile.Center - new Vector2(0f, 8f), 5, 3f);
                FaultbreakerVFX.Smoke(Projectile.Center, -Vector2.UnitY, 70f, new Color(135, 150, 175));
                SoundEngine.PlaySound(SoundID.Tink with { Volume = 0.35f, Pitch = -0.4f }, Projectile.Center);
            }
            if (Progress >= 1f)
            {
                player.itemTime = player.itemAnimation = 0;
                Projectile.Kill();
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float hit = 0f;
            float reach = Reach;
            for (int i = 0; i <= 5; i++)
            {
                Vector2 axis = MathHelper.Lerp(previousAngle, angle, i / 5f).ToRotationVector2();
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Hand + axis * (reach - 16f), Hand + axis * (reach + 12f), 26f, ref hit))
                    return true;
            }
            return false;
        }
        private void UpdateTrail(BaseTrailInfo trail, Vector2 point, float sampleAngle)
        {
            trail.relativeToPlayer = true;
            trail.myPlayer = Main.player[Projectile.owner];
            trail.trailPointLimit = 28;
            trail.trailMaxLength = 90f * Reach / 52f;
            trail.pinch = true;
            trail.pinchAmount = 0.85f;
            trail.trailPos = point;
            trail.trailRot = sampleAngle + Direction * MathHelper.PiOver2;
            trail.TrailLogic();
        }
        private static void TrimTrail(BaseTrailInfo trail)
        {
            if (trail.trailPositions == null)
                return;
            int count = Math.Min(4, trail.trailPositions.Count);
            trail.trailPositions.RemoveRange(0, count);
            trail.trailRotations.RemoveRange(0, count);
            trail.trailCurrentLength = trail.CalculateLength();
        }
        private static bool IsCrystal(NPC target) => target.ModNPC is
            global::AerovelenceMod.Content.NPCs.CrystalCaverns.CrystalSlime
            or global::AerovelenceMod.Content.NPCs.CrystalCaverns.CrystalBat
            or global::AerovelenceMod.Content.NPCs.CrystalCaverns.TumblerockSmall
            or global::AerovelenceMod.Content.NPCs.CrystalCaverns.TumblerockMedium
            or global::AerovelenceMod.Content.NPCs.Bosses.CrystalTumbler.CrystalTumbler;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;
            if (IsCrystal(target))
                SkillStrikeUtil.setSkillStrike(Projectile, 1.5f, 1, 0.4f, 0.7f);
            modifiers.HitDirectionOverride = Direction;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            bool crystal = IsCrystal(target);
            Vector2 point = Vector2.Clamp(Projectile.Center, target.Hitbox.TopLeft(), target.Hitbox.BottomRight());
            impactGlow = 1f;
            if (!impactPauseUsed)
            {
                impactPauseUsed = true;
                Projectile.ai[2] = crystal ? 5f : 3f;
                Projectile.netUpdate = true;
                if (Projectile.owner == Main.myPlayer)
                    Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = Math.Max(Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower, crystal ? 3f : 1.5f);
            }
            FaultbreakerVFX.Burst(point, crystal ? 18 : 9, crystal ? 5f : 3.5f);
            FaultbreakerVFX.Rubble(point, crystal ? 9 : 5, 4f);
            FaultbreakerVFX.Smoke(point, -Vector2.UnitY * 1.5f, 95f, FaultbreakerVFX.Violet);
            SoundEngine.PlaySound(crystal ? SoundID.Shatter with { Volume = 0.5f, Pitch = -0.2f } : SoundID.Item37 with { Volume = 0.45f, Pitch = -0.35f }, point);
            if (Projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), point, Vector2.Zero, ModContent.ProjectileType<FaultbreakerImpact>(), 0, 0f, Projectile.owner, crystal ? 1.35f : 0.85f, angle);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] <= 0f)
                return false;
            float size = Reach / 52f;
            float sweep = MathHelper.Clamp((Progress - 0.32f) / 0.15f, 0f, 1f) * MathHelper.Clamp((0.86f - Progress) / 0.15f, 0f, 1f);
            if (ribbon.trailPositions?.Count > 2 && ribbon.trailCurrentLength > 0.5f && sweep * trailOpacity > 0.01f)
            {
                float fade = sweep * trailOpacity;
                ribbon.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
                ribbon.trailWidth = (int)(17f * size);
                ribbon.trailColor = Color.Lerp(FaultbreakerVFX.Violet, FaultbreakerVFX.Aqua, 0.55f) * (fade * 0.5f);
                ribbon.trailTime = Main.GlobalTimeWrappedHourly * 0.3f;
                ribbon.TrailDrawing(Main.spriteBatch);
                edge.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Laser1").Value;
                edge.trailWidth = Math.Max(1, (int)(3f * size));
                edge.trailColor = new Color(195, 240, 255) * (fade * 0.3f);
                edge.trailTime = Main.GlobalTimeWrappedHourly * 0.15f;
                edge.TrailDrawing(Main.spriteBatch);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            Texture2D hammer = TextureAssets.Projectile[Type].Value;
            Vector2 grip = new(5f, 55f);
            Vector2 head = new(39f, 23f);
            float drawScale = Reach / Vector2.Distance(grip, head);
            float rotation = angle - (head - grip).ToRotation();
            Main.EntitySpriteDraw(hammer, Hand - Main.screenPosition, null, lightColor, rotation, grip, drawScale, SpriteEffects.None);
            Main.EntitySpriteDraw(hammer, Hand - Main.screenPosition, null, FaultbreakerVFX.Additive(Color.White, impactGlow * 0.55f), rotation, grip, drawScale, SpriteEffects.None);
            FaultbreakerVFX.Glow(Projectile.Center, new Vector2(55f * size), FaultbreakerVFX.Aqua, 0.12f + impactGlow * 0.6f);
            return false;
        }
    }

    internal static class FaultbreakerWeaponMotion
    {
        internal static float HammerAngle(float progress, int direction)
        {
            float offset;
            if (progress < 0.3f)
            {
                float windup = Math.Clamp(progress / 0.3f, 0f, 1f);
                windup = windup * windup * (3f - 2f * windup);
                offset = -0.65f - windup * 0.65f;
            }
            else if (progress < 0.72f)
            {
                float swing = (progress - 0.3f) / 0.42f;
                swing = swing * swing * (3f - 2f * swing);
                offset = -1.3f + swing * 3.8f;
            }
            else
            {
                float recover = Math.Clamp((progress - 0.72f) / 0.28f, 0f, 1f);
                recover = recover * recover * (3f - 2f * recover);
                offset = 2.5f - recover * 0.22f;
            }
            return -MathF.PI * 0.5f + direction * offset;
        }

        internal static bool HammerCanHit(float progress) => progress >= 0.34f && progress <= 0.74f;
    }

    public class FaultbreakerImpact : ModProjectile
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
            FaultbreakerVFX.Glow(Projectile.Center, new Vector2(size * 1.5f), FaultbreakerVFX.Violet, fade * 0.5f);
            FaultbreakerVFX.Ring(Projectile.Center, new Vector2(size, size * 0.7f), FaultbreakerVFX.Aqua, fade * 0.65f, Projectile.ai[1]);
            FaultbreakerVFX.Sprite(Texture, Projectile.Center, new Vector2(size * 0.6f), FaultbreakerVFX.Additive(FaultbreakerVFX.Aqua, fade * 0.4f), Projectile.identity * 2.3f);
            FaultbreakerVFX.Flare(Projectile.Center, size * 1.3f, Math.Max(0f, 1f - progress * 3f), Projectile.ai[1]);
            return false;
        }
    }

    public class FaultbreakerDebris : ModDust
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
            Main.EntitySpriteDraw(glow, position, dust.frame, FaultbreakerVFX.Additive(FaultbreakerVFX.Aqua, opacity * 0.7f), dust.rotation, dust.frame.Size() * 0.5f, dust.scale, SpriteEffects.None);
            return false;
        }
    }

    internal static class FaultbreakerVFX
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
                Dust.NewDustPerfect(center, ModContent.DustType<FaultbreakerDebris>(), new Vector2(Main.rand.NextFloat(-speed, speed), Main.rand.NextFloat(-speed * 1.3f, -1f)), Scale: Main.rand.NextFloat(0.35f, 0.75f));
        }
    }
}
