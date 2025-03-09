using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Sets.Phantic;
using AerovelenceMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Underworld
{
    public class LightOfTheAncients : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            Item.staff[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 46;
            Item.height = 48;
            Item.useTime = 8;
            Item.useAnimation = 8;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 25;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(gold: 8);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<LightOfTheAncientsProjectile>();
            Item.shootSpeed = 16f;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Skill Strikes just as it's about to overheat [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override void HoldItem(Player player)
        {
            bool foundExistingProj = false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<LightOfTheAncientsProjectile>())
                {
                    foundExistingProj = true;
                    break;
                }
            }

            if (!foundExistingProj)
            {
                int projIndex = Projectile.NewProjectile(player.GetSource_ItemUse(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<LightOfTheAncientsProjectile>(), 0, 0, player.whoAmI);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            bool foundExistingProj = false;
            int existingProjIndex = -1;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI && Main.projectile[i].type == ModContent.ProjectileType<LightOfTheAncientsProjectile>())
                {
                    foundExistingProj = true;
                    existingProjIndex = i;
                    break;
                }
            }
            if (foundExistingProj && existingProjIndex != -1)
            {
                if (Main.projectile[existingProjIndex].ModProjectile is LightOfTheAncientsProjectile gunProj)
                    gunProj.TriggerShoot();
            }
            else
            {
                int heldProj = Projectile.NewProjectile(source, position, Vector2.Zero, ModContent.ProjectileType<LightOfTheAncientsProjectile>(), 0, 0, player.whoAmI);

                if (Main.projectile[heldProj].ModProjectile is LightOfTheAncientsProjectile gunProj)
                    gunProj.TriggerShoot();
            }
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ModContent.ItemType<PhanticBar>(), 10);
            recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    public class LightOfTheAncientsProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        //heat management
        private float MAX_HEAT = 100f;
        private float HEAT_PER_SHOT = 10f; //how fast it overheats basically
        private float COOL_RATE = 0.5f;
        private float COOL_RATE_OVERHEATED = 0.75f; //cooling when overheated
        private float OVERHEATED_RECOVERY_THRESHOLD = 40f; //only recover from overheat when cooled to this amount
        private float SKILL_STRIKE_THRESHOLD = 66f; //when skill strikes begin
        private float SKILL_STRIKE_END_THRESHOLD = 95f; //when skill strikes end
        private float OVERHEAT_THRESHOLD = 100f;

        //current heat level
        private float heatLevel = 0f;

        //weapon states
        private bool isOverheated = false;
        private bool canSkillStrike = false;
        private bool needToShoot = false;

        //animation state
        private int frameCounter = 0;
        private int currentFrame = 0;

        //UI frame tracking
        private int meterFrameRow = 0;
        private int overlayFrameIndex = 0;

        //cd between shots
        private int shotCooldown = 0;

        private Player Owner => Main.player[Projectile.owner];
        private float _offset = 0f;
        private Vector2 CurrentDirection => Projectile.rotation.ToRotationVector2();

        private int inactiveCounter = 0;
        private const int MAX_INACTIVE_TIME = 60;

        private bool shouldDrawOverheatedEffect = false;

        public void TriggerShoot()
        {
            needToShoot = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 99999999;
        }

        public int OFFSET = 22;
        public int VERTICAL_OFFSET = -8;
        public ref float Angle => ref Projectile.ai[1];
        public Vector2 direction = Vector2.Zero;
        public float lerpVal = 0;

        public override bool? CanDamage() => false;

        public static Vector2 GetPlayerHandOffset(Player player)
        {
            Vector2 handOffset = Vector2.Zero;
            if (player.bodyFrame.Y == 0)
                handOffset.Y += 1.75f;

            return handOffset + player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, 0);
        }

        private Vector2 recoilOffset = Vector2.Zero;
        private float recoilStrength = 8f;
        private float recoilRecoverySpeed = 0.2f;
        private bool hasRecoil = false;
        private float recoilRotation = 0f;
        private float maxRecoilRotation = -0.35f;
        private float rotationalRecoilRecoverySpeed = 0.1f;

        private void ApplyRecoil()
        {
            float backwardRecoil = -recoilStrength;
            float upwardRecoil = recoilStrength * 0.8f;
            float heatMultiplier = 1f + (heatLevel / MAX_HEAT) * 0.5f;
            if (Owner.direction == -1)
                recoilOffset = new Vector2(backwardRecoil, upwardRecoil) * heatMultiplier;
            else
                recoilOffset = new Vector2(backwardRecoil, -upwardRecoil) * heatMultiplier;
            recoilRotation = maxRecoilRotation * heatMultiplier;
            hasRecoil = true;
        }

        private int lastSelectedItem = -1;

        public override void AI()
        {
            shouldDrawOverheatedEffect = isOverheated;

            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            if (Projectile.owner == Main.myPlayer)
                Angle = (Main.MouseWorld - (Owner.MountedCenter)).ToRotation();
            direction = Angle.ToRotationVector2();
            Owner.ChangeDir(direction.X > 0 ? 1 : -1);
            lerpVal = Math.Clamp(MathHelper.Lerp(lerpVal, -0.2f, 0.002f), 0, 0.4f);
            direction = Angle.ToRotationVector2().RotatedBy(lerpVal * Owner.direction * -1f);
            float armRotation = Projectile.rotation - MathHelper.PiOver2;
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
            float offsetForward = 16f;
            float offsetSide = -12f;
            if (Owner.direction < 0)
                offsetSide *= -1;
            Vector2 offsetVector = new(
                offsetForward * (float)Math.Cos(direction.ToRotation()) - offsetSide * (float)Math.Sin(direction.ToRotation()),
                offsetForward * (float)Math.Sin(direction.ToRotation()) + offsetSide * (float)Math.Cos(direction.ToRotation())
            );
            if (hasRecoil)
            {
                recoilOffset = Vector2.Lerp(recoilOffset, Vector2.Zero, recoilRecoverySpeed);
                recoilRotation = MathHelper.Lerp(recoilRotation, 0f, rotationalRecoilRecoverySpeed);
                if (recoilOffset.Length() < 0.1f && Math.Abs(recoilRotation) < 0.01f)
                {
                    hasRecoil = false;
                    recoilOffset = Vector2.Zero;
                    recoilRotation = 0f;
                }
            }
            Vector2 recoilVector = Vector2.Zero;
            if (hasRecoil)
            {
                recoilVector = new Vector2(
                    recoilOffset.X * (float)Math.Cos(direction.ToRotation()) - recoilOffset.Y * (float)Math.Sin(direction.ToRotation()),
                    recoilOffset.X * (float)Math.Sin(direction.ToRotation()) + recoilOffset.Y * (float)Math.Cos(direction.ToRotation())
                );
            }
            Projectile.Center = armPosition + offsetVector + recoilVector;
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, armRotation);
            float finalRotation = direction.ToRotation();
            if (hasRecoil)
                finalRotation += recoilRotation * Owner.direction;
            Projectile.rotation = finalRotation;
            Player player = Main.player[Projectile.owner];
            if (player.active && !player.dead)
                player.heldProj = Projectile.whoAmI;

            bool holdingCorrectItem = player.HeldItem.type == ModContent.ItemType<LightOfTheAncients>();
            bool selectedItemChanged = lastSelectedItem != player.selectedItem;
            if (selectedItemChanged)
                lastSelectedItem = player.selectedItem;

            if (isOverheated)
            {
                int correctItemSlot = player.FindItem(ModContent.ItemType<LightOfTheAncients>());
                if (!holdingCorrectItem)
                {
                    if (correctItemSlot != -1)
                    {
                        player.selectedItem = correctItemSlot;
                        inactiveCounter = 0;
                    }
                    else
                    {
                        Projectile.Kill();
                        return;
                    }
                }
            }
            else if (!holdingCorrectItem)
            {
                Projectile.Kill();
                return;
            }

            player.heldProj = Projectile.whoAmI;

            if (Projectile.ai[0] == 0)
                _offset = -15f;
            //_offset = MathHelper.Lerp(_offset, 0f, 0.2f);
            Projectile.ai[0]++;
            Vector2 aimDirection = Vector2.Normalize(Main.MouseWorld - Projectile.Center);
            HandleHeatMechanics(player, aimDirection);
            UpdateVisualEffects();
            UpdateAnimations();
        }

        private void HandleHeatMechanics(Player player, Vector2 aimDirection)
        {
            int overheatedRecoveryCooldown = 0;
            if (shotCooldown > 0)
                shotCooldown--;
            if (heatLevel > 0)
            {
                float coolingRate = isOverheated ? COOL_RATE_OVERHEATED : COOL_RATE;
                heatLevel = Math.Max(0, heatLevel - coolingRate);
                if (isOverheated && heatLevel < OVERHEATED_RECOVERY_THRESHOLD)
                {
                    StopGlow();
                    isOverheated = false;
                    heatLevel = OVERHEATED_RECOVERY_THRESHOLD / 2;
                    SoundEngine.PlaySound(SoundID.Item20, Projectile.position);
                    needToShoot = false;
                    heatLevel = 0;
                }
                canSkillStrike = heatLevel >= SKILL_STRIKE_THRESHOLD && heatLevel < SKILL_STRIKE_END_THRESHOLD && !isOverheated;
            }
            bool playerWantsToShoot = needToShoot;
            if (Main.myPlayer == player.whoAmI && Main.mouseRight && !Main.mouseRightRelease && shotCooldown <= 0)
                playerWantsToShoot = true;
            if (playerWantsToShoot && !isOverheated && shotCooldown <= 0)
            {
                needToShoot = false;
                float heatMultiplier = 1f + (heatLevel / MAX_HEAT) * 0.75f;
                int bulletDamage = (int)(player.HeldItem.damage * player.GetDamage(DamageClass.Ranged).Multiplicative * heatMultiplier);
                Vector2 velocity = aimDirection * player.HeldItem.shootSpeed;
                int bulletType = ModContent.ProjectileType<LightOfTheAncientsBullet>();
                bool skillStrikeShot = canSkillStrike;
                int bulletProj = Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), Projectile.Center + aimDirection * 36f, velocity, bulletType, bulletDamage / 3, player.HeldItem.knockBack, player.whoAmI);

                ApplyRecoil();

                if (skillStrikeShot)
                {
                    SkillStrikeUtil.setSkillStrikeWithImpactType(Main.projectile[bulletProj], 1.5f, 1, SkillStrikeImpactType.Basic, 0.6f, 1.2f);
                    for (int i = 0; i < 12; i++)
                    {
                        Dust d = Dust.NewDustDirect(
                            Projectile.Center,
                            10, 10,
                            DustID.GoldFlame,
                            aimDirection.X * 2f, aimDirection.Y * 2f,
                            0, Color.Orange, 1.2f);
                        d.noGravity = true;
                    }
                    SoundStyle skillStrikeSound = new SoundStyle("Terraria/Sounds/Item_14") with { Pitch = 0.15f, Volume = 0.7f };
                    SoundStyle shootSound = new("AerovelenceMod/Sounds/Effects/DeagleShoot");
                    SoundStyle normalSound = shootSound with { Pitch = 0.3f, Volume = 0.5f };
                    SoundEngine.PlaySound(normalSound, Projectile.position);
                    SoundEngine.PlaySound(skillStrikeSound, Projectile.position);
                }
                else
                {
                    float pitchVariation = 0.1f + (heatLevel / MAX_HEAT) * 0.2f;
                    SoundStyle shootSound = new("AerovelenceMod/Sounds/Effects/DeagleShoot");
                    SoundStyle normalSound = shootSound with { Pitch = 0.3f, Volume = 0.5f };
                    SoundEngine.PlaySound(normalSound, Projectile.position);
                }
                heatLevel = Math.Min(heatLevel + HEAT_PER_SHOT, MAX_HEAT);
                if (heatLevel >= OVERHEAT_THRESHOLD)
                {
                    isOverheated = true;
                    canSkillStrike = false;
                    StartGlow();
                    StopFlame();

                    for (int i = 0; i < 20; i++)
                    {
                        Dust d = Dust.NewDustDirect(
                            Projectile.Center,
                            20, 20,
                            DustID.Torch,
                            Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f),
                            0, Color.Red, 1.5f);
                        d.noGravity = true;
                    }
                    SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
                }
                shotCooldown = 8;
                canSkillStrike = heatLevel >= SKILL_STRIKE_THRESHOLD && heatLevel < OVERHEAT_THRESHOLD && !isOverheated;
            }
            UpdateMeterFrames();
        }



        private void UpdateMeterFrames()
        {
            if (isOverheated)
            {
                overlayFrameIndex = 4;
                meterFrameRow = 3;
            }
            else if (heatLevel >= SKILL_STRIKE_THRESHOLD)
            {
                //we're in skill strike territory baby
                overlayFrameIndex = 2;
               // meterFrameRow = 2;
            }
            else if (heatLevel > 0)
            {
                //any heat level below skill strike threshold shows regular meter which is the first column
                overlayFrameIndex = 1;
               // meterFrameRow = 0;
            }
            else
            {
                //don't show meter when heat is 0
                overlayFrameIndex = 0;
                meterFrameRow = 0;
            }
            canSkillStrike = heatLevel >= SKILL_STRIKE_THRESHOLD && heatLevel < SKILL_STRIKE_END_THRESHOLD && !isOverheated;
        }

        private void UpdateAnimations()
        {
            frameCounter++;
            if (frameCounter >= 5)
            {
                frameCounter = 0;

                if (isOverheated)
                {
                    currentFrame = 0;
                }
                else if (canSkillStrike)
                {
                    StartFlame();
                    currentFrame = currentFrame + 1;
                    if (currentFrame < 1 || currentFrame > 4)
                        currentFrame = 1;
                }
                else
                {
                    StopFlame();
                    currentFrame = 0;
                }
            }
        }

        private float flameOpacity = 0f;
        private float glowOpacity = 0f;
        private float targetFlameOpacity = 0f;
        private float targetGlowOpacity = 0f;
        private float OPACITY_TRANSITION_SPEED = 0.05f;

        public void StartGlow()
        {
            targetGlowOpacity = 1f;
        }

        public void StopGlow()
        {
            targetGlowOpacity = 0f;
        }

        public void StartFlame()
        {
            targetFlameOpacity = 1f;
        }

        public void StopFlame()
        {
            targetFlameOpacity = 0f;
        }

        private void UpdateVisualEffects()
        {
            if (flameOpacity < targetFlameOpacity)
            {
                flameOpacity = Math.Min(flameOpacity + OPACITY_TRANSITION_SPEED, targetFlameOpacity);
            }
            else if (flameOpacity > targetFlameOpacity)
            {
                flameOpacity = Math.Max(flameOpacity - OPACITY_TRANSITION_SPEED, targetFlameOpacity);
            }

            if (glowOpacity < targetGlowOpacity)
            {
                glowOpacity = Math.Min(glowOpacity + OPACITY_TRANSITION_SPEED, targetGlowOpacity);
            }
            else if (glowOpacity > targetGlowOpacity)
            {
                glowOpacity = Math.Max(glowOpacity - OPACITY_TRANSITION_SPEED, targetGlowOpacity);
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Underworld/LightOfTheAncientsProjectile").Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Underworld/LightOfTheAncientsProjectileWhiteGlow").Value;
            Texture2D flameGlowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Underworld/LightOfTheAncientsGlow").Value;
            float rotation = Projectile.rotation;
            if (Owner.direction == -1)
                rotation += MathHelper.Pi;
            SpriteEffects spriteEffects = (Owner.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            int frameHeight = texture.Height / 5;
            Rectangle frame = new(0, currentFrame * frameHeight, texture.Width, frameHeight);
            Vector2 origin = new(texture.Width / 2, frameHeight / 2);
            float scale = 1f;
            Vector2 actualPos = Projectile.Center - Main.screenPosition;
            if (shouldDrawOverheatedEffect || glowOpacity > 0)
            {
                for (int i = 0; i < 8; i++)
                {
                    Color col = i == 0 ? Color.DarkRed with { A = 0 } : Color.DarkRed with { A = 0 };
                    Vector2 offset = new Vector2(1, 0).RotatedBy(MathHelper.PiOver2 * i) * 2f;
                    Main.spriteBatch.Draw(texture, actualPos + Main.rand.NextVector2Circular(3f, 3f), frame,
                        col * glowOpacity, rotation, origin, Projectile.scale * 1.1f, spriteEffects, 0f);
                }
            }

            Main.spriteBatch.Draw(texture, actualPos, frame, lightColor, rotation, origin, scale, spriteEffects, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            float minGlowVisibility = 0.05f;

            //base white
            float glowIntensity = Math.Max(minGlowVisibility, Math.Min(heatLevel / MAX_HEAT, 1f));
            Main.spriteBatch.Draw(glowTexture, actualPos, null, Color.White * glowIntensity * 0.7f, rotation, origin, scale, spriteEffects, 0f);
            float orangeBase = heatLevel > MAX_HEAT * 0.4f ? Math.Min((heatLevel - (MAX_HEAT * 0.4f)) / (MAX_HEAT * 0.6f), 1f) : 0f;
            float orangeIntensity = Math.Max(minGlowVisibility, orangeBase);
            Main.spriteBatch.Draw(glowTexture, actualPos, null, Color.Orange * orangeIntensity * 0.6f, rotation, origin, scale, spriteEffects, 0f);

            //redhot
            float redBase = heatLevel > MAX_HEAT * 0.7f ?
                Math.Min((heatLevel - (MAX_HEAT * 0.7f)) / (MAX_HEAT * 0.3f), 1f) :
                0f;
            float redIntensity = Math.Max(minGlowVisibility, redBase);
            Main.spriteBatch.Draw(glowTexture, actualPos, null, Color.Red * redIntensity * 0.5f, rotation, origin, scale, spriteEffects, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            //hhhh needs this otherwise arm glow
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int j = 0; j < 3; j++)
            {
                ulong randShakeEffect = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)Projectile.whoAmI);
                for (int c = 0; c < 7; c++)
                {
                    float shakeX = Utils.RandomInt(ref randShakeEffect, -10, 11) * 0.15f;
                    float shakeY = Utils.RandomInt(ref randShakeEffect, -10, 1) * 0.35f;
                    Main.spriteBatch.Draw(flameGlowTexture, new Vector2(actualPos.X + shakeX, actualPos.Y + shakeY), frame,
                        new Color(255, 60, 10, 0) * 0.2f * flameOpacity, rotation, origin, scale * 1.1f, spriteEffects, 0f);
                }
            }

            DrawHeatMeter();
        }


        private void DrawHeatMeter()
        {
            if (heatLevel <= 0)
                return;
            Player player = Main.player[Projectile.owner];
            Texture2D meterTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Underworld/LightMeter").Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Underworld/LightMeterGlow").Value;
            Texture2D overlayTexture = ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Underworld/LightMeterOverlay").Value;
            int frameWidth = 98;
            int frameHeight = 54;
            int paddingX = 2;
            int paddingY = 2;
            int animRow = ((int)(Main.GlobalTimeWrappedHourly * 15f)) % 4;
            Vector2 meterPosition = player.Center - Main.screenPosition;
            meterPosition.Y -= player.height + 30;
            Rectangle backgroundRect = new(meterFrameRow * (frameWidth + paddingX), animRow * (frameHeight + paddingY), frameWidth, frameHeight);
            Main.spriteBatch.Draw(meterTexture, meterPosition, backgroundRect, Color.White, 0f, new Vector2(frameWidth / 2, frameHeight / 2), 1f, SpriteEffects.None,0f);

            //for overheated state the background is already the full overheated texture
            if (isOverheated)
            {
                string soulText = $"{Math.Floor((heatLevel - 30) / 10)}";
                Vector2 textSize = FontAssets.ItemStack.Value.MeasureString(soulText);
                Vector2 textPos =  meterPosition - new Vector2(textSize.X / 2, 12);
                Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.ItemStack.Value, soulText, textPos.X, textPos.Y, Color.White, Color.Black, Vector2.Zero, 1.2f);
            }
            else
            {
                float fillPercentage = heatLevel / MAX_HEAT;
                int fillWidth = (int)(frameWidth * fillPercentage);
                if (fillWidth > 0)
                {
                    //int fillColumn;
                    //if (heatLevel >= SKILL_STRIKE_THRESHOLD)
                    //    fillColumn = 2;
                    //else
                    //    fillColumn = 1;
                    Rectangle fillSourceRect = new(2 * (frameWidth + paddingX), animRow * (frameHeight + paddingY), fillWidth, frameHeight);
                    int leftEdge = (int)(meterPosition.X - frameWidth / 2);
                    Rectangle fillDestRect = new Rectangle(leftEdge, (int)(meterPosition.Y - frameHeight / 2), fillWidth, frameHeight);
                    Color fillColor = Color.White;
                    Main.spriteBatch.Draw(meterTexture, fillDestRect, fillSourceRect, fillColor, 0f, Vector2.Zero, SpriteEffects.None, 0f);
                }
            }

            for (int j = 0; j < 3; j++)
            {
                ulong randShakeEffect = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)Projectile.whoAmI);

                for (int c = 0; c < 7; c++)
                {
                    float shakeX = Utils.RandomInt(ref randShakeEffect, -10, 11) * 0.15f;
                    float shakeY = Utils.RandomInt(ref randShakeEffect, -10, 1) * 0.35f;
                    Main.spriteBatch.Draw(glowTexture, new Vector2(meterPosition.X + shakeX, meterPosition.Y + shakeY), backgroundRect, new Color(255, 60, 10, 0) * 0.2f, 0f, new Vector2(frameWidth / 2, frameHeight / 2), 1f, SpriteEffects.None, 0f);
                    Main.spriteBatch.Draw(glowTexture, new Vector2(meterPosition.X + shakeX, meterPosition.Y + shakeY), backgroundRect, (new Color(026, 194, 188, 0) * 0.2f) * flameOpacity, 0f, new Vector2(frameWidth / 2, frameHeight / 2), 1f, SpriteEffects.None, 0f);


                }
            }
            if (overlayFrameIndex > 0)
            {
                Rectangle overlayRect = new((overlayFrameIndex - 1) * (frameWidth + paddingX), 0, frameWidth, frameHeight);
                Main.spriteBatch.Draw(overlayTexture, meterPosition, overlayRect, Color.White, 0f, new Vector2(frameWidth / 2, frameHeight / 2), 1f, SpriteEffects.None, 0f);
            }
        }
    }

    public class LightOfTheAncientsBullet : TrailProjBase
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        int timer = 0;
        int trailPoints = 200;
        float alpha = 1;
        bool justHit = false;
        float justHitTimer = 4;

        public override bool? CanDamage()
        {
            return timer < 50 && !justHit;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity = Vector2.Zero;
            justHit = true;

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 3; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleQuadStar>(),
                    Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-1, 1)) * Main.rand.Next(1, 3),
                    Color.Orange, Main.rand.NextFloat(0.35f, 0.55f), 0.4f, 0f, dustShader);
            }
        }

        public override void AI()
        {
            if (!justHit)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
                trailColor = new Color(255, 140, 0);
                trailTime = timer * 0.02f;

                trailPointLimit = 100;
                trailWidth = 16;
                trailMaxLength = trailPoints;

                trailRot = Projectile.velocity.ToRotation();
                trailPos = Projectile.Center + Projectile.velocity;

                TrailLogic();

                Lighting.AddLight(Projectile.position, Color.Orange.ToVector3() * 0.45f);

                if (timer > 10)
                {
                    trailPoints = (int)Math.Clamp(MathHelper.Lerp(trailPoints, -0.2f, 0.07f), 0f, 200f);

                    if (timer > 40)
                    {
                        alpha = MathHelper.Lerp(alpha, 0, 0.08f);
                    }
                }
                if (trailPoints <= 5)
                    Projectile.active = false;

                timer++;
            }
            else
            {
                justHitTimer--;

                trailColor = Color.Lerp(Color.White, Color.Orange, 0.8f);

                if (justHitTimer <= 0)
                {
                    Projectile.Kill();
                    Projectile.active = false;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            TrailDrawing();

            Texture2D tex = Mod.Assets.Request<Texture2D>("Assets/Pixel/Nightglow").Value;
            Vector2 scale = new Vector2(Projectile.scale * 1.8f, Projectile.scale) * 0.5f;

            Color col = justHit ? Color.White : Color.Orange;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(1, 1, 0, 0), col * 2f * alpha, Projectile.rotation, tex.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(1, 1, 0, 0), Color.White * alpha, Projectile.rotation, tex.Size() / 2, scale * 0.5f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override float WidthFunction(float progress)
        {
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 26f, num) * 0.5f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_10") with { Pitch = -0.4f, PitchVariance = .28f, MaxInstances = 4, Volume = 0.2f };
            SoundEngine.PlaySound(style, Projectile.Center);

            Collision.HitTiles(Projectile.position + (Projectile.velocity * 0.5f), Projectile.velocity * 0.5f, Projectile.width, Projectile.height);

            ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
            for (int i = 0; i < 3; i++)
            {
                Dust p = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleQuadStar>(),
                    Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.Pi + Main.rand.NextFloat(-1, 1)) * Main.rand.Next(1, 3),
                    Color.Orange, Main.rand.NextFloat(0.35f, 0.55f), 0.4f, 0f, dustShader);
            }
        }
    }
}