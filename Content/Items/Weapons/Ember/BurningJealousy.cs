using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using Terraria.Audio;
using AerovelenceMod.Content.Projectiles;
using ReLogic.Content;
using Terraria.Graphics;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Content.Projectiles.Other;
using static Terraria.NPC;

namespace AerovelenceMod.Content.Items.Weapons.Ember
{
    public class BurningJealousy : ModItem
    {
        bool tick = false;

        public override void SetDefaults()
        {
            Item.damage = 94;
            Item.knockBack = KnockbackTiers.Average;

            Item.width = 52;
            Item.height = 64;
            Item.useAnimation = 40;
            Item.useTime = 40;


            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.rare = ItemRarities.EarlyHardmode;
            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.channel = true;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<BurningJealousyHeldProj>();
        }
        public override bool AltFunctionUse(Player player) => true;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine SkillStrike = new(Mod, "SkillStrike", "[i:" + ItemID.FallenStar + "] Counter-Attack Skill Strikes [i:" + ItemID.FallenStar + "]")
            {
                OverrideColor = Color.Gold,
            };
            tooltips.Add(SkillStrike);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.HellstoneBar, 15).
                AddIngredient(ItemID.SoulofNight, 7).
                AddRecipeGroup("AerovelenceMod:MechSouls", 3).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            tick = !tick;
            if (player.altFunctionUse == 2) 
                type = ModContent.ProjectileType<BurningJealousyGuard>(); 
            
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
            return false;
        }
    }

    public class BurningJealousyHeldProj : TrailProjBase
    {
        public bool bigSwing = false;
        
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;

            Projectile.timeLeft = 10000;
            Projectile.width = Projectile.height = 85;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.extraUpdates = 8;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.noEnchantmentVisuals = true;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.ownerHitCheck = true;
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f) && justHitTime <= 0;
            return shouldDamage;
        }

        bool firstFrame = true;
        float startingAng = 0;
        float currentAng = 0;

        float Angle = 0;

        bool playedSound = false;

        float storedDirection = 1;

        int timer = 0;

        int timerAfterEnd = 8;
        float mytrailWidth = 65;

        float amountsToSwing = 1f;

        public List<float> previousRotations;

        BaseTrailInfo relativeTrail = new BaseTrailInfo();

        public int justHitTime = -1;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            if (bigSwing) Projectile.scale = 1.3f;

            #region arm shit

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Projectile.Center - (player.Center)).ToRotation();
            }

            if (firstFrame)
                storedDirection = player.direction;

            float itemrotate = storedDirection < 0 ? MathHelper.Pi : 0;
            if (player.direction != storedDirection)
                itemrotate += MathHelper.Pi;
            player.itemRotation = Angle + itemrotate;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);
            //if (easingProgress > 0.5)
            //player.ChangeDir(Projectile.Center.X > player.Center.X ? 1 : -1);

            Vector2 frontHandPos = Main.GetPlayerArmPosition(Projectile);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.Center - player.Center).ToRotation() - MathHelper.PiOver2);
            //player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, (Projectile.Center - player.Center).ToRotation() - MathHelper.PiOver2);
            #endregion

            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                Projectile.Kill();
            }

            //This is were we set the beggining and ending angle of the sword //old code bad but too lazy to redo
            if (firstFrame)
            {
                easingProgress = bigSwing ? 0f : 0.1f; //0.07 \ 0.25
                timer = 0;
                //No getting the mouse Direction via Main.mouse world did not work
                Vector2 mouseDir = Projectile.velocity.SafeNormalize(Vector2.UnitX);

                //I don't know why this is called sus but this is the angle we will compare the mouseDir to 
                Vector2 sus1 = new Vector2(-10, 0).SafeNormalize(Vector2.UnitX);

                //Yes I did have to normalize it again (i don't fuckin know why but it is needed)
                Vector2 sus2 = mouseDir.SafeNormalize(Vector2.UnitX);

                startingAng = sus1.AngleTo(sus2) * 2; //psure the * 2 is from double normalization

                if (Projectile.ai[0] == 1)
                {
                    startingAng = startingAng - MathHelper.ToRadians(-180); //-170
                }
                else
                {
                    startingAng = startingAng + MathHelper.ToRadians(-180); //-170
                }

                if (Math.Abs(startingAng) == MathF.PI && storedDirection == -1)
                {
                    startingAng += MathHelper.Pi;
                }

                currentAng = startingAng;
                firstFrame = false;

                previousRotations = new List<float>();
                Projectile.spriteDirection = Main.MouseWorld.X > Main.player[Projectile.owner].MountedCenter.X ? 1 : -1;
            }


            if (timer >= 0 && justHitTime <= 0)
            {
                if (Projectile.ai[0] == 1)
                    currentAng = startingAng - MathHelper.ToRadians((360 * getProgress(easingProgress)));
                else
                    currentAng = startingAng + MathHelper.ToRadians((360 * getProgress(easingProgress)));

                float meleeSpeed = Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee);

                float advanceSpeed = bigSwing ? 0.0024f * Math.Clamp(meleeSpeed * 0.75f, 1f, 2f) : 0.0021f * meleeSpeed; //0.0024 | 0.0019,21

                easingProgress = Math.Clamp(easingProgress + advanceSpeed, 0.05f, 0.95f * amountsToSwing);
            }

            float centerOffset = bigSwing ? 70 : 60;

            Projectile.rotation = currentAng + MathHelper.PiOver4;
            Projectile.Center = (currentAng.ToRotationVector2() * centerOffset) + player.RotatedRelativePoint(player.MountedCenter);
            player.itemTime = 10;
            player.itemAnimation = 10;

            //AfterImage
            if (timer % 6 == 0 && justHitTime <= 0)
            {
                previousRotations.Add(Projectile.rotation);

                float listLength = bigSwing ? 12 : 10; //14 : 8

                if (previousRotations.Count > listLength)
                {
                    previousRotations.RemoveAt(0);
                }
            }

            if (justHitTime <= 0)
                timer++;

            if (getProgress(easingProgress) >= 0.35f * amountsToSwing && getProgress(easingProgress) <= 0.78f * amountsToSwing)
            {
                Projectile.ai[1] = 1;
            }
            else
                Projectile.ai[1] = 0;

            //Sound
            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                SoundStyle style2 = new SoundStyle("Terraria/Sounds/Custom/dd2_lightning_bug_zap_1") with { Pitch = -.36f, Volume = 0.3f, PitchVariance = 0.2f };
                SoundEngine.PlaySound(style2, Projectile.Center);

                SoundEngine.PlaySound(SoundID.Item71 with { Pitch = -0.2f, PitchVariance = 0.15f, Volume = 0.37f }, Projectile.Center);


                if (bigSwing)
                {
                    SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/StampAirSwing1") with { Volume = .14f, Pitch = 0, PitchVariance = 0.2f };
                    SoundEngine.PlaySound(style3, Projectile.Center);

                    SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_flameburst_tower_shot_2") with { Pitch = -0.8f, PitchVariance = 0.3f };
                    SoundEngine.PlaySound(style, Projectile.Center);
                } 
                else
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_flameburst_tower_shot_2") with { Pitch = -.48f, PitchVariance = 0.3f };
                    SoundEngine.PlaySound(style, Projectile.Center);
                }


                playedSound = true;
            }

            if (getProgress(easingProgress) >= .99f * amountsToSwing)
            {
                if (timerAfterEnd == 0)
                {
                    player.itemTime = 0;
                    player.itemAnimation = 0;
                    Projectile.active = false;
                }
                timerAfterEnd--;
                
            }

            if (getProgress(easingProgress) >= 0.75)
            {
                mytrailWidth = MathHelper.Lerp(mytrailWidth, 0, 0.0035f);
            }
            else if (getProgress(easingProgress) <= 0.3f)
            {
                mytrailWidth = MathHelper.Lerp(0, 60, getProgress(easingProgress) / 0.3f);
            }

            float intensity = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            //New Trail
            Color trailCol = (getProgress(easingProgress) <= 0.2f) ? Color.OrangeRed : Color.OrangeRed * (getProgress(easingProgress) >= 0.85f ? (1.2f - getProgress(easingProgress)) : 1.2f);
            Vector2 gfxOffset = new Vector2(0, -Main.player[Projectile.owner].gfxOffY);

            if (bigSwing && getProgress(easingProgress) >= 0.93f)
            {
                trailCol = Color.Lerp(Color.OrangeRed * 1.2f, Color.OrangeRed * 0.3f, (getProgress(easingProgress) - 0.93f) / 0.7f);

            }
            else if (getProgress(easingProgress) >= 0.87f)
            {
                trailCol = Color.Lerp(Color.OrangeRed * 1.2f, Color.OrangeRed * 0.3f, (getProgress(easingProgress) - 0.87f) / 0.13f);
            }
            
            relativeTrail.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trail5").Value;
            relativeTrail.trailColor = trailCol;
            relativeTrail.trailPointLimit = 800;
            relativeTrail.trailWidth = (int)(30 * (mytrailWidth / 28));
            relativeTrail.trailMaxLength = 300;
            relativeTrail.timesToDraw = 2;
            relativeTrail.relativeToPlayer = true;
            relativeTrail.myPlayer = Main.player[Projectile.owner];
            relativeTrail.trailTime = (float)trailTimeCounter * 0.004f;
            relativeTrail.trailRot = Projectile.rotation + MathHelper.PiOver4;

            relativeTrail.trailPos = Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedBy(-1f) 
                * (bigSwing ? 15f : 12f * (1 + intensity * 0.35f)) 
                - Main.player[Projectile.owner].MountedCenter + gfxOffset;

            if (justHitTime <= 0 && getProgress(easingProgress) > 0.07) 
                relativeTrail.TrailLogic();

            //NewDust
            int dustMod = (int)Math.Clamp(4f - (2f * (player.GetTotalAttackSpeed(DamageClass.Melee) - 1f)), 0, 5);
            if (timer % dustMod == 0 && (getProgress(easingProgress) >= 0.2f && getProgress(easingProgress) <= 0.8f) && justHitTime <= 0)
            {
                Dust d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + currentAng.ToRotationVector2() * Main.rand.NextFloat(50f, 100f), ModContent.DustType<GlowStrong>(),
                    currentAng.ToRotationVector2().RotatedByRandom(0.3f).RotatedBy(MathHelper.PiOver2 * (Projectile.ai[0] > 0 ? 1 : -1)) * -Main.rand.NextFloat(2f, 5f),
                    0, newColor: new Color(255, 95, 0), 0.2f + Main.rand.NextFloat(-0.05f, 0.05f));
                d.scale *= Projectile.scale;
            }

            trailTimeCounter++;
            justHitTime--;
        }

        int trailTimeCounter = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            //TODO load textures once cause wowie
            Texture2D Sword = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousy");

            Texture2D WithoutHilt = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousyWithoutHilt");
            Texture2D Hilt = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousyHilt");

            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousyGlow");
            Texture2D GlowWhole2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousyBackGlow2");
            Texture2D GlowEdge = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BGEdgeGlow");
            Texture2D Star = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Flare");

            Texture2D Wave = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/ImpactTextures/pixelKennySlashBlack");

            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            if (Projectile.ai[0] != 1)
            {
                origin = new Vector2(0, Sword.Height);
                rotationOffset = 0;
                effects = SpriteEffects.None;
            }
            else
            {
                origin = new Vector2(Sword.Width, Sword.Height);
                rotationOffset = MathHelper.ToRadians(90f);
                effects = SpriteEffects.FlipHorizontally;
            }

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAng); 

            //Sprite is 52x64 so -12y to "make it square", dont know about the x tbh
            Vector2 otherOffset = new Vector2(Projectile.spriteDirection > 0 ? 4 : 0, Projectile.spriteDirection > 0 ? -8 : -12).RotatedBy(currentAng);
            Vector2 gfxOffset = new Vector2(0, -Main.player[Projectile.owner].gfxOffY);

            //1f at middle of swing, 0f at both ends
            float intensity = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            //Hilt
            Main.spriteBatch.Draw(Hilt, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), effects, 0f);


            //Big slash texture
            Vector2 waveScale = new Vector2(0.85f, 1f) * 0.97f * (bigSwing ? 1.2f : Projectile.scale);
            Color waveColor = Color.Lerp(Color.Black * 0.3f, new Color(255, 85, 0), Easings.easeInOutCirc(intensity));
            Vector2 BusterPos = Main.player[Projectile.owner].Center + new Vector2(-30f + (70f * intensity), 0).RotatedBy(startingAng) - Main.screenPosition;

            Main.spriteBatch.Draw(Wave, BusterPos, Wave.Frame(1, 1, 0, 0), waveColor with { A = 0 } * 0.6f, startingAng - MathHelper.PiOver2, Wave.Size() / 2, waveScale * (0.3f + (intensity * 0.75f)), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Wave, BusterPos, Wave.Frame(1, 1, 0, 0), waveColor with { A = 0 } * 0.3f, startingAng - MathHelper.PiOver2, Wave.Size() / 2, waveScale * (0.15f + (intensity * 0.75f)), SpriteEffects.None, 0f);

            //Trail resets spritebatch which makes it so that the player's hand is no longer behind the weapon, so we need to do things in this order
            TrailDrawing();
            relativeTrail.TrailDrawing(Main.spriteBatch);

            //Have to reset spritebatch cuz 1.4.4 player hand drawing messed up
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //Sword
            Main.spriteBatch.Draw(WithoutHilt, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), effects, 0f);


            //SwordGlow
            float glowmaskProgress = 0.5f + (0.5f * intensity);
            Main.spriteBatch.Draw(Glow, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, Color.White * glowmaskProgress, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), effects, 0f);
            Main.spriteBatch.Draw(Glow, armPosition - Main.screenPosition + otherOffset - gfxOffset + (Main.rand.NextVector2Circular(4, 4) * intensity), null, Color.White with { A = 0 } * intensity, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), effects, 0f);


            Main.spriteBatch.Draw(GlowEdge, armPosition - Main.screenPosition + otherOffset - gfxOffset + (Main.rand.NextVector2Circular(4, 4) * intensity), null, Color.OrangeRed with { A = 0 } * intensity * 1f, Projectile.rotation + rotationOffset, origin, Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f), effects, 0f);


            //AfterImage
            if (previousRotations != null)
            {
                for (int afterI = 0; afterI < previousRotations.Count; afterI++)
                {
                    float progress = (float)afterI / previousRotations.Count;

                    float size = (Projectile.scale + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f));
                    size *= (0.75f + (progress * 0.25f));

                    Main.spriteBatch.Draw(GlowWhole2, armPosition - Main.screenPosition + otherOffset - gfxOffset, null, Color.OrangeRed with { A = 0 } * progress * intensity, previousRotations[afterI] + rotationOffset, origin, size, effects, 0f);
                }

            }

            //OnHit star
            Vector2 starPos = Main.player[Projectile.owner].Center + (new Vector2(1 + (intensity * 0.25f), 0f).RotatedBy(currentAng + (Projectile.ai[0] == 1 ? 0.1f : -0.1f)) * 75f * (bigSwing ? 1.25f : 1f));
            float starRot = (float)Main.timeForVisualEffects * 0.1f;
            Vector2 starScale = new Vector2(0.9f, 2f) * Projectile.scale;

            Main.spriteBatch.Draw(Star, starPos - Main.screenPosition, null, Color.OrangeRed with { A = 0 } * intensity, starRot, Star.Size() / 2, starScale * 0.5f * intensity, effects, 0f);
            Main.spriteBatch.Draw(Star, starPos - Main.screenPosition, null, Color.OrangeRed with { A = 0 } * intensity, starRot + MathHelper.PiOver2, Star.Size() / 2, starScale * 0.5f * intensity, effects, 0f);

            Main.spriteBatch.Draw(Star, starPos - Main.screenPosition, null, Color.White with { A = 0 } * intensity, -starRot, Star.Size() / 2, starScale * 0.15f * intensity, effects, 0f);
            Main.spriteBatch.Draw(Star, starPos - Main.screenPosition, null, Color.White with { A = 0 } * intensity, -starRot + MathHelper.PiOver2, Star.Size() / 2, starScale * 0.15f * intensity, effects, 0f);


            return false;
        }

        public override void OnHitNPC(NPC target, HitInfo hit, int damageDone)
        {
            //justHitTime = bigSwing ? 50 : 37;

            //Want less hitpause at higher attack speeds for swing

            if (bigSwing)
            {
                justHitTime = (6 - (int)((Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1) * 4f)) * Projectile.extraUpdates;
                Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 15f - ((Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1) * 10f); ;

            }
            else
            {
                justHitTime = (4 - (int)((Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1) * 3f)) * Projectile.extraUpdates;
                Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 8f - ((Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1) * 4f);

            }

            float bigSwingMultiplier = bigSwing ? 1.25f : 1f;

            Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = bigSwing ? 10 : 5;



            Vector2 vec = (target.Center - Main.player[Projectile.owner].Center).SafeNormalize(Vector2.UnitX);

            Vector2 orthToSwing = (MathHelper.PiOver2 + currentAng).ToRotationVector2() * (Projectile.ai[0] == 1 ? -1 : 1f);


            int explosion = Projectile.NewProjectile(null, Main.player[Projectile.owner].Center + vec * 45f, Vector2.Zero, ModContent.ProjectileType<FadeExplosionHandler>(), 0, 0, Main.myPlayer);

            if (Main.projectile[explosion].ModProjectile is FadeExplosionHandler feh)
            {
                feh.color = Color.OrangeRed;
                feh.colorIntensity = 0.8f;
                feh.fadeSpeed = 0.035f;
                for (int m = 0; m < 10; m++)
                {
                    FadeExplosionClass newSmoke = new FadeExplosionClass(target.Center, orthToSwing.RotatedByRandom(1.2f) * Main.rand.NextFloat(0.5f, 2f) * 2f * bigSwingMultiplier);

                    newSmoke.size = (0.25f + Main.rand.NextFloat(-0.15f, 0.15f)) * bigSwingMultiplier;
                    feh.Smokes.Add(newSmoke);

                }
            }

            for (int fg = 0; fg < 10 * bigSwingMultiplier; fg++)
            {
                Vector2 randomStart = vec.RotatedByRandom(1) * 3f;
                Dust gd = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowPixelAlts>(), randomStart * Main.rand.NextFloat(0.3f, 1.35f) * 1.5f, newColor: new Color(255, 130, 0) * 0.85f, Scale: Main.rand.NextFloat(1f, 1.4f) * 0.45f);
                gd.alpha = 2;
                gd.scale *= bigSwingMultiplier;

                //gd.fadeIn = Main.rand.NextFloat(5f, 10f) + 30;
                //gd.alpha = 255;

            }

            for (int i = 0; i < 3 + Main.rand.Next(3); i++)
            {
                Vector2 v = vec.RotatedByRandom(2);
                Dust sa = Dust.NewDustPerfect(target.Center, DustID.PortalBoltTrail, v * Main.rand.NextFloat(2f, 6f), 0,
                    Color.Orange, Main.rand.NextFloat(0.4f, 0.7f) * 1.35f);
                sa.scale *= (bigSwingMultiplier - 0.10f);

                if (sa.velocity.Y > 0)
                    sa.velocity.Y *= -1;

                //sa.velocity += vec * 2f;
            }

            SoundEngine.PlaySound(SoundID.Item70 with { Pitch = -0.25f, Volume = 0.4f, MaxInstances = -1, PitchVariance = 0.25f }, Projectile.Center);

            SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/hero_fury_charm_burst") with { Pitch = .65f, PitchVariance = 0.2f, MaxInstances = -1, Volume = 0.6f };
            SoundEngine.PlaySound(style2, Projectile.Center);

            SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_explosive_trap_explode_1") with { PitchVariance = 0.15f, Pitch = 0.1f, Volume = 0.6f };
            SoundEngine.PlaySound(style3, Projectile.Center);

        }

        float easingProgress = 0;
        public float getProgress(float x) //From 0 to 1
        {
            float toReturn = 0f;
            #region easeExpo
            
            //pre 0.5
            if (x <= 0.5f)
            {
                toReturn = (float)(Math.Pow(2.21f, (20 * x) - 10)) / 2;
            }
            else if (x > 0.5)
            {
                toReturn = (float)(2 - ((Math.Pow(2.21f, (-20 * x) + 10)))) / 2;
            }

            //post 0.5
            if (x == 0)
                toReturn = 0;
            if (x == 1)
                toReturn = 1;

            return toReturn;
            

            #endregion;

            #region easeCircle
            /*
            if (x < 0.5)
            {
                toReturn = (float)(1 - Math.Sqrt(1 - Math.Pow(2 * x, 2))) / 2;
            }
            else
            {
                toReturn = (float)(Math.Sqrt(1 - Math.Pow(-2 * x + 2, 2)) + 1) / 2;
            }

            return toReturn;
            */
            #endregion

            #region easeOutBack
            return (float)(x < 0.5 ? 16 * x * x * x * x * x : 1 - Math.Pow(-2 * x + 2, 5) / 2);
            #endregion
        }

        public override float WidthFunction(float progress)
        {
            //return mytrailWidth;
            float num = 1f;
            float lerpValue = Utils.GetLerpValue(0f, 0.4f, progress, clamped: true);
            num *= 1f - (1f - lerpValue) * (1f - lerpValue);
            return MathHelper.Lerp(0f, 30f, num) * (mytrailWidth / 28) * (bigSwing ? 1.1f : 1); // 0.3f
            
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Main.player[Projectile.owner].MountedCenter;
            Vector2 end = start + currentAng.ToRotationVector2() * (95f * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 50f * Projectile.scale, ref collisionPoint);
        }

    }

    public class BurningJealousyGuard : ModProjectile
    {

        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Burning Jealousy");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;


            Projectile.timeLeft = 4000;
            Projectile.width = Projectile.height = 85;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.scale = 1f;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

        int timer = 0;
        float Angle = 0;

        bool fading = false;
        float fadeCounter = 0;
        float symbolIntensity_Sword = 0;

        float fadeInVal = 0.05f;

        float x1 = 0f;
        float y2 = 0f;

        Projectile symbol = null;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (symbol == null)
                symbol = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<BlockFX>(), 0, 0, Main.myPlayer);
            else if (symbol.ModProjectile is BlockFX fx)
            {
                fx.symbolIntensity = symbolIntensity_Sword;
                fx.fadeInBonus = fadeInVal;
            }
            if (timer == 0)
            {
                fadeInVal = 0.05f;

                x1 = player.velocity.X * 0.015f;
                y2 = player.velocity.Y * 0.015f * (player.direction == 1 ? -1f : 1f);
            }

            player.heldProj = Projectile.whoAmI;
            #region arm shit

            if (Projectile.owner == Main.myPlayer)
            {
                Angle = (Main.MouseWorld - player.Center).ToRotation();
            }

            player.itemRotation = Angle;
            player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

            Vector2 frontHandPos = Main.GetPlayerArmPosition(Projectile);

            float xOffset = (player.velocity.X * 0.04f) * player.direction;
            float yOffset = (player.velocity.Y * 0.02f * (player.direction == 1 ? -1f : 1f)) * player.direction;

            x1 = MathHelper.Lerp(x1, player.velocity.X * 0.015f, 0.1f);
            y2 = MathHelper.Lerp(y2, player.velocity.Y * 0.015f * (player.direction == 1 ? -1f : 1f), 0.1f);

            float desiredArmAngle = (0.3f - MathHelper.PiOver2 + xOffset + yOffset) * Projectile.direction;

            Projectile.rotation = Projectile.rotation.AngleLerp(desiredArmAngle, fadeInVal < 1 ? 0.2f : 0.1f);

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation);
            #endregion

            if (!player.active || player.dead || player.CCed || player.noItems)
            {
                player.GetModPlayer<BurningJealousyPlayer>().gaurding = false;
                Projectile.Kill();
            }

            Projectile.Center = player.Center + new Vector2(20 * Projectile.direction,0);
            player.itemTime = 10;
            player.itemAnimation = 10;


            if (!Main.mouseRight)
            {
                fading = true;
            }

            if (fading)
            {
                symbolIntensity_Sword = Math.Clamp(MathHelper.Lerp(symbolIntensity_Sword, -0.5f, 0.12f), 0f, 1f);
                fadeCounter += 0.07f;

                if (fadeCounter >= 1)
                {
                    player.itemTime = 1;
                    player.itemAnimation = 1;
                    player.GetModPlayer<BurningJealousyPlayer>().gaurding = false;

                    if (symbol != null)
                        symbol.active = false;
                    Projectile.active = false;

                }
            }
            else
            {
                if (timer > 7)
                {
                    player.statDefense = player.statDefense + 30;
                    player.GetModPlayer<BurningJealousyPlayer>().gaurding = true;
                }
                symbolIntensity_Sword = Math.Clamp(MathHelper.Lerp(symbolIntensity_Sword, 1.5f, 0.07f), 0f, 1f);
            }

            if (player.GetModPlayer<BurningJealousyPlayer>().justHit)
            {

                player.GetModPlayer<BurningJealousyPlayer>().justHit = false;
                player.GetModPlayer<BurningJealousyPlayer>().gaurding = false;

                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_explosive_trap_explode_1") with { PitchVariance = 1.16f, };
                SoundEngine.PlaySound(style, player.Center);
                
                int a = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<BurningJealousyPulse>(), Projectile.damage * 2, 0, player.whoAmI);

                SkillStrikeUtil.setSkillStrike(Main.projectile[a], 2f, 1000, 0f, 0f);

                
                if (symbol != null)
                    symbol.active = false;
                Projectile.active = false;
            }

            if (timer > 0)
                fadeInVal = Math.Clamp(MathHelper.Lerp(fadeInVal, 1.25f, 0.08f), 0f, 1f);

            timer++;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Color glowMaskCol = Color.White;
            if (fading)
            {
                lightColor = Color.Lerp(lightColor, Color.Black * 0.1f, fadeCounter);
                glowMaskCol = Color.Lerp(Color.White, Color.Black * 0.1f, fadeCounter);
            }
            else
            {
                lightColor = Color.Lerp(lightColor, Color.Black * 0.2f, 1f - fadeInVal);
                glowMaskCol = Color.Lerp(Color.White, Color.Black * 0.2f, 1f - fadeInVal);
            }

            Player p = Main.player[Projectile.owner];
            Texture2D Sword = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousy");
            Texture2D Glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/BurningJealousyGlow");

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation);

            //NewDraw
            Vector2 origin = p.direction == 1 ? new Vector2(4, 4) : new Vector2(4, Sword.Height - 4);
            float rotationOffset = 0;
            SpriteEffects effects = p.direction == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Vector2 otherOffset = new Vector2(0, Main.player[Projectile.owner].gfxOffY);
            float rot = p.direction == 1 ? 2f : 2f - (MathF.PI / 4f);

            float scale = Projectile.scale + ((1f - fadeInVal) * 0.15f);

            Main.spriteBatch.Draw(Sword, armPosition - Main.screenPosition + otherOffset, null, lightColor, rot + rotationOffset + x1, origin, scale, effects, 0f);
            Main.spriteBatch.Draw(Glow,  armPosition - Main.screenPosition + otherOffset, null, glowMaskCol * 0.85f, rot + rotationOffset + x1, origin, scale, effects, 0f);

            float sinVal = (float)(Math.Sin(Main.timeForVisualEffects * 0.05f));
            Main.spriteBatch.Draw(Glow, armPosition - Main.screenPosition + otherOffset + Main.rand.NextVector2Circular(1.5f, 1.5f), null, glowMaskCol with { A = 0 } * (0.5f + (sinVal * 0.25f)), rot + rotationOffset + x1, origin, scale, effects, 0f);

            return false;
        }
    }

    public class BlockFX : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Block");
        }
         
        public override void SetDefaults()
        {
            Projectile.timeLeft = 4000; 
            Projectile.width = Projectile.height = 85;
            Projectile.scale = 0f;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

        int timer = 0;
        public float fadeCounter = 0;
        public float symbolIntensity = 0;
        public float fadeInBonus = 0f;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.Center = player.MountedCenter;

            Projectile.scale = (symbolIntensity);

            timer++;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            float offset = 0;

            offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.15f;

            float offset2 = (float)Math.Sin(Main.GlobalTimeWrappedHourly) * 0.15f;

            float scale = (1.55f + (offset * symbolIntensity)) * MathHelper.Lerp(1.8f, 1f, symbolIntensity);
            float scale2 = (1.2f + (offset * symbolIntensity)) * MathHelper.Lerp(1.8f, 1f, symbolIntensity);
            float colIntensity = Math.Clamp(symbolIntensity * symbolIntensity, 0f, 1f);


            Vector2 center = (Projectile.Center - Main.screenPosition) + new Vector2(0, Main.player[Projectile.owner].gfxOffY);
            Texture2D Symbol = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Misc/Melee/BlockSymbol");
            Main.spriteBatch.Draw(Symbol, new Vector2((int)center.X, (int)center.Y), null, Color.Black * 0.6f * colIntensity, offset2, Symbol.Size() / 2, scale, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            Color ReddishOrange = new Color(255, 80, 0);
            Main.spriteBatch.Draw(Symbol, new Vector2((int)center.X, (int)center.Y), null, ReddishOrange * colIntensity, offset2, Symbol.Size() / 2, scale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Symbol, new Vector2((int)center.X, (int)center.Y), null, Color.Gold * colIntensity, offset2, Symbol.Size() / 2, scale2, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class BurningJealousyPlayer : ModPlayer
    {
        public bool gaurding = false;
        public bool justHit = false;
        public bool notTile = false;
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (gaurding)
            {
                Player player = Main.player[Main.myPlayer];

                int swingDir = Main.MouseWorld.X < player.Center.X ? 0 : 1;

                Projectile a = Projectile.NewProjectileDirect(null, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX), ModContent.ProjectileType<BurningJealousyHeldProj>(), (int)player.GetTotalDamage(DamageClass.Melee).ApplyTo(140), 4, Main.myPlayer, swingDir);
                if (a.ModProjectile is BurningJealousyHeldProj sword)
                {
                    sword.bigSwing = true;
                }

                SkillStrikeUtil.setSkillStrike(a, 3f, 1000, 0f, 0f);


                player.ChangeDir(Main.MouseWorld.X > player.Center.X ? 1 : -1);
                justHit = true;
                gaurding = false;

            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (gaurding)
            {
                int itemDamage = Player.inventory[Player.selectedItem].damage;
                int bigSwingDamage = (int)(itemDamage * 2.5f);

                Player player = Main.player[Main.myPlayer];

                int swingDir = Main.MouseWorld.X > player.Center.X ? 0 : 1;

                Projectile a = Projectile.NewProjectileDirect(null, player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX), ModContent.ProjectileType<BurningJealousyHeldProj>(), (int)player.GetTotalDamage(DamageClass.Melee).ApplyTo(bigSwingDamage), KnockbackTiers.Average, Main.myPlayer, swingDir);
                if (a.ModProjectile is BurningJealousyHeldProj sword)
                {
                    sword.bigSwing = true;
                }
                SkillStrikeUtil.setSkillStrike(a, 3f, 1000, 0f, 0f);

                player.ChangeDir(Main.MouseWorld.X > player.Center.X ? 1 : -1);
                justHit = true;
                gaurding = false;
            }
            
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (gaurding)
            {
                Player player = Main.player[Main.myPlayer];
                player.statDefense = player.statDefense + 30;
            }
        }
    }

    public class BurningJealousyPulse : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";


        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Burning Explosion");
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;

            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.scale = 0.1f;
            Projectile.timeLeft = 300;
            Projectile.penetrate = -1;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false; 
        }

        //Damage is done through a strike
        public override bool? CanDamage() => false;


        int timer = 0;
        bool firstFrame = true;
        float colorIntensity = 1f;
        float scale2 = 0;
        float scale3 = 0;

        float randomRot = 0;
        public override void AI()
        {
            if (firstFrame)
            {
                randomRot = Main.rand.NextFloat(6.28f);
                //AOE
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && Vector2.Distance(Projectile.Center, Main.npc[i].Center) < 170f)
                    {
                        int Direction = 0;
                        if (Projectile.position.X - Main.npc[i].position.X < 0)
                            Direction = 1;
                        else
                            Direction = -1;

                        HitInfo myHit = new HitInfo();
                        myHit.Damage = (int)(Projectile.damage * 1f);
                        myHit.Knockback = Projectile.knockBack;
                        myHit.HitDirection = Direction;
                        myHit.DamageType = DamageClass.Melee;

                        Main.npc[i].StrikeNPC(myHit);

                        //Main.npc[i].AddBuff(ModContent.BuffType<EmberFire>(), 30);

                    }
                }

                firstFrame = false;

                Projectile.rotation = Main.rand.NextFloat(6.28f);

                //Spawn Dust
                ArmorShaderData dustShader2 = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");

                for (int i = 0; i < 30; i++)
                {
                    if (Main.rand.NextBool())
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(5, 5);
                        Dust gd = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<LineGlow>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.OrangeRed, 0.15f, 0.2f, 0f, dustShader2);
                        gd.fadeIn = 45 + Main.rand.NextFloat(-3f, 14f);
                        gd.scale *= Main.rand.NextFloat(0.9f, 2.1f);
                    }
                    else
                    {
                        Vector2 randomStart = Main.rand.NextVector2CircularEdge(6, 6);
                        Dust gd = GlowDustHelper.DrawGlowDustPerfect(Projectile.Center, ModContent.DustType<GlowCircleFlare>(), randomStart * Main.rand.NextFloat(0.65f, 1.35f), Color.Orange, 0.7f, 0.1f, 0f, dustShader2);
                        gd.fadeIn = 1;
                    }
                }
                
            }

            Projectile.scale = Math.Clamp(MathHelper.Lerp(Projectile.scale, 2.1f, 0.15f), 0f, 2f);
            scale2 = Math.Clamp(MathHelper.Lerp(scale2, 2.1f, 0.05f), 0f, 2f);
            scale3 = Math.Clamp(MathHelper.Lerp(scale3, 2.1f, 0.10f), 0f, 2f);

            Projectile.velocity = Vector2.Zero;

            if (timer > 10)
            {
                //Projectile.scale *= 0.9f;
                colorIntensity -= 0.12f;
                if (colorIntensity <= 0)
                    Projectile.active = false;
            }
            Projectile.rotation += 0.06f;

            timer++;
        }

        //OrangeRed, Orange, Gold, Gold, Wheat, White
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D sixStar = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/star_05");
            Texture2D circle = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/MagmaBall");
            Texture2D circle2 = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_05");
            Texture2D color = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Ember/color_burst_30");


            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.Black * 0.85f * colorIntensity, Projectile.rotation * 1.5f, circle2.Size() / 2, Projectile.scale * 0.75f, 0, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * 0.7f * colorIntensity, Projectile.rotation * 2f, sixStar.Size() / 2, Projectile.scale * 0.5f, 0, 0f);

            Main.spriteBatch.Draw(sixStar, Projectile.Center - Main.screenPosition, null, Color.OrangeRed * colorIntensity, Projectile.rotation, sixStar.Size() / 2, Projectile.scale * 0.75f, 0, 0f);
            Main.spriteBatch.Draw(circle, Projectile.Center - Main.screenPosition, null, Color.Orange * colorIntensity, Projectile.rotation, circle.Size() / 2, scale3 * 0.17f, 0, 0f);

            Main.spriteBatch.Draw(color, Projectile.Center - Main.screenPosition, null, Color.Gold * colorIntensity, randomRot, color.Size() / 2, Projectile.scale * 0.5f, 0, 0f);
            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.Wheat * 1f * colorIntensity, Projectile.rotation * 1.5f, circle2.Size() / 2, Projectile.scale * 0.5f, 0, 0f);
            Main.spriteBatch.Draw(circle2, Projectile.Center - Main.screenPosition, null, Color.White * 1f * colorIntensity, Projectile.rotation * 1.5f, circle2.Size() / 2, Projectile.scale * 0.25f, 0, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
