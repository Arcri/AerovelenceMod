using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.GameContent;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Melee.Spears
{
    public class GoldHalberd : ModItem
    {
        public override void SetStaticDefaults() { CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1; }

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.rare = ItemRarities.EarlyPHM;
            Item.value = Item.sellPrice(silver: 50);

            Item.damage = 12;
            Item.knockBack = 3f;
            Item.crit = 4;
            Item.DamageType = DamageClass.Melee;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shootSpeed = 3.5f;
            Item.shoot = ModContent.ProjectileType<GoldHalberdStabProjectile>();
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<GoldHalberdStabProjectile>()] > 0)
                    return false;
                Item.useTime = 28;
                Item.useAnimation = 28;
                Item.shootSpeed = 3.7f;
                Item.shoot = ModContent.ProjectileType<GoldHalberdStabProjectile>();
                Item.UseSound = SoundID.Item71;
            }
            else
            {
                if (player.ownedProjectileCounts[ModContent.ProjectileType<GoldHalberdSwingProjectile>()] > 0)
                    return false;
                Item.useTime = 45;
                Item.useAnimation = 45;
                Item.shootSpeed = 1f;
                Item.shoot = ModContent.ProjectileType<GoldHalberdSwingProjectile>();
            }

            return true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
            else
            {
                bool alternateSwing = player.GetModPlayer<GoldHalberdPlayer>().alternateSwing;
                int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, alternateSwing ? 1 : 0);
                player.GetModPlayer<GoldHalberdPlayer>().alternateSwing = !player.GetModPlayer<GoldHalberdPlayer>().alternateSwing;
            }
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.GoldBar, 14);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class GoldHalberdPlayer : ModPlayer { public bool alternateSwing = false; }

    public class GoldHalberdSwingProjectile : BaseSwingSwordProj
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/Misc/Melee/Spears/GoldHalberdProjectileSwing";
        private bool playedSound = false;
        private readonly int[] hitCooldowns = new int[Main.maxNPCs];
        private List<AfterimageData> afterimages = new List<AfterimageData>();
        private const int MAX_AFTERIMAGES = 18;
        private Vector2 lastPosition;
        private float lastRotation;

        public override void SetDefaults()
        {
            Projectile.timeLeft = 10000;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 62;
            Projectile.height = 62;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            if (timer == 0)
                Projectile.spriteDirection = Main.MouseWorld.X > Main.player[Projectile.owner].MountedCenter.X ? 1 : -1;
            SwingHalfAngle = 75;
            easingAdditionAmount = 0.015f / Projectile.extraUpdates;
            offset = 60;
            frameToStartSwing = 1;
            timeAfterEnd = 2;
            startingProgress = 0.01f;
            StandardSwingUpdate();
            StandardHeldProjCode();

            for (int i = 0; i < hitCooldowns.Length; i++)
            {
                if (hitCooldowns[i] > 0)
                    hitCooldowns[i]--;
            }

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Sharp_M_a") with { Pitch = -.4f, PitchVariance = .3f, Volume = 0.25f };
                SoundEngine.PlaySound(style, Projectile.Center);
                playedSound = true;
            }
            float intensity = (float)Math.Sin(getProgress(easingProgress) * Math.PI);
            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle);
            Vector2 positionOffset = new Vector2(Projectile.spriteDirection > 0 ? 4 : 0, Projectile.spriteDirection > 0 ? -8 : -12).RotatedBy(currentAngle);
            Vector2 currentPosition = armPosition + positionOffset;
            if (getProgress(easingProgress) >= 0.1f && getProgress(easingProgress) <= 0.9f && timer % 2 == 0)
            {
                if (timer > 0 && (Vector2.Distance(currentPosition, lastPosition) > 5f || Math.Abs(Projectile.rotation - lastRotation) > 0.1f))
                {
                    Vector2 gfxOffset = new(0, -Main.player[Projectile.owner].gfxOffY);
                    afterimages.Add(new AfterimageData
                    {
                        Position = currentPosition - gfxOffset,
                        Rotation = Projectile.rotation,
                        Scale = Projectile.scale + (intensity * 0.3f),
                        Opacity = intensity * 0.8f,
                        TimeLeft = 10,
                        SpriteDirection = Projectile.spriteDirection,
                        Color = new Color(255, 50, 100) * intensity
                    });
                    if (afterimages.Count > MAX_AFTERIMAGES)
                        afterimages.RemoveAt(0);
                }
            }
            lastPosition = currentPosition;
            lastRotation = Projectile.rotation;
            for (int i = afterimages.Count - 1; i >= 0; i--)
            {
                afterimages[i].TimeLeft--;
                if (afterimages[i].TimeLeft <= 0)
                {
                    afterimages.RemoveAt(i);
                    continue;
                }
                afterimages[i].Opacity *= 0.85f;
                float shakeAmount = 3f * (1f - afterimages[i].TimeLeft / 10f);
                afterimages[i].Position += Main.rand.NextVector2Circular(shakeAmount, shakeAmount);
                afterimages[i].Rotation += Main.rand.NextFloat(-0.05f, 0.05f) * (1f - afterimages[i].TimeLeft / 10f);
                afterimages[i].Scale += Main.rand.NextFloat(-0.02f, 0.02f);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (hitCooldowns[target.whoAmI] > 0)
                return false;

            return base.CanHitNPC(target);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitCooldowns[target.whoAmI] = 20;
            SoundEngine.PlaySound(SoundID.Item71.WithPitchOffset(0.2f), target.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D spearTexture = ModContent.Request<Texture2D>(Texture).Value;
            float rotationOffset = 0;
            Vector2 origin;
            SpriteEffects effects;
            if (afterimages.Count > 0)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                foreach (var afterimage in afterimages)
                {
                    if (afterimage.SpriteDirection > 0)
                    {
                        origin = new Vector2(10, spearTexture.Height - 5);
                        rotationOffset = 0;
                        effects = SpriteEffects.None;
                    }
                    else
                    {
                        origin = new Vector2(spearTexture.Width - 10, spearTexture.Height - 5);
                        rotationOffset = MathHelper.ToRadians(90f);
                        effects = SpriteEffects.FlipHorizontally;
                    }
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }

            if (Projectile.ai[0] != 1)
            {
                origin = new Vector2(10, spearTexture.Height - 5);
                rotationOffset = 0;
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(spearTexture.Width - 10, spearTexture.Height - 5);
                rotationOffset = MathHelper.ToRadians(90f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle);
            Vector2 positionOffset = new Vector2(Projectile.spriteDirection > 0 ? 4 : 0, Projectile.spriteDirection > 0 ? -8 : -12).RotatedBy(currentAngle);
            Vector2 gfxOffset = new(0, -Main.player[Projectile.owner].gfxOffY);
            float intensity = (float)Math.Sin(getProgress(easingProgress) * Math.PI);
            Main.spriteBatch.Draw(spearTexture, armPosition - Main.screenPosition + positionOffset - gfxOffset, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + (intensity * 0.3f), effects, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //This literally has to be here otherwise the player's arm glows
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override float getProgress(float x) { return Easings.easeInOutQuint(x); }

        private class AfterimageData
        {
            public Vector2 Position;
            public float Rotation;
            public float Scale;
            public float Opacity;
            public int TimeLeft;
            public int SpriteDirection;
            public Color Color;
        }
    }

    public class GoldHalberdStabProjectile : ModProjectile
    {
        public override string Texture => "AerovelenceMod/Content/Items/Weapons/Misc/Melee/Spears/GoldHalberdProjectileStab";

        protected virtual float HoldoutRangeMin => 10f;
        protected virtual float HoldoutRangeMax => 60f;

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Spear);
            Projectile.width = 55;
            Projectile.height = 55;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];
            int duration = player.itemAnimationMax;
            player.heldProj = Projectile.whoAmI;
            if (Projectile.timeLeft > duration)
                Projectile.timeLeft = duration;
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);

            float halfDuration = duration * 0.5f;
            float progress;
            if (Projectile.timeLeft < halfDuration)
                progress = Projectile.timeLeft / halfDuration;
            else
                progress = (duration - Projectile.timeLeft) / halfDuration;
            Vector2 perpendicularOffset = new Vector2(0, 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2);
            float angleDifference = Math.Abs(Vector2.Dot(Vector2.UnitX, Projectile.velocity.SafeNormalize(Vector2.Zero)));
            perpendicularOffset *= MathHelper.Lerp(0.5f, 1f, angleDifference);

            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(
                Projectile.velocity * HoldoutRangeMin,
                Projectile.velocity * HoldoutRangeMax,
                progress) + perpendicularOffset;
            if (Projectile.spriteDirection == -1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D spearTexture = ModContent.Request<Texture2D>(Texture).Value;
            Player player = Main.player[Projectile.owner];
            Vector2 position = Projectile.Center - Main.screenPosition;
            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Vector2 origin = new Vector2(30, spearTexture.Height / 2);
            if (Projectile.spriteDirection == -1)
                origin.X = spearTexture.Width;
            Main.spriteBatch.Draw(spearTexture, position, null, lightColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            
            SoundEngine.PlaySound(SoundID.Item71.WithPitchOffset(0.3f), target.Center);
        }

    }
    
}