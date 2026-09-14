using System;
using System.Collections.Generic;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Glimmerwood;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns
{
    public class GlimmerwoodSword : TranslatableModItem
    {
        internal const string Sprite = "AerovelenceMod/Content/Items/Weapons/CrystalCaverns/GlimmerwoodSword/GlimmerwoodSword";
        private const string Description = "A simple wooden blade with a glimmer in its grain";
        private bool reverseSwing;
        public override string Texture => Sprite;

        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Glimmerwood Sword", Description);
            base.SetStaticDefaults();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.RemoveAll(line => line.Mod == "Terraria" && line.Name.StartsWith("Tooltip"));
            tooltips.Add(new TooltipLine(Mod, "Tooltip0", Description));
            base.ModifyTooltips(tooltips);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = Item.height = 32;
            Item.damage = 9;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 4f;
            Item.useTime = Item.useAnimation = 24;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = Item.noUseGraphic = Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GlimmerwoodSwordSwing>();
            Item.shootSpeed = 1f;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(copper: 30);
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            reverseSwing = !reverseSwing;
            Projectile.NewProjectile(source, player.MountedCenter, velocity.SafeNormalize(Vector2.UnitX * player.direction), type,
                damage, knockback, player.whoAmI, reverseSwing ? 1f : -1f, Math.Max(10, player.itemAnimationMax));
            return false;
        }

        public override void AddRecipes() => CreateRecipe().AddIngredient<GlimmerwoodItem>(7).AddTile(TileID.WorkBenches).Register();
    }

    public class GlimmerwoodSwordSwing : ModProjectile
    {
        private readonly BaseTrailInfo ribbon = new();
        private readonly BaseTrailInfo edge = new();
        private Vector2 previousTip;
        private float impactGlow;
        private float angle;
        private bool initialized;
        private Player Owner => Main.player[Projectile.owner];
        private Vector2 Hand => Owner.RotatedRelativePoint(Owner.MountedCenter);
        private float Scale => Owner.GetAdjustedItemScale(Owner.HeldItem);
        private float Progress => Projectile.ai[2] / Math.Max(10f, Projectile.ai[1]);
        private Vector2 Tip => Hand + angle.ToRotationVector2() * 42f * Scale;
        public override string Texture => GlimmerwoodSword.Sprite;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 180;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => Progress >= 0.22f && Progress < 0.85f ? null : false;

        internal static float SwingOffset(float progress)
        {
            if (progress < 0.25f)
                return MathHelper.SmoothStep(-1.7f, -1.95f, Math.Clamp(progress / 0.25f, 0f, 1f));
            if (progress < 0.8f)
                return MathHelper.SmoothStep(-1.95f, 1.6f, (progress - 0.25f) / 0.55f);
            return MathHelper.SmoothStep(1.6f, 1.7f, Math.Clamp((progress - 0.8f) / 0.2f, 0f, 1f));
        }

        public override void AI()
        {
            Player player = Owner;
            if (!player.active || player.dead || player.noItems || player.CCed || player.HeldItem.type != ModContent.ItemType<GlimmerwoodSword>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 2;
            int facing = Projectile.velocity.X < 0f ? -1 : 1;
            float swingDirection = Projectile.ai[0] * facing * player.gravDir;
            previousTip = initialized ? Tip : Hand;
            float oldProgress = Progress;
            Projectile.ai[2] += 1f / (Projectile.extraUpdates + 1f);
            angle = Projectile.velocity.ToRotation() + SwingOffset(Progress) * swingDirection;
            Projectile.rotation = angle;
            Projectile.Center = Hand + angle.ToRotationVector2() * 26f * Scale;
            if (!initialized)
            {
                initialized = true;
                previousTip = Tip;
            }
            player.ChangeDir(facing);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, angle - MathHelper.PiOver2);
            impactGlow *= 0.86f;
            if (oldProgress < 0.28f && Progress >= 0.28f)
                SoundEngine.PlaySound(SoundID.Item1 with { Volume = 0.45f, Pitch = 0.25f }, Hand);
            if (!Main.dedServ)
            {
                Vector2 point = Tip - player.Center;
                UpdateTrail(ribbon, point, swingDirection);
                UpdateTrail(edge, point, swingDirection);
                if (Progress is > 0.3f and < 0.75f && Projectile.numUpdates == 0 && (int)Projectile.ai[2] % 5 == 0)
                    Spark(Tip, -angle.ToRotationVector2() * 0.4f);
                Lighting.AddLight(Projectile.Center, new Vector3(0.025f, 0.06f, 0.08f));
            }
            if (Progress >= 1f)
                Projectile.Kill();
        }

        private void UpdateTrail(BaseTrailInfo trail, Vector2 point, float direction)
        {
            trail.relativeToPlayer = true;
            trail.myPlayer = Owner;
            trail.trailPointLimit = 20;
            trail.trailMaxLength = 65f * Scale;
            trail.pinch = true;
            trail.pinchAmount = 0.8f;
            trail.trailPos = point;
            trail.trailRot = angle + MathHelper.PiOver2 * direction;
            trail.TrailLogic();
        }

        private static void Spark(Vector2 point, Vector2 velocity)
        {
            if (Main.dedServ)
                return;
            Dust spark = Dust.NewDustPerfect(point, ModContent.DustType<GlowPixelCross>(), velocity, newColor: new Color(125, 220, 255), Scale: 0.12f);
            spark.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.04f, preSlowPower: 0.9f, timeBeforeSlow: 2,
                postSlowPower: 0.8f, velToBeginShrink: 1f, fadePower: 0.85f, shouldFadeColor: false);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            Vector2 start = Hand + angle.ToRotationVector2() * 8f * Scale;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, Tip, 9f * Scale, ref point)
                || Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), previousTip, Tip, 7f * Scale, ref point);
        }

        public override void CutTiles()
        {
            if (CanDamage() == false)
                return;
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(Hand, Tip, 7f * Scale, DelegateMethods.CutTiles);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            impactGlow = 1f;
            Vector2 point = Vector2.Clamp(Tip, target.Hitbox.TopLeft(), target.Hitbox.BottomRight());
            for (int i = 0; i < 3; i++)
                Spark(point, Main.rand.NextVector2Circular(1.5f, 1.5f));
        }

        public override void OnKill(int timeLeft)
        {
            if (Owner.heldProj == Projectile.whoAmI && Owner.HeldItem.type == ModContent.ItemType<GlimmerwoodSword>())
                Owner.itemTime = Owner.itemAnimation = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (!initialized)
                return false;
            float fade = Math.Clamp((1f - Progress) / 0.15f, 0f, 1f);
            float trailFade = Math.Clamp((Progress - 0.23f) / 0.2f, 0f, 1f) * fade;
            if (ribbon.trailPositions?.Count > 2 && trailFade > 0f)
            {
                ribbon.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
                ribbon.trailWidth = (int)(11f * Scale);
                ribbon.trailColor = new Color(85, 180, 245) * (0.4f * trailFade);
                ribbon.trailTime = Main.GlobalTimeWrappedHourly * 0.4f;
                ribbon.TrailDrawing(Main.spriteBatch);
                edge.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Laser1").Value;
                edge.trailWidth = 2;
                edge.trailColor = new Color(190, 240, 255) * (0.22f * trailFade);
                edge.trailTime = Main.GlobalTimeWrappedHourly * 0.2f;
                edge.TrailDrawing(Main.spriteBatch);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            }
            Texture2D sword = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 grip = new(5f, 27f);
            Vector2 position = Hand + angle.ToRotationVector2() * 8f * Scale - Main.screenPosition;
            float rotation = angle + MathHelper.PiOver4;
            Main.EntitySpriteDraw(sword, position, null, lightColor * fade, rotation, grip, Scale, SpriteEffects.None);
            Main.EntitySpriteDraw(sword, position, null, new Color(170, 225, 255, 0) * (impactGlow * fade * 0.4f), rotation, grip, Scale, SpriteEffects.None);
            return false;
        }
    }
}
