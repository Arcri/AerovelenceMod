using AerovelenceMod.Common.Systems.Language;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Sets.Phantic.Armor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Sets.Phantic
{
    public class SoulerangPlayer : ModPlayer
    {
        public int soulerangThrowStage = 0;
    }

    public class Soulerang : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            this.ModifyLocalization("Soulerang", "This boomerang has a unique flight pattern, and can go through blocks")
            .AddName(Language.Default, "Soulerang").AddTooltip(Language.Default, "This boomerang has a unique flight pattern, and can go through blocks");

            //.AddName(Language.Spanish, "").AddSkillStrike(Language.Spanish, "")
            //.AddName(Language.French, "").AddSkillStrike(Language.French, "")
            //.AddName(Language.German, "").AddSkillStrike(Language.German, "")
            //.AddName(Language.Italian, "").AddSkillStrike(Language.Italian, "")
            //.AddName(Language.Polish, "").AddSkillStrike(Language.Polish, "")
            //.AddName(Language.PortugueseBrazil, "").AddSkillStrike(Language.PortugueseBrazil, "")
            //.AddName(Language.Russian, "").AddSkillStrike(Language.Russian, "");
            //.AddName(Language.ChineseTraditional, "").AddSkillStrike(Language.ChineseTraditional, "")
            //.AddName(Language.ChineseSimplified, "").AddSkillStrike(Language.ChineseSimplified, "")

        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarities.MidPHM;
            Item.value = Item.sellPrice(silver: 50);

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.autoReuse = true;
            Item.damage = 15;
            Item.knockBack = 4f;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Melee;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ModContent.ProjectileType<SoulerangProjectile>();
            Item.shootSpeed = 10f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<SoulerangProjectile>()] < 1;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var modPlayer = player.GetModPlayer<SoulerangPlayer>();
            modPlayer.soulerangThrowStage = (modPlayer.soulerangThrowStage + 1) % 3;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, modPlayer.soulerangThrowStage);

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 12);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class SoulerangProjectile : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private int ThrowStage => (int)Projectile.ai[1];
        private int PulseCooldown;

        private Vector2 initialVelocity;
        private Vector2 targetPosition;
        private Vector2 initialTargetPosition;
        private int pathTimer;
        private bool initialized;

        private float returnSpeed;
        private float returnAcceleration;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.7f);
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 8;
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 speed = Main.rand.NextVector2Circular(1f, 1.3f);
                Dust gd = Dust.NewDustPerfect(Projectile.Center - new Vector2(5, 5), ModContent.DustType<GlowPixelCross>(), speed, newColor: Color.IndianRed, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                    preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }

            if (!initialized)
            {
                initialVelocity = Projectile.velocity;
                initialTargetPosition = Main.MouseWorld;
                float distanceToMouse = Vector2.Distance(Owner.Center, initialTargetPosition);
                float targetDistance = Math.Max(distanceToMouse, 250f + ThrowStage * 50f);
                Vector2 directionToCursor = (initialTargetPosition - Owner.Center).SafeNormalize(Vector2.UnitX);
                targetPosition = Owner.Center + directionToCursor * targetDistance;
                returnSpeed = 12f + ThrowStage * 2f;
                returnAcceleration = 0.6f + ThrowStage * 0.3f;

                initialized = true;
            }
            if (Projectile.ai[0] == 0f)
            {
                HandleOutwardPhase();
            }
            else
            {
                HandleReturnPhase();
            }
            if (ThrowStage == 2)
            {
                float glowProgress = Math.Min(pathTimer / 30f, 1f);
                Projectile.localAI[0] = glowProgress;
            }
            if (PulseCooldown > 0)
                PulseCooldown--;
            if (ThrowStage == 2 && Projectile.ai[0] == 0f)

                Projectile.rotation += 0.5f * Projectile.direction;
            else
                Projectile.rotation += 0.4f * Projectile.direction;

            if (Vector2.Distance(Owner.Center, Projectile.Center) > 3000f)
                Projectile.Kill();
        }

        private void HandleOutwardPhase()
        {
            pathTimer++;
            Vector2 directionToTarget = (initialTargetPosition - Owner.Center).SafeNormalize(Vector2.UnitX);
            Vector2 perpendicular = new(-directionToTarget.Y, directionToTarget.X);
            float baseSpeed = initialVelocity.Length() * 0.9f;
            Vector2 baseVelocity = directionToTarget * baseSpeed;
            float distanceToTarget = Vector2.Distance(Owner.Center, initialTargetPosition);
            float arcScaleFactor = Math.Min(distanceToTarget / 300f, 1.5f);
            float arcWidthMultiplier = 1.2f;
            float arcWidth = 5f * arcWidthMultiplier;

            switch (ThrowStage)
            {
                case 0:
                    {
                        float arcProgress = pathTimer / 30f;
                        float arcHeight;
                        if (arcProgress < 0.5f)
                            arcHeight = -1f + arcProgress * 2f;
                        else
                            arcHeight = (arcProgress - 0.5f) * 2f;
                        Projectile.velocity = baseVelocity + perpendicular * arcHeight * arcWidth;
                        if (arcProgress > 0.5f)
                        {
                            Vector2 toTarget = targetPosition - Projectile.Center;
                            float distanceRatio = Math.Min(arcProgress - 0.5f, 0.5f) * 2f;
                            Projectile.velocity += toTarget.SafeNormalize(Vector2.Zero) * baseSpeed * 0.05f * distanceRatio;
                        }
                    }
                    break;

                case 1:
                    {
                        float arcProgress = pathTimer / 35f;
                        float arcHeight;
                        if (arcProgress < 0.5f)

                            arcHeight = 1f - arcProgress * 2f;
                        else
                            arcHeight = -1f * (arcProgress - 0.5f) * 2f;
                        Projectile.velocity = baseVelocity + perpendicular * arcHeight * arcWidth;
                        if (arcProgress > 0.5f)
                        {
                            Vector2 toTarget = targetPosition - Projectile.Center;
                            float distanceRatio = Math.Min(arcProgress - 0.5f, 0.5f) * 2f;
                            Projectile.velocity += toTarget.SafeNormalize(Vector2.Zero) * baseSpeed * 0.05f * distanceRatio;
                        }
                    }
                    break;

                case 2:
                    {
                        float arcProgress = pathTimer / 45f;
                        float infinityArcWidth = 15f * arcWidthMultiplier;

                        float arcHeight;
                        if (arcProgress < 0.25f)
                            arcHeight = -1f + arcProgress * 4f;
                        else if (arcProgress < 0.5f)
                            arcHeight = (arcProgress - 0.25f) * 4f;
                        else if (arcProgress < 0.75f)
                            arcHeight = 1f - (arcProgress - 0.5f) * 4f;
                        else
                        {
                            arcHeight = -1f * (arcProgress - 0.75f) * 4f;
                            float extendFactor = (arcProgress - 0.75f) * 4f;
                            Projectile.velocity += directionToTarget * baseSpeed * 0.3f * extendFactor;
                        }
                        Projectile.velocity = baseVelocity + perpendicular * arcHeight * infinityArcWidth;
                        if (arcProgress > 0.5f)
                        {
                            Vector2 toTarget = targetPosition - Projectile.Center;
                            float distanceRatio = Math.Min((arcProgress - 0.5f) * 2f, 1f);
                            Projectile.velocity += toTarget.SafeNormalize(Vector2.Zero) * baseSpeed * 0.04f * distanceRatio;
                        }
                    }
                    break;
            }

            Vector2 targetAttraction = targetPosition - Projectile.Center;
            float targetDistance = targetAttraction.Length();
            if (targetDistance > 50f)
            {
                targetAttraction.Normalize();
                float attractionStrength = ThrowStage == 2 ? 0.15f : 0.25f;
                Projectile.velocity += targetAttraction * attractionStrength;
            }
            bool shouldReturn = false;
            if (ThrowStage == 0 && pathTimer >= 30)
                shouldReturn = true;
            else if (ThrowStage == 1 && pathTimer >= 35)
                shouldReturn = true;
            else if (ThrowStage == 2 && pathTimer >= 45)
                shouldReturn = true;
            if (Vector2.Distance(Projectile.Center, targetPosition) < 30f && pathTimer > 20)
                shouldReturn = true;
            if (Vector2.Distance(Projectile.Center, Owner.Center) > 800f)
                shouldReturn = true;

            if (shouldReturn)
            {
                Projectile.ai[0] = 1f;
                Projectile.netUpdate = true;
                pathTimer = 0;
            }
        }

        private void HandleReturnPhase()
        {
            Projectile.tileCollide = false;

            Vector2 toOwner = Owner.Center - Projectile.Center;
            float distanceToOwner = toOwner.Length();

            if (distanceToOwner > 3000f)
            {
                Projectile.Kill();
                return;
            }
            if (distanceToOwner > 0f)
                toOwner.Normalize();
            Vector2 idealVelocity = toOwner * returnSpeed;
            if (Projectile.velocity.X < idealVelocity.X)
            {
                Projectile.velocity.X += returnAcceleration;
                if (Projectile.velocity.X < 0f && idealVelocity.X > 0f)
                    Projectile.velocity.X += returnAcceleration; //extra correction for direction changes
            }
            else if (Projectile.velocity.X > idealVelocity.X)
            {
                Projectile.velocity.X -= returnAcceleration;
                if (Projectile.velocity.X > 0f && idealVelocity.X < 0f)
                    Projectile.velocity.X -= returnAcceleration; //extra correction for direction changes
            }

            if (Projectile.velocity.Y < idealVelocity.Y)
            {
                Projectile.velocity.Y += returnAcceleration;
                if (Projectile.velocity.Y < 0f && idealVelocity.Y > 0f)
                    Projectile.velocity.Y += returnAcceleration; //extra correction for direction changes
            }
            else if (Projectile.velocity.Y > idealVelocity.Y)
            {
                Projectile.velocity.Y -= returnAcceleration;
                if (Projectile.velocity.Y > 0f && idealVelocity.Y < 0f)
                    Projectile.velocity.Y -= returnAcceleration; //extra correction for direction changes
            }

            if (ThrowStage == 2)
            {
                pathTimer++;
                Vector2 perpendicular = new(-toOwner.Y, toOwner.X);
                Projectile.velocity += perpendicular * (float)Math.Sin(pathTimer * 0.15f) * 0.6f;
            }

            Rectangle projectileHitbox = new((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
            Rectangle playerHitbox = new((int)Owner.position.X, (int)Owner.position.Y, Owner.width, Owner.height);

            if (projectileHitbox.Intersects(playerHitbox))
            {
                Projectile.Kill();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X * 0.6f;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y * 0.6f;

            TriggerPulse();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TriggerPulse();
            if (ThrowStage == 2)
            {
                //idk maybe something
            }
        }

        private void TriggerPulse()
        {
            if (PulseCooldown <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(8f, 8f);
                    int dustType = ThrowStage == 2 ? DustID.PurpleTorch : DustID.WhiteTorch;
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                        dustType, speed.X, speed.Y);
                }
                SoundEngine.PlaySound(SoundID.Item7, Projectile.position);
                PulseCooldown = 10;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Sets/Phantic/SoulerangProjectile_Glow").Value;
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Color color = Projectile.GetAlpha(lightColor);
            if (ThrowStage == 2 || Projectile.ai[0] == 1f)
                DrawTrail(texture, drawOrigin, color);
            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            if (ThrowStage == 2)
            {
                float glowIntensity = Projectile.localAI[0];
                if (PulseCooldown > 0)
                    glowIntensity = 1f;
                Color glowColor = Color.White * glowIntensity;
                glowColor.A = 0;
                Main.EntitySpriteDraw(glowTexture, drawPos, null, glowColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            return false;
        }

        private void DrawTrail(Texture2D texture, Vector2 drawOrigin, Color baseColor)
        {
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    continue;

                float progress = 1f - (float)i / ProjectileID.Sets.TrailCacheLength[Projectile.type];
                Vector2 drawPos = Projectile.oldPos[i] + new Vector2(Projectile.width, Projectile.height) / 2f - Main.screenPosition;
                Color trailColor = baseColor * progress * 0.5f;
                trailColor.A = 0;
                if (ThrowStage == 2)
                    trailColor = Color.Lerp(new Color(180, 80, 255, 0), new Color(100, 20, 255, 0), progress) * progress * 0.6f;
                float rotationValue = Projectile.oldRot[i];
                float scaleValue = Projectile.scale * progress * 0.8f;

                Main.EntitySpriteDraw(texture, drawPos, null, trailColor, rotationValue, drawOrigin, scaleValue, SpriteEffects.None, 0);
            }
        }
    }
}