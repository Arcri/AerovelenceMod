using System;
using System.Collections.Generic;
using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Ammo
{
    public class HellfireBullet : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            this.ModifyLocalization("Hellfire Bullet", "Pierces one enemy and sets them on fire\nFiring creates a short-lived superheated flare behind the bullet")
			            .AddName(Language.Default, "Hellfire Bullet").AddTooltip(Language.Default, "Pierces one enemy and sets them on fire\nFiring creates a short-lived superheated flare behind the bullet");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 8;
            Item.height = 8;
            Item.damage = 6;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 1f;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.ammo = AmmoID.Bullet;
            Item.shoot = ModContent.ProjectileType<HellfireBulletProjectile>();
            Item.shootSpeed = 11f;
            Item.value = Item.sellPrice(copper: 1);
            Item.rare = ItemRarityID.Orange;
        }

        public override void AddRecipes()
        {
            CreateRecipe(100)
                .AddIngredient(ItemID.MusketBall, 100)
                .AddIngredient(ItemID.Hellstone, 1)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }

    public class HellfireBulletProjectile : ModProjectile
    {
        private const int AmmoBaseDamage = 6;

        public override string Texture => "Terraria/Images/Projectile_14";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            if (Projectile.owner != Main.myPlayer)
                return;

            Player owner = Main.player[Projectile.owner];
            Item weapon = owner.HeldItem;
            int gunDamage = weapon != null && !weapon.IsAir && weapon.useAmmo == AmmoID.Bullet
                ? owner.GetWeaponDamage(weapon)
                : Math.Max(AmmoBaseDamage, Projectile.damage - AmmoBaseDamage);
            int flareDamage = Math.Max(1, (AmmoBaseDamage + gunDamage) / 2);

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Projectile.velocity,
                ModContent.ProjectileType<HellfireBulletFlare>(),
                flareDamage,
                Projectile.knockBack * 0.35f,
                Projectile.owner);
        }

        public override void AI()
        {
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 3f,
                    DustID.Torch,
                    -Projectile.velocity * 0.08f + Main.rand.NextVector2Circular(0.35f, 0.35f),
                    80,
                    new Color(255, 90, 25),
                    Main.rand.NextFloat(0.55f, 0.8f));
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 4; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    ModContent.DustType<GlowPixelCross>(),
                    Main.rand.NextVector2Circular(1.5f, 1.5f),
                    0,
                    Main.rand.NextBool() ? new Color(255, 75, 20) : new Color(255, 155, 55),
                    Main.rand.NextFloat(0.14f, 0.24f));
                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.12f, timeBeforeSlow: 4, preSlowPower: 0.93f, postSlowPower: 0.85f, velToBeginShrink: 1f, fadePower: 0.86f, shouldFadeColor: false);
            }
        }
    }

    public class HellfireBulletFlare : TrailProjBase
    {
        private const int ActiveTicks = 10;
        private const int Lifetime = 22;
        private readonly List<Vector2> collisionPoints = new();
        private int timer;
        private bool stopped;

        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Lifetime + 4;
        }

        public override bool? CanDamage()
        {
            return timer <= ActiveTicks ? null : false;
        }

        public override void AI()
        {
            float fade = timer <= ActiveTicks
                ? MathHelper.Clamp(timer / 4f, 0f, 1f)
                : MathHelper.Clamp((Lifetime - timer) / (float)(Lifetime - ActiveTicks), 0f, 1f);

            trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value;
            trailColor = Color.Lerp(new Color(255, 35, 10), new Color(255, 130, 35), 0.25f + 0.15f * MathF.Sin(timer * 0.8f)) * fade;
            trailTime = timer * 0.045f;
            trailPointLimit = 90;
            trailWidth = 26;
            trailMaxLength = 145;
            trailRot = Projectile.velocity.LengthSquared() > 0.01f ? Projectile.velocity.ToRotation() : Projectile.rotation;
            trailPos = Projectile.Center;
            TrailLogic();

            if (!stopped)
            {
                collisionPoints.Insert(0, Projectile.Center);
                if (collisionPoints.Count > 20)
                    collisionPoints.RemoveAt(collisionPoints.Count - 1);
                Projectile.rotation = Projectile.velocity.ToRotation();
            }

            if (!Main.dedServ)
            {
                float glow = fade * 0.45f;
                Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.18f, 0.035f) * glow);

                if (timer < ActiveTicks && Main.rand.NextBool(2))
                {
                    Vector2 backwards = -Projectile.velocity.SafeNormalize(Vector2.UnitX);
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center + backwards * Main.rand.NextFloat(2f, 12f) + Main.rand.NextVector2Circular(3f, 3f),
                        ModContent.DustType<GlowPixelCross>(),
                        backwards * Main.rand.NextFloat(0.4f, 1.5f) + Main.rand.NextVector2Circular(0.45f, 0.45f),
                        0,
                        Main.rand.NextBool(3) ? new Color(255, 185, 70) : new Color(255, 55, 15),
                        Main.rand.NextFloat(0.12f, 0.23f));
                    dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.1f, timeBeforeSlow: 4, preSlowPower: 0.94f, postSlowPower: 0.86f, velToBeginShrink: 1f, fadePower: 0.84f, shouldFadeColor: false);
                }
            }

            timer++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (timer > ActiveTicks || collisionPoints.Count < 2)
                return false;

            float collisionPoint = 0f;
            for (int i = 1; i < collisionPoints.Count; i++)
            {
                float progress = i / (float)(collisionPoints.Count - 1);
                float width = MathHelper.Lerp(17f, 8f, progress);
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), collisionPoints[i - 1], collisionPoints[i], width, ref collisionPoint))
                    return true;
            }

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.position -= Projectile.velocity;
            Projectile.velocity = Vector2.Zero;
            Projectile.tileCollide = false;
            stopped = true;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float fade = timer <= ActiveTicks
                ? MathHelper.Clamp(timer / 4f, 0f, 1f)
                : MathHelper.Clamp((Lifetime - timer) / (float)(Lifetime - ActiveTicks), 0f, 1f);

            Texture2D glow = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/SoftGlow64").Value;
            Texture2D streak = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/Starlight").Value;
            Vector2 velocityDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            if (velocityDirection.LengthSquared() < 0.01f && collisionPoints.Count > 1)
                velocityDirection = (collisionPoints[0] - collisionPoints[1]).SafeNormalize(Vector2.UnitX);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition - velocityDirection * 8f;
            float rotation = velocityDirection.ToRotation();
            float pulse = 0.9f + MathF.Sin(timer * 0.7f) * 0.1f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(glow, drawPosition, null, new Color(255, 55, 10, 0) * fade * 0.42f, 0f, glow.Size() * 0.5f, 0.55f * pulse, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(streak, drawPosition, null, new Color(255, 65, 10, 0) * fade * 0.8f, rotation, streak.Size() * 0.5f, new Vector2(0.65f, 0.18f) * pulse, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(streak, drawPosition, null, new Color(255, 225, 160, 0) * fade * 0.75f, rotation, streak.Size() * 0.5f, new Vector2(0.35f, 0.07f) * pulse, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            TrailDrawing();
            return false;
        }

        public override float WidthFunction(float progress)
        {
            float head = Utils.GetLerpValue(0f, 0.16f, progress, true);
            float tail = Utils.GetLerpValue(1f, 0.45f, progress, true);
            float shape = MathF.Sin(MathHelper.Pi * MathHelper.Clamp(progress, 0f, 1f));
            return MathHelper.Lerp(5f, 26f, MathF.Sqrt(Math.Max(0f, shape))) * head * tail;
        }
    }
}
