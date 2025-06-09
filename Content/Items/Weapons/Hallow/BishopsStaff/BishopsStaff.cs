using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;
using System;
using AerovelenceMod.Content.Items.Weapons.Hallow.BishopsStaff;
using Terraria.DataStructures;
using Terraria.Graphics.CameraModifiers;
using AerovelenceMod.Common.Utilities;
using System.Collections.Generic;
using AerovelenceMod.Content.Buffs.FlareDebuffs;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Weapons.Flares;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Common;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using AerovelenceMod.Common.Systems.Language;

namespace AerovelenceMod.Content.Items.Weapons.Hallow.BishopsStaff
{
    public class BishopsStaff : TranslatableModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            this.ModifyLocalization("BishopsStaff", "Hold Left Click to ground the staff and channel mana towards it\nWhile channeling, the staff generates a starry sky above itself\nThe stars fire stellar beams towards the ground and generate faster over time\nAfter some time, larger stars will generate, firing larger beams")
            .AddName(Language.Default, "Bishop's Staff").AddTooltip(Language.Default, "Hold Left Click to ground the staff and channel mana towards it\nWhile channeling, the staff generates a starry sky above itself\nThe stars fire stellar beams towards the ground and generate faster over time\nAfter some time, larger stars will generate, firing larger beams")
            .AddSkillStrike(Language.Default, "Large stars Skill Strike");

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
            Item.width = 40;
            Item.height = 40;
            Item.rare = ItemRarities.PrePlantPostMech;
            Item.value = Item.sellPrice(gold: 5);
            Item.damage = 85;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 8f;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.mana = 16;
            Item.useStyle = ItemUseStyleID.HiddenAnimation;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<BishopsStaffProj>();
            Item.shootSpeed = 1f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.velocity.Y != 0)
                return false;
            if (player.mount.Active)
                player.mount.Dismount(player);

            Projectile.NewProjectile(source, player.Center, Vector2.Zero, type, damage, knockback, player.whoAmI);

            return false;
        }

        private int manaDrainTimer = 0;
        private int currentDrainRate = 20;
        private int drainRateMin = 4;
        private int drainRateMax = 20;
        private float drainAcceleration = 0.1f;

        public override void HoldItem(Player player)
        {
            int manaCost = 5;
            if (player.channel)
            {
                manaDrainTimer++;
                if (currentDrainRate > drainRateMin)
                    currentDrainRate -= (int)drainAcceleration;
                else
                    currentDrainRate = drainRateMin;
                if (manaDrainTimer >= currentDrainRate)
                {
                    manaDrainTimer = 0;

                    if (player.statMana >= manaCost)
                    {
                        player.statMana -= manaCost;
                        player.manaRegenDelay = 60;
                    }
                    else
                        player.channel = false;
                }
            }
            else
            {
                player.channel = false;
                manaDrainTimer = 0;
                currentDrainRate = drainRateMax;
            }
            if (player.statMana <= 5)
                player.channel = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddIngredient(ItemID.FallenStar, 10)
                .AddIngredient(ItemID.SoulofMight, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class BishopsStaffProj : ModProjectile
    {
        private enum AttackState
        {
            Raising,    //Initial raising of the staff
            Slamming,   //Slamming the staff down
            Impact,     //Impact
            Finished    //Cleanup and stuff
        }
        private AttackState _currentState = AttackState.Raising;
        private float _attackTimer = 0f;
        private float RaisingDuration = 30f;
        private float SlammingDuration = 15f;
        private Vector2 _initialPosition;
        private float _maxRaiseHeight = 20f;
        private bool _hasHitGround = false;
        private int timer = 0;

        private int starSpawnTimer = 0;
        private int totalStarsSpawned = 0;
        private int minSpawnInterval = 20;

        private const int STAR_SPAWN_INTERVAL = 120;
        private const int AFTERIMAGE_COUNT = 4;
        private float rotationAngle = 0f;
        private float rotationSpeed = 0.05f;
        private float afterimageDistance = 40f;

        private float ActionDustTime = 0;

        private float flashTimer = 0f;
        private float glowPulseTimer = 0f;
        private Color currentGlowColor = Color.Red;
        private float glowPulseSpeed = 1f;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 999999999;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.alpha = 0;
            Projectile.ownerHitCheck = true;
        }

        public override bool? CanDamage() { return false; }

        public override bool? CanCutTiles() { return false; }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.channel || player.noItems || player.CCed)
            {
                Projectile.Kill();
                return;
            }

            if (timer == 0)
            {
                previousRotations = [];
                previousPostions = [];
            }
            if (player.dead || !player.active)
            {
                Projectile.Kill();
                return;
            }
            player.itemAnimation = player.itemAnimationMax;
            player.itemTime = player.itemTimeMax;
            _attackTimer++;

            glowPulseTimer += (float)Main.gameTimeCache.ElapsedGameTime.TotalSeconds * glowPulseSpeed;
            float pulseIntensity = (float)(Math.Sin(glowPulseTimer * MathHelper.TwoPi) * 0.5f + 0.5f);
            currentGlowColor = Color.Lerp(Color.Orange, Color.White, pulseIntensity);
            if (flashTimer > 0)
                flashTimer -= (float)Main.gameTimeCache.ElapsedGameTime.TotalSeconds * 1f;


            switch (_currentState)
            {
                case AttackState.Raising:
                    HandleRaising(player);
                    player.controlJump = false;
                    player.releaseJump = false;
                    break;
                case AttackState.Slamming:
                    HandleSlamming(player);
                    break;
                case AttackState.Impact:
                    HandleImpact(player);
                    SpawnJesusStars(player);
                    player.controlJump = true;
                    player.releaseJump = true;
                    break;
            }
            player.direction = Utils.ToDirectionInt(Main.MouseWorld.X > player.Center.X);
            UpdateCompositeArm(player);

            if (timer % 2 == 0)
            {
                int trailCount = 10;
                previousRotations.Add(Projectile.rotation);
                previousPostions.Add(Projectile.Center);

                if (previousRotations.Count > trailCount)
                    previousRotations.RemoveAt(0);

                if (previousPostions.Count > trailCount)
                    previousPostions.RemoveAt(0);
            }
            UpdateRotatingAfterimages();
            player.heldProj = Projectile.whoAmI;
            timer++;

            if (ActionDustTime % 9 == 0)
            {
                Projectile.NewProjectile(null, player.Center, Main.rand.NextVector2CircularEdge(3, 3), ModContent.ProjectileType<BishopsStaffStar>(), 0, 0, Main.myPlayer);
            }
            
            ActionDustTime++;

        }

        private void UpdateRotatingAfterimages()
        {
            rotationAngle += rotationSpeed;
            if (_currentState == AttackState.Impact)
            {
                for (int i = 0; i < AFTERIMAGE_COUNT; i++)
                {
                    float angle = rotationAngle + (MathHelper.TwoPi / AFTERIMAGE_COUNT * i);
                    Vector2 offset = new((float)Math.Cos(angle) * afterimageDistance, (float)Math.Sin(angle) * afterimageDistance);
                    Vector2 position = Projectile.Center + offset;
                    Vector2 adjPosition = new Vector2(position.X, position.Y - 12f);
                    int dustType = DustID.HallowedTorch;
                    float dustScale = 1.5f;

                    Dust dust = Dust.NewDustPerfect(adjPosition, dustType, Vector2.Zero, 0, Color.White, dustScale);
                    dust.noGravity = true;
                    dust.noLight = false;
                    dust.fadeIn = 0.2f;
                }
            }
        }

        private void SpawnJesusStars(Player player)
        {
            starSpawnTimer++;
            int dynamicInterval = 100 - (int)(timer * 0.1f);
            if (dynamicInterval < minSpawnInterval)
                dynamicInterval = minSpawnInterval;

            if (starSpawnTimer >= dynamicInterval)
            {
                starSpawnTimer = 0;
                SpawnStarInArc(player);
                flashTimer = 0.5f;
            }
        }

        private List<Vector2> recentSpawnPositions = [];
        private int maxRecentPositions = 5;
        private float minSpawnDistanceFromEachother = 60f;

        private void SpawnStarInArc(Player player)
        {
            float arcRadius = 500f;
            float innerRadius = 100f;
            for (int attempts = 0; attempts < 10; attempts++)
            {
                float angle = Main.rand.NextFloat(0, MathHelper.Pi);
                float distance = Main.rand.NextFloat(innerRadius, arcRadius);
                Vector2 adjStaffPos = new Vector2(Projectile.Center.X, Projectile.Center.Y - 100f);
                Vector2 spawnPosition = adjStaffPos + new Vector2((float)Math.Cos(angle) * distance, (float)Math.Sin(angle) * distance * -1);
                if (spawnPosition.Y >= Projectile.Center.Y)
                    continue;
                bool tooClose = false;
                foreach (Vector2 recentPos in recentSpawnPositions)
                {
                    if (Vector2.Distance(spawnPosition, recentPos) < minSpawnDistanceFromEachother)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (!tooClose)
                {
                    recentSpawnPositions.Add(spawnPosition);
                    if (recentSpawnPositions.Count > maxRecentPositions)
                        recentSpawnPositions.RemoveAt(0);
                    int starDamage = Projectile.damage;
                    totalStarsSpawned++;
                    if (timer > 500 && totalStarsSpawned % 5 == 0)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<HugeJesusStar>(), starDamage, 0.5f, player.whoAmI);
                    else
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<JesusStar>(), starDamage, 0.5f, player.whoAmI);
                    return;
                }
            }
        }

        private void UpdateCompositeArm(Player player)
        {
            float armRotation = 0f;
            switch (_currentState)
            {
                case AttackState.Raising:
                    float raiseProgress = Math.Min(_attackTimer / RaisingDuration, 1f);
                    armRotation = MathHelper.Lerp(-0.2f, -2f, SmoothStep(0f, 1f, raiseProgress));
                    break;
                case AttackState.Slamming:
                    float slamProgress = Math.Min(_attackTimer / SlammingDuration, 1f);
                    armRotation = MathHelper.Lerp(-2f, 0.5f, SmoothStep(0f, 1f, slamProgress));
                    break;
                case AttackState.Impact:
                    armRotation = 0.1f;
                    break;
                case AttackState.Finished:
                    armRotation = MathHelper.Lerp(0.1f, 0f, Math.Min(_attackTimer / 10f, 1f));
                    break;
            }
            if (player.direction > 0)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation * player.direction);
            else
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -armRotation);
        }

        private void HandleRaising(Player player)
        {
            if (_attackTimer == 1)
            {
                _initialPosition = player.Center;
                player.velocity.X *= 0.5f;
            }
            float raiseProgress = Math.Min(_attackTimer / RaisingDuration, 1f);
            float raiseHeight = _maxRaiseHeight * SmoothStep(0f, 1f, raiseProgress);
            float xOffset = (float)Math.Sin(_attackTimer * 0.1f) * 2f;
            xOffset *= player.direction;
            Projectile.Center = new Vector2(player.Center.X + xOffset, player.Center.Y - raiseHeight);
            float targetRotation = player.direction > 0 ? -MathHelper.PiOver4 * 0.5f : MathHelper.PiOver4 * 0.5f;
            Projectile.rotation = MathHelper.Lerp(0f, targetRotation, raiseProgress);
            if (raiseProgress >= 1f)
            {
                _currentState = AttackState.Slamming;
                _attackTimer = 0f;
                SoundEngine.PlaySound(SoundID.Item71.WithVolumeScale(0.75f), player.Center);
            }
        }

        private void HandleSlamming(Player player)
        {
            float slamProgress = Math.Min(_attackTimer / SlammingDuration, 1f);
            float acceleratedSlamProgress = SmoothStep(0f, 1f, slamProgress);
            acceleratedSlamProgress = acceleratedSlamProgress * acceleratedSlamProgress;
            Vector2 startPos = new(player.Center.X, player.Center.Y - _maxRaiseHeight);
            Vector2 endPos = new(player.Center.X, player.Center.Y - 8f);
            Projectile.Center = Vector2.Lerp(startPos, endPos, acceleratedSlamProgress);
            float endOfRaisingRotation = player.direction > 0 ? -MathHelper.PiOver4 * 0.5f : MathHelper.PiOver4 * 0.5f;
            Projectile.rotation = MathHelper.Lerp(endOfRaisingRotation, 0f, acceleratedSlamProgress);

            player.velocity.X *= 0.9f;
            if (slamProgress >= 0.9f && !_hasHitGround)
            {
                _hasHitGround = true;
                _currentState = AttackState.Impact;
                _attackTimer = 0f;
                SoundEngine.PlaySound(SoundID.Item14, player.Center);
                Main.instance.CameraModifiers.Add(new PunchCameraModifier(player.Center, new Vector2(0, -1), 10f, 8f, 10, 1000f));
                for (int i = 0; i < 30; i++)
                {
                    Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-2f, -8f));
                    Dust.NewDust(Projectile.Center, 4, 4, DustID.HallowedTorch, dustVelocity.X, dustVelocity.Y);
                }
            }
        }

        private void HandleImpact(Player player)
        {
            if (_attackTimer >= 10)
                _attackTimer = 10;
            Lighting.AddLight(Projectile.Center, 0.9f, 0.9f, 0.6f);
        }

        private static float SmoothStep(float edge0, float edge1, float x)
        {
            x = MathHelper.Clamp((x - edge0) / (edge1 - edge0), 0f, 1f);
            return x * x * (3 - 2 * x);
        }

        public List<float> previousRotations;
        public List<Vector2> previousPostions;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D textureNoCloth = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Hallow/BishopsStaff/BishopsStaffNoClothProj").Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Hallow/BishopsStaff/BishopsStaffProjGlow").Value;
            Texture2D sunGlowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Hallow/BishopsStaff/BishopsStaffProjSunGlow").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Color drawColor = lightColor;
            if (_currentState == AttackState.Impact)
            {
                for (int i = 0; i < AFTERIMAGE_COUNT; i++)
                {
                    float angle = rotationAngle + (MathHelper.TwoPi / AFTERIMAGE_COUNT * i);
                    Vector2 offset = new((float)Math.Cos(angle) * afterimageDistance, (float)Math.Sin(angle) * afterimageDistance);

                    Vector2 position = Projectile.Center + offset;
                    float scale = 0.7f;
                    float alpha = 0.5f;

                    Main.EntitySpriteDraw(textureNoCloth, position - Main.screenPosition, null, Color.Yellow * alpha, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
                }
            }

            if (_currentState == AttackState.Slamming || _currentState == AttackState.Impact)
            {
                float intensity = 0.1f + (float)Math.Sin(_attackTimer * 0.2f) * 0.1f;
                drawColor = Color.Lerp(Color.Yellow, Color.PaleGoldenrod, intensity);
                for (int i = 0; i < 8; i++)
                    Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(3f, 3f), null, drawColor * intensity, Projectile.rotation, drawOrigin, Projectile.scale * 1.1f, SpriteEffects.None, 0f);
            }
            if (_currentState == AttackState.Finished)
                drawColor.A = (byte)(255 - Projectile.alpha);

            #region after image
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;
                    float size = (1f - ((1f - progress) * 0.5f)) * Projectile.scale;
                    Color col = Color.Yellow * Easings.easeOutCirc(progress);
                    int reverseI = (previousPostions.Count - 1) - i;
                    float size1 = Math.Clamp(Projectile.scale - (reverseI * 0.05f), 0f, 1f);
                    Main.EntitySpriteDraw(texture, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * progress * 0.9f, previousRotations[i], texture.Size() / 2f, size1, SpriteEffects.None);
                }
            }
            #endregion
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float sunGlowOpacity = MathHelper.Clamp(flashTimer * 2f, 0f, 1f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, null, currentGlowColor * 0.7f, Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(sunGlowTexture, Projectile.Center - Main.screenPosition, null, Color.White * sunGlowOpacity, Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;


            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            if (player.velocity.Y != 0)
            {
                if (player.velocity.Y < Player.defaultGravity * 8)
                    player.velocity.Y = Player.defaultGravity * 8;
                Projectile.Kill();
            }
        }
    }

    public class JesusStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;
        public float vortexRot = 0;
        public float vortexRotsmall;
        public float FlareLerp = 0.3f;

        public float[] randomRotation = new float[5];
        float alpha = 0f;
        private bool hasSpawnedLaser = false;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;

            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.scale = 1f;
            Projectile.timeLeft = 100;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override bool? CanDamage() { return false; }

        public override bool? CanCutTiles() { return false; }

        public override void AI()
        {
            Projectile.velocity.Y -= 0.01f;
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1f);

            if (timer == 0)
            {
                for (int i = 0; i < randomRotation.Length; i++)
                {
                    randomRotation[i] = Main.rand.NextFloat(6.28f);
                }
            }

            if (!hasSpawnedLaser && timer >= 30)
            {
                hasSpawnedLaser = true;
                ShootSingleLaser();
            }
            float rotationTargetSpeed = 0.06f;
            float rotationAcceleration = 0.002f;
            float currentRotationSpeed = MathHelper.Lerp(0f, rotationTargetSpeed, MathHelper.Clamp(timer / 40f, 0f, 1f));

            if (Projectile.velocity.X < 0)
            {
                vortexRot -= currentRotationSpeed;
                vortexRotsmall += 1;
                Projectile.rotation -= MathHelper.Lerp(0f, 0.08f, MathHelper.Clamp(timer / 40f, 0f, 1f));
            }
            else
            {
                vortexRot += currentRotationSpeed;
                vortexRotsmall -= 1;
                Projectile.rotation += MathHelper.Lerp(0f, 0.08f, MathHelper.Clamp(timer / 40f, 0f, 1f));
            }
            if (timer < 20)
                alpha = MathHelper.Clamp(timer / 20f, 0f, 1f);
            if (timer > 20)
                FlareLerp = Math.Clamp(FlareLerp - 0.015f, 0, 0.3f);

            goldPulseValue = Math.Clamp(MathHelper.Lerp(goldPulseValue, -0.25f, 0.02f), 0f, 0.5f);

            timer++;
        }

        private Projectile FindBishopsStaffProjectile()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == ModContent.ProjectileType<BishopsStaffProj>())
                {
                    return proj;
                }
            }
            return null;
        }

        private void ShootSingleLaser()
        {
            Projectile staffProj = FindBishopsStaffProjectile();
            float initialAngle;
            bool rotateLeft;

            if (staffProj != null)
            {
                bool staffIsToRight = staffProj.Center.X > Projectile.Center.X;

                if (staffIsToRight)
                {
                    initialAngle = MathHelper.Pi - MathHelper.PiOver2;
                    rotateLeft = true;
                }
                else
                {
                    initialAngle = MathHelper.PiOver2;
                    rotateLeft = false;
                }
            }
            else
            {
                initialAngle = MathHelper.PiOver2;
                rotateLeft = false;
            }

            int laserIndex = Projectile.NewProjectile(
                Projectile.GetSource_FromAI(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<JesusStarLaser>(),
                Projectile.damage,
                1f,
                Projectile.owner
            );

            if (laserIndex >= 0 && laserIndex < Main.maxProjectiles)
            {
                Main.projectile[laserIndex].timeLeft = 180;

                if (Main.projectile[laserIndex].ModProjectile is JesusStarLaser laser)
                {
                    laser.ParentIndex = Projectile.whoAmI;
                    laser.LaserRotation = initialAngle;
                    laser.ForceRotateLeft = rotateLeft;

                    if (staffProj != null)
                        laser.StaffProjIndex = staffProj.whoAmI;
                }
            }

            SoundEngine.PlaySound(SoundID.Item122.WithPitchOffset(0.3f), Projectile.Center);
        }

        float goldPulseValue = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D softGlow = CommonTextures.SoftGlow.Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), Color.Gold * 0.3f * alpha, Projectile.rotation, softGlow.Size() / 2, 0.3f, SpriteEffects.None, 0f);

            Texture2D star = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
            Texture2D star2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_05").Value;

            Main.spriteBatch.Draw(star2, Projectile.Center - Main.screenPosition, star2.Frame(1, 1, 0, 0), Color.Pink * 0.7f * alpha, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * -2), star2.Size() / 2, 0.20f, SpriteEffects.None, 0f);

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            myEffect.Parameters["uColor"].SetValue(Color.Gold.ToVector3() * 1.3f * alpha);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.2f);
            myEffect.Parameters["uSaturation"].SetValue(1.2f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.DeepPink * alpha, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, 0.20f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.Gold * alpha, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, 0.20f, SpriteEffects.None, 0f);

            myEffect.CurrentTechnique.Passes[0].Apply();

            Texture2D FlareFlare = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/flare_01").Value;
            Main.spriteBatch.Draw(FlareFlare, Projectile.Center - Main.screenPosition, FlareFlare.Frame(1, 1, 0, 0), Color.Gold * alpha, MathF.PI, FlareFlare.Size() / 2, 0.35f * 0.5f, SpriteEffects.None, 0f);

            Texture2D swirl = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_02").Value;
            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.Gold * alpha, vortexRot, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.Gold * alpha, vortexRot + MathHelper.Pi, swirl.Size() / 2, 0.10f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }


        public override void OnKill(int timeLeft)
        {
            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.53f, };
            SoundEngine.PlaySound(style, Projectile.Center);

            for (int i = 0; i < 5; i++) //2
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(0, 0), ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), Color.Pink, Main.rand.NextFloat(0.4f, 0.7f), 0.7f, 0f, dustShader);
                p.alpha = 0;
            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/FlareImpact") with { Volume = 0.5f, PitchVariance = 0.1f };
            SoundEngine.PlaySound(style2, Projectile.Center);

            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_45") with { Pitch = .75f, PitchVariance = 0.2f };
            SoundEngine.PlaySound(style, Projectile.Center);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

            int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FireFlareExplosion>(), 0, 0, Main.myPlayer);
            Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
            for (int i = 0; i < 3; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), Color.Gold, Main.rand.NextFloat(0.4f, 0.7f), 0.4f, 0f, dustShader);
                p.alpha = 0;
            }
        }
    }

    public class JesusStarLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int ParentIndex = -1;
        public int StaffProjIndex = -1;
        public float LaserRotation = 0;
        public float LaserLength = 850f;
        public float LaserWidth = 40f;
        public bool ForceRotateLeft = false;
        public bool UseVisualCenter = true;
        private Vector2 VisualOffset => new Vector2(0, 0);
        private Vector2 lastCollisionPos = Vector2.Zero;
        private bool hadCollision = false;
        private float rotationSpeed = 0.004f;
        public int timer = 0;
        public float laserAlpha = 0f;
        private float vortexRotsmall = 0f;

        private Vector2 _endPoint;
        public Vector2 EndPoint => _endPoint;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
            vortexRotsmall += 1;

            if (timer == 0)
                SoundEngine.PlaySound(SoundID.Item122.WithPitchOffset(0.3f), Projectile.Center);
            if (ParentIndex >= 0 && ParentIndex < Main.maxProjectiles)
            {
                if (!Main.projectile[ParentIndex].active || Main.projectile[ParentIndex].type != ModContent.ProjectileType<JesusStar>())
                {
                    Projectile.alpha += 15;
                    if (Projectile.alpha >= 255)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                else
                {
                    Projectile.Center = Main.projectile[ParentIndex].Center;
                }
            }
            else
            {
                Projectile.Kill();
                return;
            }
            if (ForceRotateLeft)
                LaserRotation -= rotationSpeed;
            else
                LaserRotation += rotationSpeed;
            UpdateLaserEndpoint();
            if (timer < 20)
                laserAlpha = timer / 20f;
            else
                laserAlpha = 1f;
            if (timer % 12 == 0)
            {
                float beamLength = (_endPoint - Projectile.Center).Length();

                for (int i = 0; i < 3; i++)
                {
                    float dustDist = Main.rand.NextFloat() * beamLength;
                    Vector2 direction = Vector2.UnitX.RotatedBy(LaserRotation);
                    Vector2 dustPos = Projectile.Center + direction * dustDist;
                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.HallowedTorch, Main.rand.NextVector2Circular(0.5f, 0.5f), 0, Color.Yellow, Main.rand.NextFloat(0.3f, 0.6f));
                    dust.noGravity = true;
                }
            }
            Vector2 lightPos = Vector2.Lerp(Projectile.Center, _endPoint, 0.5f);
            Lighting.AddLight(lightPos, Color.Yellow.ToVector3() * 0.5f * laserAlpha);
            Lighting.AddLight(_endPoint, Color.Yellow.ToVector3() * 0.7f * laserAlpha);
            timer++;
            starAlpha = MathHelper.Clamp(MathHelper.Lerp(starAlpha, 1.25f, 0.02f), 0f, 1f);
            if (Projectile.timeLeft < 30)
                Projectile.alpha = (byte)MathHelper.Lerp(0, 255, 1 - (Projectile.timeLeft / 30f));
        }

        private void UpdateLaserEndpoint()
        {
            float currentDistance = 0;
            Vector2 samplingPoint;
            Vector2 samplingDir = Vector2.UnitX.RotatedBy(LaserRotation);
            float step = 10f;

            while (currentDistance < LaserLength)
            {
                currentDistance += step;
                samplingPoint = Projectile.Center + samplingDir * currentDistance;
                if (Collision.SolidTiles(samplingPoint, 1, 1))
                {
                    _endPoint = samplingPoint;
                    if ((lastCollisionPos - samplingPoint).Length() > 20 || !hadCollision)
                    {
                        lastCollisionPos = samplingPoint;
                        SpawnCollisionDust(samplingPoint);
                    }

                    hadCollision = true;
                    return;
                }
            }
            _endPoint = Projectile.Center + samplingDir * LaserLength;
            hadCollision = false;
        }

        private static void SpawnCollisionDust(Vector2 collisionPoint)
        {
            for (int t = 0; t < 8; t++)
            {
                Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(2f, 3.25f);
                Dust gd = Dust.NewDustPerfect(collisionPoint, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.Gold, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5, preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, EndPoint, LaserWidth * 0.5f, ref point);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer <= 0) return false;

            //Draw the laser
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 end = _endPoint - Main.screenPosition;

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/CheapScroll", AssetRequestMode.ImmediateLoad).Value;

            #region Shader Params
            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Laser1").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);

            Color c1 = Color.DeepPink;
            Color c2 = Color.Gold;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color1Mult"].SetValue(1f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["totalMult"].SetValue(1f);

            myEffect.Parameters["tex1reps"].SetValue(0.25f);
            myEffect.Parameters["tex2reps"].SetValue(0.25f);
            myEffect.Parameters["satPower"].SetValue(1f);
            myEffect.Parameters["time1Mult"].SetValue(1f);
            myEffect.Parameters["time2Mult"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.018f);
            #endregion

            Texture2D beamTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/Clear/ThinnerGlowTrailClear").Value;
            Vector2 beamOrigin = new(0, beamTexture.Height / 2);
            float beamLength = (end - start).Length();
            int beamWidth = (int)(LaserWidth * laserAlpha);
            float alphaMultiplier = (255 - Projectile.alpha) / 255f;
            var mainTarget = new Rectangle((int)start.X, (int)start.Y, (int)beamLength, beamWidth);
            var coreTarget = new Rectangle((int)start.X, (int)start.Y, (int)beamLength, beamWidth / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            //Draw the black base
            Main.spriteBatch.Draw(beamTexture, mainTarget, null, Color.Black * 0.2f * alphaMultiplier, LaserRotation, beamOrigin, SpriteEffects.None, 0);
            Color outerColor = Color.Yellow * 0.6f * alphaMultiplier;
            Color innerColor = Color.White * 0.8f * alphaMultiplier;

            //Draw outer glow
            Main.spriteBatch.Draw(beamTexture, mainTarget, null, outerColor, LaserRotation, beamOrigin, SpriteEffects.None, 0);

            //Draw inner core
            Main.spriteBatch.Draw(beamTexture, coreTarget, null, innerColor, LaserRotation, beamOrigin, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        float starAlpha = 0f;
        public override void PostDraw(Color lightColor)
        {
            Texture2D spotTex = Mod.Assets.Request<Texture2D>("Assets/Flare/CrispStarPMA").Value;
            Texture2D glowTex = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;
            Texture2D star2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_05").Value;

            Vector2 thisPos = _endPoint - Main.screenPosition;

            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Black * 0.5f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.8f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Black * 0.5f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.35f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(glowTex, thisPos, glowTex.Frame(1, 1, 0, 0), Color.Gold * 0.3f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), glowTex.Size() / 2, 0.1f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Gold * 2f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.8f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Gold * 1.5f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.25f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Gold * 2f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.15f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.White * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D softGlow = CommonTextures.SoftGlow.Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(softGlow, thisPos, softGlow.Frame(1, 1, 0, 0), Color.Gold * 0.3f * starAlpha, Projectile.rotation, softGlow.Size() / 2, 0.3f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(star2, thisPos, star2.Frame(1, 1, 0, 0), Color.Pink * 0.7f * starAlpha, vortexRotsmall + MathHelper.ToRadians(vortexRotsmall * -2), star2.Size() / 2, 0.20f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);
                Dust.NewDust(target.Center, 10, 10, DustID.HallowedTorch, velocity.X, velocity.Y, 0, default, 1.2f);
            }
            if (Main.rand.NextBool(3))
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.53f, PitchVariance = 0.25f, MaxInstances = 3, Volume = 0.2f };
                SoundEngine.PlaySound(style, target.Center);
            }
            hit.Damage = (int)(hit.Damage * 1.5f);
            target.immune[Projectile.owner] = 7;
        }
    }

    public class HugeJesusStar : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;
        public float vortexRot = 0;
        public float vortexRotsmall;
        public float FlareLerp = 0.3f;

        public float[] randomRotation = new float[5];

        float alpha = 0f;
        private bool hasSpawnedLaser = false;
        float goldPulseValue = 0f;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;

            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.scale = 1f;
            Projectile.timeLeft = 100;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override bool? CanDamage() { return false; }

        public override bool? CanCutTiles() { return false; }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Yellow.ToVector3() * 1f);

            if (timer == 0)
            {
                for (int i = 0; i < randomRotation.Length; i++)
                {
                    randomRotation[i] = Main.rand.NextFloat(6.28f);
                }
            }

            if (!hasSpawnedLaser && timer >= 30)
            {
                hasSpawnedLaser = true;
                ShootSingleLaser();
            }
            float rotationTargetSpeed = 0.06f;
            float rotationAcceleration = 0.002f;
            float currentRotationSpeed = MathHelper.Lerp(0f, rotationTargetSpeed, MathHelper.Clamp(timer / 40f, 0f, 1f));

            if (Projectile.velocity.X < 0)
            {
                vortexRot -= currentRotationSpeed;
                vortexRotsmall += 1;
                Projectile.rotation -= MathHelper.Lerp(0f, 0.08f, MathHelper.Clamp(timer / 40f, 0f, 1f));
            }
            else
            {
                vortexRot += currentRotationSpeed;
                vortexRotsmall -= 1;
                Projectile.rotation += MathHelper.Lerp(0f, 0.08f, MathHelper.Clamp(timer / 40f, 0f, 1f));
            }
            if (timer < 20)
                alpha = MathHelper.Clamp(timer / 20f, 0f, 1f);
            if (timer > 20)
                FlareLerp = Math.Clamp(FlareLerp - 0.015f, 0, 0.3f);

            goldPulseValue = Math.Clamp(MathHelper.Lerp(goldPulseValue, -0.25f, 0.02f), 0f, 0.5f);

            timer++;
        }

        private Projectile FindBishopsStaffProjectile()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == ModContent.ProjectileType<BishopsStaffProj>())
                {
                    return proj;
                }
            }
            return null;
        }

        private void ShootSingleLaser()
        {
            Projectile staffProj = FindBishopsStaffProjectile();
            float initialAngle;
            bool rotateLeft;

            if (staffProj != null)
            {
                bool staffIsToRight = staffProj.Center.X > Projectile.Center.X;

                if (staffIsToRight)
                {
                    initialAngle = MathHelper.Pi - MathHelper.PiOver2;
                    rotateLeft = true;
                }
                else
                {
                    initialAngle = MathHelper.PiOver2;
                    rotateLeft = false;
                }
            }
            else
            {
                initialAngle = MathHelper.PiOver2;
                rotateLeft = false;
            }

            int laserIndex = Projectile.NewProjectile(
                Projectile.GetSource_FromAI(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<HugeJesusStarLaser>(),
                Projectile.damage,
                1f,
                Projectile.owner
            );

            if (laserIndex >= 0 && laserIndex < Main.maxProjectiles)
            {
                Main.projectile[laserIndex].timeLeft = 180;

                if (Main.projectile[laserIndex].ModProjectile is HugeJesusStarLaser laser)
                {
                    laser.ParentIndex = Projectile.whoAmI;
                    laser.LaserRotation = initialAngle;
                    laser.ForceRotateLeft = rotateLeft;

                    if (staffProj != null)
                        laser.StaffProjIndex = staffProj.whoAmI;
                }
            }

            SoundEngine.PlaySound(SoundID.Item122.WithPitchOffset(0.3f), Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D softGlow = CommonTextures.SoftGlow.Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(softGlow, Projectile.Center - Main.screenPosition, softGlow.Frame(1, 1, 0, 0), Color.Gold * 0.3f * alpha, Projectile.rotation, softGlow.Size() / 2, 0.7f, SpriteEffects.None, 0f);

            Texture2D star = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_06").Value;
            Texture2D star2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_05").Value;

            Main.spriteBatch.Draw(star2, Projectile.Center - Main.screenPosition, star2.Frame(1, 1, 0, 0), Color.Pink * 0.7f * alpha, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * -2), star2.Size() / 2, 0.40f, SpriteEffects.None, 0f);

            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;

            #region Shader Parameters
            myEffect.Parameters["uColor"].SetValue(Color.Gold.ToVector3() * 1.3f * alpha);
            myEffect.Parameters["uTime"].SetValue(2);
            myEffect.Parameters["uOpacity"].SetValue(0.2f);
            myEffect.Parameters["uSaturation"].SetValue(1.2f);
            #endregion

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, null, null, myEffect, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.Green * alpha, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, 0.60f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, star.Frame(1, 1, 0, 0), Color.Gold * alpha, randomRotation[1] + MathHelper.ToRadians(vortexRotsmall * 3 + 45), star.Size() / 2, 0.40f, SpriteEffects.None, 0f);

            myEffect.CurrentTechnique.Passes[0].Apply();

            Texture2D FlareFlare = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/flare_01").Value;

            Main.spriteBatch.Draw(FlareFlare, Projectile.Center - Main.screenPosition, FlareFlare.Frame(1, 1, 0, 0), Color.Gold * alpha, MathF.PI, FlareFlare.Size() / 2, 0.35f * 0.7f, SpriteEffects.None, 0f);

            Texture2D swirl = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_02").Value;
            Texture2D swirl2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/twirl_03").Value;

            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.Gold * alpha, vortexRot, swirl.Size() / 2, 0.20f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(swirl, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.Gold * alpha, vortexRot + MathHelper.Pi, swirl.Size() / 2, 0.30f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(swirl2, Projectile.Center - Main.screenPosition, swirl.Frame(1, 1, 0, 0), Color.Goldenrod * alpha, MathHelper.ToRadians(vortexRotsmall * 8), swirl.Size() / 2, 0.06f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }


        public override void OnKill(int timeLeft)
        {
            ArmorShaderData dustShader = new ArmorShaderData(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad), "ArmorBasic");
            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.53f, };
            SoundEngine.PlaySound(style, Projectile.Center);

            for (int i = 0; i < 5; i++) //2
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(0, 0), ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), Color.Pink, Main.rand.NextFloat(0.4f, 0.7f), 0.7f, 0f, dustShader);
                p.alpha = 0;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/FlareImpact") with { Volume = 0.5f, PitchVariance = 0.1f };
            SoundEngine.PlaySound(style2, Projectile.Center);

            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_45") with { Pitch = .75f, PitchVariance = 0.2f };
            SoundEngine.PlaySound(style, Projectile.Center);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;

            int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FireFlareExplosion>(), 0, 0, Main.myPlayer);
            Main.projectile[a].rotation = Main.rand.NextFloat(6.28f);
            for (int i = 0; i < 3; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(target.Center, ModContent.DustType<GlowCircleRise>(),
                    Main.rand.NextVector2Circular(5, 5), Color.Gold, Main.rand.NextFloat(0.4f, 0.7f), 0.4f, 0f, dustShader);
                p.alpha = 0;
            }
        }
    }

    public class HugeJesusStarLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int ParentIndex = -1;
        public int StaffProjIndex = -1;
        public float LaserRotation = 0;
        public float LaserLength = 850f;
        public float LaserWidth = 120f;
        public bool ForceRotateLeft = false;
        public bool UseVisualCenter = true;
        private Vector2 lastCollisionPos = Vector2.Zero;
        private bool hadCollision = false;
        private float rotationSpeed = 0.004f;
        public int timer = 0;
        public float laserAlpha = 0f;
        private float vortexRotsmall = 0f;
        private Vector2 _endPoint;
        public Vector2 EndPoint => _endPoint;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void AI()
        {
                        starAlpha = MathHelper.Clamp(MathHelper.Lerp(starAlpha, 1.25f, 0.02f), 0f, 1f);
            vortexRotsmall += 1;
            SkillStrikeUtil.setSkillStrike(Projectile, 1.5f);
            if (timer == 0)
                SoundEngine.PlaySound(SoundID.Item122.WithPitchOffset(0.3f), Projectile.Center);
            if (ParentIndex >= 0 && ParentIndex < Main.maxProjectiles)
            {
                if (!Main.projectile[ParentIndex].active || Main.projectile[ParentIndex].type != ModContent.ProjectileType<HugeJesusStar>())
                {
                    Projectile.alpha += 15;
                    if (Projectile.alpha >= 255)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                else
                    Projectile.Center = Main.projectile[ParentIndex].Center;
            }
            else
            {
                Projectile.Kill();
                return;
            }
            if (ForceRotateLeft)
                LaserRotation -= rotationSpeed;
            else
                LaserRotation += rotationSpeed;
            UpdateLaserEndpoint();
            if (timer < 20)
                laserAlpha = timer / 20f;
            else
                laserAlpha = 1f;
            if (timer % 12 == 0)
            {
                float beamLength = (_endPoint - Projectile.Center).Length();

                for (int i = 0; i < 3; i++)
                {
                    float dustDist = Main.rand.NextFloat() * beamLength;
                    Vector2 direction = Vector2.UnitX.RotatedBy(LaserRotation);
                    Vector2 dustPos = Projectile.Center + direction * dustDist;
                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.HallowedTorch, Main.rand.NextVector2Circular(0.5f, 0.5f), 0, Color.Yellow, Main.rand.NextFloat(0.3f, 0.6f));
                    dust.noGravity = true;
                }
            }
            Vector2 lightPos = Vector2.Lerp(Projectile.Center, _endPoint, 0.5f);
            Lighting.AddLight(lightPos, Color.Yellow.ToVector3() * 0.5f * laserAlpha);
            Lighting.AddLight(_endPoint, Color.Yellow.ToVector3() * 0.7f * laserAlpha);
            timer++;
            if (Projectile.timeLeft < 30)
                Projectile.alpha = (byte)MathHelper.Lerp(0, 255, 1 - (Projectile.timeLeft / 30f));
        }

        private void UpdateLaserEndpoint()
        {
            float currentDistance = 0;
            Vector2 samplingPoint;
            Vector2 samplingDir = Vector2.UnitX.RotatedBy(LaserRotation);
            float step = 10f;
            while (currentDistance < LaserLength)
            {
                currentDistance += step;
                samplingPoint = Projectile.Center + samplingDir * currentDistance;
                if (Collision.SolidTiles(samplingPoint, 1, 1))
                {
                    _endPoint = samplingPoint;
                    if ((lastCollisionPos - samplingPoint).Length() > 20 || !hadCollision)
                    {
                        lastCollisionPos = samplingPoint;
                        SpawnCollisionDust(samplingPoint);
                    }

                    hadCollision = true;
                    return;
                }
            }
            _endPoint = Projectile.Center + samplingDir * LaserLength;
            hadCollision = false;
        }

        private static void SpawnCollisionDust(Vector2 collisionPoint)
        {
            for (int t = 0; t < 8; t++)
            {
                Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(2f, 3.25f);
                Dust gd = Dust.NewDustPerfect(collisionPoint, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.Gold, Scale: Main.rand.NextFloat(0.2f, 0.4f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5, preSlowPower: 0.95f, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.9f, shouldFadeColor: false);
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, EndPoint, LaserWidth * 0.5f, ref point);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer <= 0) return false;
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 end = _endPoint - Main.screenPosition;
            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/CheapScroll", AssetRequestMode.ImmediateLoad).Value;
            #region Shader Params
            myEffect.Parameters["sampleTexture1"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Laser1").Value);
            myEffect.Parameters["sampleTexture2"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value);
            Color c1 = Color.DarkGoldenrod;
            Color c2 = Color.Gold;
            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color1Mult"].SetValue(1f);
            myEffect.Parameters["Color2Mult"].SetValue(1f);
            myEffect.Parameters["totalMult"].SetValue(1f);
            myEffect.Parameters["tex1reps"].SetValue(0.25f);
            myEffect.Parameters["tex2reps"].SetValue(0.25f);
            myEffect.Parameters["satPower"].SetValue(1f);
            myEffect.Parameters["time1Mult"].SetValue(1f);
            myEffect.Parameters["time2Mult"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.018f);
            #endregion
            Texture2D beamTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/Clear/ThinnerGlowTrailClear").Value;
            Vector2 beamOrigin = new(0, beamTexture.Height / 2);
            float beamLength = (end - start).Length();
            int beamWidth = (int)(LaserWidth * laserAlpha);
            float alphaMultiplier = (255 - Projectile.alpha) / 255f;
            var mainTarget = new Rectangle((int)start.X, (int)start.Y, (int)beamLength, beamWidth);
            var coreTarget = new Rectangle((int)start.X, (int)start.Y, (int)beamLength, beamWidth / 2);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();
            Main.spriteBatch.Draw(beamTexture, mainTarget, null, Color.Black * 0.2f * alphaMultiplier, LaserRotation, beamOrigin, SpriteEffects.None, 0);
            Color outerColor = Color.Gold * 0.6f * alphaMultiplier;
            Color innerColor = Color.White * 0.8f * alphaMultiplier;
            Main.spriteBatch.Draw(beamTexture, mainTarget, null, outerColor, LaserRotation, beamOrigin, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(beamTexture, coreTarget, null, innerColor, LaserRotation, beamOrigin, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;

            return false;
        }

        float starAlpha = 0f;
        public override void PostDraw(Color lightColor)
        {
            Texture2D spotTex = Mod.Assets.Request<Texture2D>("Assets/Flare/CrispStarPMA").Value;
            Texture2D glowTex = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;
            Texture2D star2 = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/Flares/star_05").Value;

            Vector2 thisPos = _endPoint - Main.screenPosition;

            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Black * 0.5f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.8f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Black * 0.5f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.35f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(glowTex, thisPos, glowTex.Frame(1, 1, 0, 0), Color.Gold * 0.3f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), glowTex.Size() / 2, 0.1f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Gold * 2f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.8f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Gold * 1.5f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.25f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.Gold * 2f * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.15f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(spotTex, thisPos, spotTex.Frame(1, 1, 0, 0), Color.White * starAlpha, MathHelper.ToRadians(vortexRotsmall * 3 + 45), spotTex.Size() / 2, 0.2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D softGlow = CommonTextures.SoftGlow.Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(softGlow, thisPos, softGlow.Frame(1, 1, 0, 0), Color.Gold * 0.3f * starAlpha, Projectile.rotation, softGlow.Size() / 2, 0.3f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(star2, thisPos, star2.Frame(1, 1, 0, 0), Color.Pink * 0.7f * starAlpha, vortexRotsmall + MathHelper.ToRadians(vortexRotsmall * -2), star2.Size() / 2, 0.20f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(2f, 2f);
                Dust.NewDust(target.Center, 10, 10, DustID.HallowedTorch, velocity.X, velocity.Y, 0, default, 1.2f);
            }
            if (Main.rand.NextBool(3))
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_betsy_fireball_shot_2") with { Pitch = -.53f, PitchVariance = 0.25f, MaxInstances = 3, Volume = 0.2f };
                SoundEngine.PlaySound(style, target.Center);
            }
            hit.Damage = (int)(hit.Damage * 1.5f);
            target.immune[Projectile.owner] = 7;
        }
    }

    public class BishopsStaffStar : ModProjectile
    {
        //this is quite literally just linty's mana leech projectile it looks so good

        public override string Texture => "Terraria/Images/Projectile_0";

        private int timer;
        public float scale = 1f;

        public override void SetDefaults()
        {
            Projectile.scale = 1;
            Projectile.width = 2;
            Projectile.height = 2;

            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            Projectile staffProj = FindClosestBishopsStaffProj();
            if (staffProj != null)
            {
                if (timer > 20)
                {
                    Vector2 toStaff = staffProj.Center - Projectile.Center;
                    float desiredSpeed = 13f + (timer * 0.02f);
                    Vector2 desiredVelocity = toStaff.SafeNormalize(Vector2.Zero) * desiredSpeed;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.3f);
                    if (Projectile.Center.Distance(staffProj.Center) < 30f)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            Vector2 dustVel = Main.rand.NextVector2Circular(2f, 2f);
                            Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, dustVel, 150, Color.Gold, 1.3f);
                            d.noGravity = true;
                        }
                        SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = 0.7f, Volume = 0.2f }, staffProj.Center);
                        Projectile.Kill();
                    }
                }
                else
                {
                    Projectile.velocity *= 0.96f;
                }
            }
            else
            {
                Projectile.velocity *= 0.96f;
            }


            scale = Math.Clamp(MathHelper.Lerp(scale, 1.25f, 0.08f), 0f, 1f);
            if (Projectile.velocity.X > 0)
                Projectile.rotation += 0.3f;
            else
                Projectile.rotation -= 0.3f;

            timer++;
        }

        private Projectile FindClosestBishopsStaffProj()
        {
            Projectile closest = null;
            float closestDist = float.MaxValue;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == ModContent.ProjectileType<BishopsStaffProj>())
                {
                    float dist = Vector2.Distance(Projectile.Center, proj.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closest = proj;
                    }
                }
            }
            return closest;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Star = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Pixel/Twinkle", AssetRequestMode.ImmediateLoad).Value;

            Color colToUse = new Color(255, 210, 30); //Gold color
            colToUse.A = 0;

            float finalScale = Projectile.scale * scale * 0.3f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.Draw(Star, drawPos, null, colToUse, Projectile.rotation, Star.Size() / 2, finalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, drawPos, null, colToUse, Projectile.rotation, Star.Size() / 2, finalScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Star, drawPos, null, colToUse, Projectile.rotation, Star.Size() / 2, finalScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}