using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Content.Projectiles.Other;
using AerovelenceMod.Common.Globals.Players;
using System.Linq;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System.Threading;
using AerovelenceMod.Content.Items.Weapons.Aurora.Eos;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;
using System.IO;
using Terraria.Map;
using AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns.Skylight;
using static Terraria.NPC;
using AerovelenceMod.Common.Globals.SkillStrikes;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Magic.Ceroba
{

    //This weapon is a fucking atrocity codewise
    //I am sorry for anyone going through this that isn't me

    public class CerobaStaffItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 76;
            Item.knockBack = KnockbackTiers.Average;
            Item.mana = 12;
            Item.shootSpeed = 10;

            Item.useTime = 9; //13
            Item.useAnimation = 36; //13

            Item.reuseDelay = 24;

            Item.width = 62;
            Item.height = 62;
            Item.value = Item.sellPrice(0, 12, 9, 23);
            Item.rare = ItemRarities.PostPlantDungeon;

            Item.DamageType = DamageClass.Magic;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<CerobaFireBall>();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpectreBar, 12).
                AddIngredient(ItemID.SoulofLight, 6).
                AddIngredient(ItemID.GoldBar, 6).
                AddTile(TileID.MythrilAnvil).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.SpectreBar, 12).
                AddIngredient(ItemID.SoulofLight, 6).
                AddIngredient(ItemID.PlatinumBar, 6).
                AddTile(TileID.MythrilAnvil).
                Register();
        }

        public override void HoldItem(Player player)
        {
            bool checkOne = player.ownedProjectileCounts[ModContent.ProjectileType<CerobaIdleHeldProj>()] < 1;
            bool checkTwo = player.ownedProjectileCounts[ModContent.ProjectileType<CerobaSpinProj>()] < 1;
            if (checkOne && checkTwo)
                Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<CerobaIdleHeldProj>(), 0, 0, player.whoAmI);

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) 
        {
            Vector2 randomVel = velocity.RotatedByRandom(0.6f); // 0.75

            if (player.altFunctionUse == 2)
            {
                int cerobaSwing = Projectile.NewProjectile(null, player.Center, velocity, ModContent.ProjectileType<CerobaSpinProj>(), damage, 0, Main.myPlayer);
                int cerobaWard = Projectile.NewProjectile(null, Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<CerobaWard>(), (int)(damage * 2.5f), knockback, Main.myPlayer);
            }
            else
            {
                foreach (Projectile p in Main.projectile)
                {
                    //This check first to eliminate more projectiles
                    if (p.type == ModContent.ProjectileType<CerobaIdleHeldProj>())
                    {
                        if (p.owner == player.whoAmI)
                        {

                            (p.ModProjectile as CerobaIdleHeldProj).justShotValue = 1;
                            (p.ModProjectile as CerobaIdleHeldProj).starRot += MathHelper.PiOver4;
                            (p.ModProjectile as CerobaIdleHeldProj).starDir *= -1;

                            Vector2 spawnPos = (p.ModProjectile as CerobaIdleHeldProj).ProjSpawnPosition;

                            //Make sure dumb shit didnt happen
                            if (spawnPos.Distance(player.Center) < 100)
                            {
                                int FireBall = Projectile.NewProjectile(null, spawnPos, randomVel * 2f, ModContent.ProjectileType<CerobaFireBall>(), damage, knockback, Main.myPlayer);

                                for (int d = 0; d < 18; d++)
                                {
                                    int rand = Main.rand.Next(0, 3);
                                    Color col = rand == 0 ? Color.DeepPink : (rand == 1 ? Color.Pink : Color.HotPink);

                                    Dust dust = Dust.NewDustPerfect(spawnPos, ModContent.DustType<RoaParticle>(), randomVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.35f, 1f), newColor: col, Scale: Main.rand.NextFloat(0.5f, 1f));
                                    dust.fadeIn = Main.rand.Next(0, 4);
                                    dust.alpha = Main.rand.Next(0, 2);
                                }
                            }

                        }
                    }
                }
                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_etherian_portal_dryad_touch") with { Volume = .3f, Pitch = .81f, PitchVariance = .32f, MaxInstances = -1, };
                SoundEngine.PlaySound(style3, player.Center);
            }

            

            return false;
        }

    }

    public class CerobaIdleHeldProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.scale = 1f;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;

        public bool startDone = false;

        int timer = 0;
        float armRot = 0f;

        //Want to store player dir so we can kill the afterimages whenever the player turns around
        public int previousDir = 1;

        //This thing has been an abomination to try and work when facing both directions so excuse the dogshit code
        public override void AI()
        {
            if (timer == 0)
            {
                previousRotations = new List<float>();

                previousDir = Main.player[Projectile.owner].direction;

                if (startDone)
                {
                    Main.player[Projectile.owner].itemAnimation = 1;
                    Main.player[Projectile.owner].itemTime = 1;
                    Main.player[Projectile.owner].reuseDelay = 1;


                    timer = 40;
                }
                else
                {
                    SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/trident_twirl_01") with { Pitch = .29f, Volume = 0.25f, };
                    SoundEngine.PlaySound(style2, Projectile.Center);
                }
            }

            Projectile.timeLeft++;
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            int dir = player.direction;

            if (player.inventory[player.selectedItem].type != ModContent.ItemType<CerobaStaffItem>() && timer != 0 && !(startDone && timer == 40))
            {
                Projectile.Kill();
            }

            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            if (player.ownedProjectileCounts[ModContent.ProjectileType<CerobaSpinProj>()] >= 1 && timer != 0 && !(startDone && timer == 40))
                Projectile.active = false;


            if (dir == 1)
                FaceRightLogic(player);
            else
                FaceLeftLogic(player);

            if (previousDir != dir)
            {
                previousRotations.Clear();
            }

            int trailCount = 6;
            previousRotations.Add(Projectile.rotation);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (timer % 3 == 0 && animProgress <= 0.5f)
            {
                Vector2 vel = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 5f * dir;
                float scale = Main.rand.NextFloat(0.75f, 1f);
                int leaf2 = Dust.NewDust(Projectile.Center + Projectile.rotation.ToRotationVector2() * 30f, 1, 1, DustID.Enchanted_Pink, vel.X, vel.Y, 150, Color.HotPink with { A = 0 }, scale);
                Main.dust[leaf2].noGravity = true;
            }

            #region ProjectileSpawnPosition for Item

            float faceLeftBonus = dir == 1 ? 0f : MathHelper.PiOver4 + MathHelper.Pi;
            float projSpawnRotVal = Projectile.rotation + faceLeftBonus + (dir == 1 ? 0f : -MathHelper.PiOver2);
            Vector2 moveOff = new Vector2(player.velocity.X * 0.02f, player.velocity.Y * -0.02f);

            Vector2 armPos = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, armRot) + moveOff;

            ProjSpawnPosition = armPos + new Vector2(21f, -21f).RotatedBy(projSpawnRotVal);

            #endregion
            justShotValue = MathHelper.Clamp(MathHelper.Lerp(justShotValue, -0.5f, 0.1f), 0f, 1f);
            //starRot += 0.02f * dir;

            timer++;
        }

        //SHUT UP SHUT UP SHUT UP SHUT UP SHUT UP SHUT UP
        public void FaceRightLogic(Player player)
        {
            float progress = Utils.GetLerpValue(0f, 1f, timer / 40f, true);

            float xOffset = (player.velocity.X * 0.02f);
            float yOffset = player.velocity.Y * -0.02f;

            float armGoal = (MathHelper.TwoPi - 0.85f) + xOffset + yOffset;

            armRot = MathHelper.Lerp(MathHelper.PiOver2, armGoal, Easings.easeOutCubic(progress));

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

            Projectile.Center = player.Center;

            float goalRot = (-MathHelper.PiOver4 + 1.15f);
            Projectile.rotation = MathHelper.Lerp(goalRot * -30, (goalRot + ((yOffset + xOffset) * 0.5f)), Easings.easeOutCubic(progress)) + (justShotValue * -0.125f);

            animProgress = progress;
        }

        public void FaceLeftLogic(Player player)
        {
            float progress = Utils.GetLerpValue(0f, 1f, timer / 40f, true);

            float xOffset = -player.velocity.X * -0.02f;
            float yOffset = player.velocity.Y * +0.02f;

            float armGoal = (MathHelper.TwoPi - 0.7f + MathHelper.PiOver2) + xOffset + yOffset;

            armRot = MathHelper.Lerp(MathHelper.Pi + (MathHelper.PiOver2 * 0.75f), armGoal - MathHelper.TwoPi, Easings.easeOutCubic(progress));

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRot);

            Projectile.Center = player.Center;

            float goalRot = (-MathHelper.PiOver4 + 1.15f * player.direction);
            Projectile.rotation = MathHelper.Lerp(goalRot * -5.85f, (goalRot + ((-yOffset + -xOffset) * 0.5f)) * -1, Easings.easeOutCubic(progress)) + (justShotValue * 0.125f);

            animProgress = progress;
        }

        public float justShotValue = 0f;

        float alpha = 1f;
        float animProgress = 0f;
        public List<float> previousRotations;
        public float starRot = 0f;
        public int starDir = 1;

        public Vector2 ProjSpawnPosition = Vector2.Zero;
        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0)
                return false;

            string path = "Content/Items/Weapons/Misc/Magic/Ceroba/";
            Texture2D Staff = Mod.Assets.Request<Texture2D>(path + "CerobaStaffProj").Value;
            Texture2D Stick = Mod.Assets.Request<Texture2D>(path + "CerobaStaffStick").Value;

            Texture2D White = Mod.Assets.Request<Texture2D>(path + "CerobaStaffWhiteBell").Value;
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>(path + "CerobaStaffGlowMask").Value;

            Texture2D Swirl = Mod.Assets.Request<Texture2D>("Assets/TrailImages/TerraOrbC").Value;
            Texture2D SwirlD = Mod.Assets.Request<Texture2D>("Assets/TrailImages/TerraSwingD").Value;
            Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/TrailImages/CrispStarPMA").Value;


            Player player = Main.player[Projectile.owner];
            Vector2 origin = Staff.Size() / 2;
            Vector2 bellOff = new Vector2(21f, -21f).RotatedBy(Projectile.rotation);

            float xOffset = (player.velocity.X * 0.02f);// * player.direction;
            float yOffset = player.velocity.Y * -0.02f;// * player.direction;
            Vector2 moveOff = new Vector2(xOffset, yOffset);

            Vector2 armPos = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, armRot) - Main.screenPosition + moveOff + new Vector2(0f, player.gfxOffY);
            int dir = player.direction;

            SpriteEffects spriteFX = dir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float faceLeftBonus = dir == 1 ? 0f : MathHelper.PiOver4 + MathHelper.Pi;

            float swirlAlpha = (1f - animProgress);
            float swirlRot1 = Projectile.rotation - 1.5f;
            float swirlRot2 = Projectile.rotation + MathF.PI - 0.25f;

            Main.EntitySpriteDraw(Swirl, armPos, null, Color.HotPink with { A = 0 } * Easings.easeInQuad(swirlAlpha) * 0.4f, swirlRot1 + (dir == 1 ? 0f : 0.5f), Swirl.Size() / 2, Projectile.scale * 0.5f, spriteFX);
            Main.EntitySpriteDraw(Swirl, armPos, null, Color.DeepPink with { A = 0 } * Easings.easeInQuad(swirlAlpha) * 0.4f, swirlRot2 + (dir == 1 ? 0f : 4f), Swirl.Size() / 2, Projectile.scale * 0.5f, spriteFX);

            Main.EntitySpriteDraw(SwirlD, armPos, null, Color.HotPink with { A = 0 } * Easings.easeInQuad(swirlAlpha) * 0.2f, swirlRot1 + (dir == 1 ? 0f : 0.5f), SwirlD.Size() / 2, Projectile.scale * 0.5f, spriteFX);
            Main.EntitySpriteDraw(SwirlD, armPos, null, Color.DeepPink with { A = 0 } * Easings.easeInQuad(swirlAlpha) * 0.2f, swirlRot2 + (dir == 1 ? 0f : 4f), SwirlD.Size() / 2, Projectile.scale * 0.5f, spriteFX);


            if (previousRotations != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    Color col = Color.Brown * progress;

                    float adjustedRot = previousRotations[i] + faceLeftBonus + (dir == 1 ? 0f : -MathHelper.PiOver2);

                    Main.EntitySpriteDraw(Stick, armPos, null, col * 0.55f * alpha * swirlAlpha, adjustedRot, origin, Projectile.scale, SpriteEffects.None);

                    Main.EntitySpriteDraw(White, armPos + new Vector2(21f, -21f).RotatedBy(adjustedRot), null, Color.Gold with { A = 0 } * 0.3f * alpha * swirlAlpha * Easings.easeOutCirc(progress), adjustedRot, White.Size() / 2f, Projectile.scale, SpriteEffects.None);
                }

            }

            #region Ribbons
            Texture2D RibbonBottom = Mod.Assets.Request<Texture2D>(path + "RibbonBottom").Value;
            Texture2D RibbonTop = Mod.Assets.Request<Texture2D>(path + "RibbonTop").Value;

            Vector2 bottomOrigin = new Vector2(9f, 0f);
            Vector2 topOrigin = new Vector2(RibbonTop.Width, 8f);

            float scalePercent = Easings.easeOutQuad(0.35f + (0.65f * animProgress));
            SpriteEffects ribbonSpriteFXTop = dir == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            SpriteEffects ribbonSpriteFXBottom = dir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 ribbonBottomPos = armPos + new Vector2((dir == 1 ? 15f : 14f) * player.direction, (dir == 1 ? -11f : -11.5f)).RotatedBy(Projectile.rotation + faceLeftBonus);
            Vector2 ribbonTopPos = armPos + new Vector2(11f * player.direction, -15f).RotatedBy(Projectile.rotation + faceLeftBonus);

            float ribbonLeftBonusBottom = dir == 1 ? 0f : 0f;
            float ribbonLeftBonusTop = dir == 1 ? 0f : -3.14f;

            float ribbonBottomSinRot = dir == 1 ? 
                (MathF.Sin((float)Main.timeForVisualEffects * 0.03f) * 0.15f) + yOffset + (xOffset > 0 ? xOffset : 0f)
                : (MathF.Sin((float)Main.timeForVisualEffects * 0.03f) * 0.15f) + (-yOffset * 0.5f)  + (xOffset < 0 ? xOffset : 0f);
            
            float ribbonTopSinRot = (MathF.Sin((float)Main.timeForVisualEffects * 0.03f) * 0.05f) + (yOffset * 0.5f * dir) + xOffset;

            Vector2 bottomScale = dir == 1 ?
                new Vector2(scalePercent - (ribbonBottomSinRot < 0 ? ribbonBottomSinRot : ribbonBottomSinRot * 0.5f) - (justShotValue * 0.1f), 1f)
                : new Vector2(scalePercent - (ribbonBottomSinRot > 0 ? -ribbonBottomSinRot * 0.75f : -ribbonBottomSinRot * 0.5f) - (justShotValue * 0.1f), 1f);

            Vector2 topScale = new Vector2(1f, scalePercent - (ribbonTopSinRot < 0 ? ribbonTopSinRot : ribbonTopSinRot * 0.5f) - (justShotValue * 0.1f));
           

            Main.EntitySpriteDraw(RibbonBottom, ribbonBottomPos, null, lightColor * alpha, Projectile.rotation + ribbonBottomSinRot + faceLeftBonus + ribbonLeftBonusBottom, bottomOrigin, bottomScale * Projectile.scale, ribbonSpriteFXBottom);
            Main.EntitySpriteDraw(RibbonTop, ribbonTopPos, null, lightColor * alpha, Projectile.rotation + ribbonTopSinRot + faceLeftBonus + ribbonLeftBonusTop, topOrigin, topScale * Projectile.scale, ribbonSpriteFXTop);

            //Glow Ribbons
            float glowRibbonBonusScale = 1f + (justShotValue * 0.05f);
            Main.EntitySpriteDraw(RibbonBottom, ribbonBottomPos, null, Color.Pink with { A = 0 } * justShotValue * 0.75f, Projectile.rotation + ribbonBottomSinRot + faceLeftBonus + ribbonLeftBonusBottom, bottomOrigin, bottomScale * Projectile.scale * glowRibbonBonusScale, ribbonSpriteFXBottom);
            Main.EntitySpriteDraw(RibbonTop, ribbonTopPos, null, Color.Pink with { A = 0 } * justShotValue * 0.75f, Projectile.rotation + ribbonTopSinRot + faceLeftBonus + ribbonLeftBonusTop, topOrigin, topScale * Projectile.scale * glowRibbonBonusScale, ribbonSpriteFXTop);;

            #endregion


            //Draw Underglow
            Color underGlowColor = Color.Lerp(Color.Gold with { A = 0 } * 0.2f, Color.HotPink with { A = 0 }, justShotValue);
            float underGlowExtraScale = justShotValue * 0.2f;

            float underGlowAdjustedRot = Projectile.rotation + faceLeftBonus + (dir == 1 ? 0f : -MathHelper.PiOver2);
            for (int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(White, armPos + Main.rand.NextVector2Circular(1.5f, 1.5f) + new Vector2(21f, -21f).RotatedBy(underGlowAdjustedRot), null, underGlowColor * alpha, underGlowAdjustedRot, White.Size() / 2, Projectile.scale + underGlowExtraScale, SpriteEffects.None);
            }

            //Draw Stars
            float starAlpha = justShotValue * alpha;
            float starScale = 1f - (justShotValue * 0.7f);
            Color middleGroundPink = Color.Lerp(Color.DeepPink, Color.HotPink, 0.25f);
            Main.EntitySpriteDraw(Star, armPos + new Vector2(21f, -21f).RotatedBy(underGlowAdjustedRot), null, middleGroundPink with { A = 0 } * starAlpha, starRot, Star.Size() / 2, starScale * 1.25f, SpriteEffects.None);
            Main.EntitySpriteDraw(Star, armPos + new Vector2(21f, -21f).RotatedBy(underGlowAdjustedRot), null, Color.HotPink with { A = 0 } * starAlpha, starRot, Star.Size() / 2, starScale, SpriteEffects.None);

            Main.EntitySpriteDraw(Staff, armPos, null, lightColor * alpha * Easings.easeOutQuart(animProgress), Projectile.rotation + faceLeftBonus, origin, Projectile.scale, spriteFX);
            Main.EntitySpriteDraw(Glowmask, armPos, null, Color.White * alpha * Easings.easeOutQuart(animProgress), Projectile.rotation + faceLeftBonus, origin, Projectile.scale, spriteFX);

            return false;
        }

    }

    public class CerobaFireBall : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        int timer = 0;
        public int advancer = 0;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = Projectile.height = 30;
            Projectile.ignoreWater = false;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.tileCollide = true;
            Projectile.timeLeft = 170;

        }

        public float accelTime = 40f;

        float alpha = 0f;
        float pulseIntensity = 0f;

        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.ai[0] = Projectile.velocity.Length();

                previousRotations = new List<float>();
                previousPostions = new List<Vector2>();

                Projectile.rotation = Projectile.velocity.ToRotation();

                pulseIntensity = 1f;
            }

            if (timer > 5)
            {
                if (timer % 3 == 0 && Main.rand.NextBool())
                {

                    Color col = Main.rand.NextBool() ? Color.DeepPink : Color.HotPink;

                    Vector2 randomStart = Main.rand.NextVector2Circular(3.5f, 3.5f) * 1f;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: col, Scale: Main.rand.NextFloat(0.3f, 0.4f));
                    dust.velocity += Projectile.velocity * 0.25f;

                    dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                        rotPower: 0.15f, preSlowPower: 0.9f, timeBeforeSlow: 0, postSlowPower: 0.9f, velToBeginShrink: 3f, fadePower: 0.85f, shouldFadeColor: false);
                    
                }
            }

            NPC target = Main.npc.Where(n => n.CanBeChasedBy() && n.Distance(Projectile.Center) < 1000f && (Collision.CanHitLine(Projectile.Center, 1, 1, n.Center, 1, 1) || Collision.CanHitLine(Main.player[Projectile.owner].Center, 1, 1, n.Center, 1, 1))).OrderBy(n => n.Distance(Projectile.Center)).FirstOrDefault();

            #region myHoming
            
            if (target != null && timer > 15 && timer < 120)
            {
                float homingVal = Utils.GetLerpValue(0f, 1f, (timer - 10f) / 80f, true);

                float extraHoming = target.Distance(Projectile.Center) < 35 ? 0.09f : 0f;

                Projectile.velocity = Vector2.Normalize(Vector2.Lerp(Projectile.velocity,
                    Vector2.Normalize(target.Center - Projectile.Center) * Projectile.velocity.Length(), homingVal * 0.4f + extraHoming)) * Projectile.velocity.Length();

                if (timer < 75)
                    Projectile.velocity *= 1.01f;
                else if (timer > 75)
                    Projectile.velocity *= 0.97f;
            }
            
            #endregion

            #region Photonic
            if (target != null && timer > 0 && false)
            {
                float homingStrength = RemapEased((timer - 0f) * 0.25f, 1, 20, 0, .3f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center) * Projectile.ai[0] * 1.5f, homingStrength);

            }
            #endregion
            Projectile.rotation = Projectile.velocity.ToRotation();


            int trailCount = 14;
            previousRotations.Add(Projectile.rotation);
            previousPostions.Add(Projectile.Center);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPostions.Count > trailCount)
                previousPostions.RemoveAt(0);

            pulseIntensity = Math.Clamp(MathHelper.Lerp(pulseIntensity, -0.25f, 0.03f), 0f, 2f);
            alpha = Math.Clamp(MathHelper.Lerp(alpha, 1.25f, 0.06f), 0f, 1f);

            Lighting.AddLight(Projectile.Center, Color.DeepPink.ToVector3() * 0.65f);

            timer++;
        }

        //Photonic0
        public float RemapEased(float fromValue, float fromMin, float fromMax, float toMin, float toMax, bool clamp = true)
        {
            return MathHelper.Lerp(toMin, toMax, Easings.easeInOutSine(Utils.GetLerpValue(fromMin, fromMax, fromValue, clamp)));
        }

        public List<float> previousRotations;
        public List<Vector2> previousPostions;

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer <= 0) return false;
            Texture2D FireBall = Mod.Assets.Request<Texture2D>("Assets/TrailImages/FireBallBlur").Value;
            Texture2D FireBallPixel = Mod.Assets.Request<Texture2D>("Assets/TrailImages/Extra_91").Value;

            //Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/ImpactTextures/star_07").Value;
            Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/TrailImages/CrispStarPMA").Value;


            Texture2D Buster = Mod.Assets.Request<Texture2D>("Assets/TrailImages/BusterGlow").Value;
            Texture2D Glow = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle128PMA").Value;

            Vector2 off = (Projectile.rotation).ToRotationVector2() * 10f * Projectile.scale;

            Main.EntitySpriteDraw(Glow, Projectile.Center - Main.screenPosition + off * 1.5f, null, Color.HotPink with { A = 0 } * alpha * 0.5f, Projectile.rotation + MathHelper.PiOver2, Glow.Size() / 2f, Projectile.scale * 1f, SpriteEffects.None);

            float starScale = alpha * 2.5f * Projectile.scale;
            Main.EntitySpriteDraw(Star, Projectile.Center - Main.screenPosition + off * 2f, null, Color.DeepPink with { A = 0 } * (1f - alpha) * 1f, Projectile.rotation + MathHelper.PiOver2 + (timer * 0.15f), Star.Size() / 2f, starScale, SpriteEffects.None);
            Main.EntitySpriteDraw(Star, Projectile.Center - Main.screenPosition + off * 2f, null, Color.White with { A = 0 } * (1f - alpha) * 2f, Projectile.rotation + MathHelper.PiOver2 + (timer * 0.15f), Star.Size() / 2f, starScale * 0.75f, SpriteEffects.None);


            Color outerCol = Color.HotPink * 0.5f;//Color.Lerp(Color.DeepSkyBlue * 0.5f, Color.SkyBlue with { A = 0 } * 0.8f, pulseIntensity);
            float scale = MathHelper.Lerp(1f, 1.25f, pulseIntensity);
            for (int i = 0; i < 1; i++)
            {
                Main.EntitySpriteDraw(FireBall, Projectile.Center - Main.screenPosition, null, outerCol with { A = 0 } * alpha, Projectile.rotation + MathHelper.PiOver2, FireBall.Size() / 2f, Projectile.scale * 1f * scale, SpriteEffects.None);
            }

            #region after image
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    float size = (1f - (progress * 0.5f)) * Projectile.scale;

                    float colVal = progress;

                    Color col = Color.Lerp(Color.Pink * 0.75f, Color.HotPink, progress) * progress;

                    float size2 = (1f - (progress * 0.15f)) * Projectile.scale;
                    Main.EntitySpriteDraw(FireBallPixel, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 0.85f * alpha * colVal,
                            previousRotations[i] + MathHelper.PiOver2, FireBallPixel.Size() / 2f, size2, SpriteEffects.None);

                    Vector2 vec2Scale = new Vector2(0.25f, 1.15f) * size;

                    Main.EntitySpriteDraw(FireBall, previousPostions[i] - Main.screenPosition, null, col with { A = 0 } * 1.25f * alpha * colVal,
                            previousRotations[i] + MathHelper.PiOver2, FireBall.Size() / 2f, vec2Scale * 1.5f, SpriteEffects.None);
                }

            }
            #endregion

            Vector2 v2scale = new Vector2(1.25f, 1f);

            Main.EntitySpriteDraw(FireBall, Projectile.Center - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f), null, Color.DeepPink with { A = 0 } * alpha, Projectile.rotation + MathHelper.PiOver2, FireBall.Size() / 2f, Projectile.scale * v2scale, SpriteEffects.None);

            Main.EntitySpriteDraw(FireBall, Projectile.Center - Main.screenPosition + off, null, Color.White with { A = 0 } * 1f * alpha, Projectile.rotation + MathHelper.PiOver2, FireBall.Size() / 2f, v2scale * Projectile.scale * 0.5f, SpriteEffects.None);


            return false;
        }

        public override void OnKill(int timeLeft)
        {
            //On trail
            if (previousRotations != null && previousPostions != null)
            {
                for (int i = 0; i < previousRotations.Count; i += 1)
                {
                    //Color col = Main.rand.NextBool() ? Color.DeepPink : Color.HotPink;

                    //Vector2 randomStart = Main.rand.NextVector2Circular(1f, 1f) * 1f;
                    //Dust dust = Dust.NewDustPerfect(previousPostions[i], ModContent.DustType<GlowPixelCross>(), randomStart, newColor: col, Scale: Main.rand.NextFloat(0.2f, 0.45f));
                    //dust.velocity += previousRotations[i].ToRotationVector2() * Projectile.velocity.Length() * 0.15f;

                    //dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    //    rotPower: 0.15f, preSlowPower: 0.95f, timeBeforeSlow: 12, postSlowPower: 0.92f, velToBeginShrink: 3f, fadePower: 0.91f, shouldFadeColor: false);

                    
                    if (!Main.rand.NextBool(2))
                    {
                        int dust2 = Dust.NewDust(previousPostions[i], 1, 1, ModContent.DustType<GlowPixelRise>(), Scale: 0.35f + Main.rand.NextFloat(-0.25f, 0.1f), newColor: Main.rand.NextBool() ? Color.HotPink : Color.Pink);
                        Main.dust[dust2].velocity *= 0.35f;
                        Main.dust[dust2].velocity += previousRotations[i].ToRotationVector2() * Projectile.velocity.Length() * 0.25f;
                        Main.dust[dust2].alpha = 5;
                        Main.dust[dust2].noLight = false;
                    }
                }

            }

            //Impact
            for (int i = 0; i < 12; i++)
            {
                Vector2 randomStart = Main.rand.NextVector2Circular(3.5f, 3.5f) * 1f;
                Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.35f, 0.65f));
                //dust.velocity += Projectile.velocity * 0.25f;

                dust.noLight = false;
                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.15f, preSlowPower: 0.99f, timeBeforeSlow: 12, postSlowPower: 0.92f, velToBeginShrink: 3f, fadePower: 0.91f, shouldFadeColor: false);
            }

            for (int i = 0; i < 15; i++)
            {
                Color col = Main.rand.NextBool() ? Color.DeepPink : Color.HotPink;
                Vector2 vel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1f, 9f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<RoaParticle>(), vel, newColor: col, Scale: Main.rand.NextFloat(0.75f, 1.5f));
                d.fadeIn = Main.rand.Next(0, 4);
                d.alpha = Main.rand.Next(0, 2);
                d.noLight = false;

            }

            //Light Dust
            Dust softGlow = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SoftGlowDust>(), Vector2.Zero, newColor: Color.DeepPink, Scale: 0.35f);

            softGlow.customData = DustBehaviorUtil.AssignBehavior_SGDBase(timeToStartFade: 3, timeToChangeScale: 0, fadeSpeed: 0.9f, sizeChangeSpeed: 0.95f, timeToKill: 10,
                overallAlpha: 0.15f, DrawWhiteCore: false, 1f, 1f);

            //Sound
            SoundStyle style = new SoundStyle("Terraria/Sounds/Item_14") with { Pitch = .3f, MaxInstances = -1, PitchVariance = 0.2f, Volume = 0.3f };
            SoundEngine.PlaySound(style, Projectile.Center);

            string randomSound = Main.rand.NextBool() ? "2" : "1";

            SoundStyle style4 = new SoundStyle("Terraria/Sounds/Custom/dd2_flameburst_tower_shot_" + randomSound) with { Pitch = .25f, PitchVariance = .32f, MaxInstances = -1, Volume = 0.35f };
            SoundEngine.PlaySound(style4, Projectile.Center);

            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_62") with { Volume = .23f, Pitch = .51f, PitchVariance = .27f, MaxInstances = -1 };
            SoundEngine.PlaySound(style2, Projectile.Center);

            //SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_etherian_portal_dryad_touch") with { Volume = .44f, Pitch = .81f, PitchVariance = .32f, MaxInstances = -1, }; 
            //SoundEngine.PlaySound(style3, Projectile.Center);

            base.OnKill(timeLeft);
        }

        public override void ModifyHitNPC(NPC target, ref HitModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<CerobaMark>())) {

                SkillStrikeUtil.setSkillStrike(Projectile, 1.3f, impactVolume: 0.5f, impactScale: 0f);

                int a = Projectile.NewProjectile(null, Projectile.Center, Projectile.velocity, ModContent.ProjectileType<CerobaSkillStrikeFX>(), 0, 0, Main.myPlayer);
                Main.projectile[a].scale = 0.75f;

                modifiers.FinalDamage *= 1f;
            }
            
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void OnHitNPC(NPC target, HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }
    }

    public class CerobaWard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        int timer = 0;
        public int advancer = 0;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;

            Projectile.width = Projectile.height = 30;
            Projectile.ignoreWater = false;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;

            Projectile.timeLeft = 1700;

        }

        Vector2 storedDistanceFromOwner = Vector2.Zero;

        public override bool? CanDamage() => false; 

        float alpha = 1f;
        float progress = 0f;
        float flashScale = 0f;
        float flashAlpha = 0f;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (timer == 0)
            {
                storedDistanceFromOwner = Projectile.Center - owner.Center;
                Projectile.ai[0] = (storedDistanceFromOwner.X + owner.Center.X) > owner.Center.X ? -1f : 1f; 
            }
            

            float timeProgress = Utils.GetLerpValue(0f, 1f, timer / 50f, true);
            float alphaTime = Utils.GetLerpValue(0f, 1f, timer / 40f, true);

            Projectile.rotation = MathHelper.Lerp(MathHelper.TwoPi * 1.25f * Projectile.ai[0], 0f, Easings.easeOutQuart(timeProgress));
            progress = Easings.easeOutQuad(timeProgress);
            
            if (timer < 50)
            {
                alpha = MathHelper.Lerp(0f, 1f, Easings.easeOutQuad(alphaTime));
                Projectile.Center = Vector2.Lerp(Projectile.Center, owner.Center + storedDistanceFromOwner, 0.1f);
            }

            Projectile.scale = MathHelper.Lerp(0f, 1f, Easings.easeOutCirc(alphaTime));

            if (timer >= 50f)
            {
                if (timer == 50)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Item_68") with { Pitch = -.05f, PitchVariance = .15f, MaxInstances = -1, Volume = 0.5f }; 
                    SoundEngine.PlaySound(style, Projectile.Center);

                    //SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = 0f, PitchVariance = .15f, MaxInstances = -1, Volume = 0f}; 
                    //SoundEngine.PlaySound(style2, Projectile.Center);

                    //SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_phantom_phoenix_shot_2") with { Pitch = .65f, PitchVariance = .2f, MaxInstances = -1 }; 
                    //SoundEngine.PlaySound(style3, Projectile.Center);

                    SoundStyle style4 = new SoundStyle("Terraria/Sounds/Custom/dd2_dark_mage_heal_impact_2") with { Pitch = .22f, PitchVariance = .19f, }; 
                    SoundEngine.PlaySound(style4, Projectile.Center);

                    for (int i = 0; i < 0; i++)
                    {
                        Vector2 vel = Main.rand.NextVector2CircularEdge(5f, 5f) * Main.rand.NextFloat(1f, 1.5f);
                        float scale = Main.rand.NextFloat(0.35f, 0.55f);

                        Dust dust = Dust.NewDustPerfect(Projectile.Center + vel.SafeNormalize(Vector2.UnitX) * 10, ModContent.DustType<GlowPixelCross>(), vel, 0, Color.DeepPink, scale);
                        dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.1f, preSlowPower: 0.97f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.85f, shouldFadeColor: false);

                        //Dust leaf2 = Dust.NewDustPerfect(Projectile.Center, DustID.Enchanted_Pink, vel, 150, Color.HotPink with { A = 0 }, scale);
                        //leaf2.noGravity = true;
                    }

                    #region AoE Strike
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active)
                        {
                            if (!Main.npc[i].friendly && !Main.npc[i].dontTakeDamage && Vector2.Distance(Projectile.Center, Main.npc[i].Center) < 80f)
                            {
                                int Direction = 0;
                                if (Projectile.Center.X - Main.npc[i].Center.X < 0)
                                    Direction = 1;
                                else
                                    Direction = -1;

                                HitInfo myHit = new HitInfo();
                                myHit.Damage = (int)Projectile.damage;
                                myHit.Knockback = 0;
                                myHit.HitDirection = Direction;

                                Main.npc[i].StrikeNPC(myHit);

                                Main.npc[i].AddBuff(ModContent.BuffType<CerobaMark>(), 320);


                                for (int k = 0; k < 4 + Main.rand.Next(0, 2); k++)
                                {
                                    //Vector2 randomStart = Main.rand.NextVector2CircularEdge(3, 3);
                                    //Dust d = Dust.NewDustPerfect(Main.npc[i].Center, ModContent.DustType<GlowStrong>(),
                                    //randomStart * Main.rand.NextFloat(0.65f, 1.35f), newColor: Color.Pink, Scale: 0.1f + Main.rand.NextFloat(0, 0.2f));

                                    Vector2 vel = Main.rand.NextVector2CircularEdge(3f, 3f) * Main.rand.NextFloat(1f, 1.5f);
                                    float scale = Main.rand.NextFloat(0.55f, 0.75f);

                                    Dust dust = Dust.NewDustPerfect(Main.npc[i].Center + vel.SafeNormalize(Vector2.UnitX) * 10, ModContent.DustType<GlowPixelCross>(), vel, 0, Color.DeepPink, scale);
                                    dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.1f, preSlowPower: 0.97f, timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 4f, fadePower: 0.85f, shouldFadeColor: false);

                                }
                            }
                        }
                    }
                    #endregion
                }

                float flashProgress = Utils.GetLerpValue(0f, 1f, (timer - 50f) / 30f, true);

                flashAlpha = MathHelper.Lerp(1f, 0f, Easings.easeOutCubic(flashProgress));
                flashScale = MathHelper.Lerp(1f, 1.5f, Easings.easeOutQuad(flashProgress));

                if (flashAlpha <= 0.1f)
                {
                    alpha = Math.Clamp(MathHelper.Lerp(alpha, -0.25f, 0.15f), 0f, 1f);

                    if (alpha == 0)
                        Projectile.active = false;
                }
            }
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (timer <= 0) return false;

            string path = "AerovelenceMod/Content/Items/Weapons/Misc/Magic/Ceroba/";

            Texture2D Ward = ModContent.Request<Texture2D>(path + "CerobaWardFull").Value;
            Texture2D WardTop = ModContent.Request<Texture2D>(path + "CerobaWardFullNoBG").Value;

            Texture2D Overlay = ModContent.Request<Texture2D>(path + "CerobaWardCore").Value;

            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle framing = Ward.Frame(1, 1, 0, 0);

            Main.EntitySpriteDraw(Ward, position, framing, Color.HotPink with { A = 0 } * alpha, Projectile.rotation, Ward.Size() / 2f, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(Ward, position, framing, Color.DeepPink with { A = 0 } * alpha, Projectile.rotation, Ward.Size() / 2f, Projectile.scale, SpriteEffects.None);

            Main.EntitySpriteDraw(WardTop, position, framing, Color.White * alpha * 1f, Projectile.rotation, WardTop.Size() / 2f, Projectile.scale, SpriteEffects.None);

            Main.EntitySpriteDraw(Overlay, position, framing, Color.HotPink with { A = 0 } * alpha * flashAlpha * 2f, Projectile.rotation, Overlay.Size() / 2f, Projectile.scale * flashScale, SpriteEffects.None);
            Main.EntitySpriteDraw(Overlay, position, framing, Color.DeepPink with { A = 0 } * alpha * flashAlpha * 0.5f, Projectile.rotation, Overlay.Size() / 2f, Projectile.scale * flashScale * 0.5f, SpriteEffects.None);

            return false;
        }

    }

    public class CerobaPrimarySwing : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 9000;
            Projectile.scale = 1f;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles() => false;

        public override bool? CanDamage() => false;

        int timer = 0;
        float armRot = 0f;


        float arcCurrentAngle = 0f;
        float arcStartAngle = 0f;
        float arcMiddleAngle = 0f;
        float arcEndAngle = 0f;
        public override void AI()
        {
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            if (timer == 0)
            {
                previousRotations = new List<float>();

                float middleRot = Projectile.velocity.ToRotation();
                arcMiddleAngle = middleRot;
                Projectile.velocity = Vector2.Zero;

                int swingDir = (int)Projectile.ai[1];

                arcStartAngle = middleRot + 9f * swingDir;
                arcEndAngle = middleRot - 9f * swingDir;
                timer++;
            }

            Projectile.timeLeft++;
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;
            Projectile.Center = player.Center;

            player.itemAnimation = 2;
            player.itemTime = 2;

            int dir = player.direction;

            float arcTravelProgress = Utils.GetLerpValue(0f, 1f, timer / 50f, true);
            float staffSpinProgress = Utils.GetLerpValue(0f, 1f, timer / 50f, true);

            animProgress = Easings.easeInOutHarsh(arcTravelProgress);

            arcCurrentAngle = MathHelper.Lerp(arcStartAngle, arcEndAngle, animProgress);
            

            Projectile.rotation = arcCurrentAngle;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, arcCurrentAngle - MathHelper.PiOver2);
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, arcCurrentAngle - MathHelper.PiOver2);

            if (timer == 50)
                Projectile.active = false;

            int timeVal = 25;
            if (timer == timeVal - 3 || timer == timeVal + 3 || timer == timeVal - 12 || timer == timeVal + 12) 
            {
                Vector2 randomVel = arcMiddleAngle.ToRotationVector2().RotatedByRandom(1f) * Main.rand.NextFloat(9f, 11f);

                int firePulse = Projectile.NewProjectile(null, player.Center, randomVel * 2f, ModContent.ProjectileType<CerobaFireBall>(), 10, 0, Main.myPlayer);

                for (int d = 0; d < 18; d++)
                {
                    Color col = Main.rand.NextBool() ? Color.Pink : Color.HotPink;

                    Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<RoaParticle>(), randomVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.35f, 1f), newColor: col, Scale: Main.rand.NextFloat(0.5f, 1f));
                    dust.fadeIn = Main.rand.Next(0, 4);
                    dust.alpha = Main.rand.Next(0, 2);
                }

                SoundStyle style3 = new SoundStyle("Terraria/Sounds/Custom/dd2_etherian_portal_dryad_touch") with { Volume = .30f, Pitch = .81f, PitchVariance = .32f, MaxInstances = -1, };
                SoundEngine.PlaySound(style3, player.Center);
            }


            if (timer == 18)
            {
                SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/glaive_shot_01") with { Pitch = -.15f, PitchVariance = .1f, Volume = 0.3f }; 
                SoundEngine.PlaySound(stylea, player.Center);

                SoundStyle styleb = new SoundStyle("AerovelenceMod/Sounds/Effects/trident_twirl_01") with { Pitch = .3f, Volume = 0.5f, PitchVariance = 0.2f };
                SoundEngine.PlaySound(styleb, Projectile.Center);
            }

            timer++;
        }

        float alpha = 1f;
        float animProgress = 0f;
        public List<float> previousRotations;
        public override bool PreDraw(ref Color lightColor)
        {
            string path = "Content/Items/Weapons/Misc/Magic/Ceroba/";
            Texture2D Staff = Mod.Assets.Request<Texture2D>(path + "CerobaStaffProj").Value;
            Texture2D Stick = Mod.Assets.Request<Texture2D>(path + "CerobaStaffStick").Value;

            Texture2D White = Mod.Assets.Request<Texture2D>(path + "CerobaStaffWhiteBell").Value;
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>(path + "CerobaStaffGlowMask").Value;

            Texture2D Swirl = Mod.Assets.Request<Texture2D>("Assets/TrailImages/TerraOrbC").Value;

            Player player = Main.player[Projectile.owner];
            Vector2 origin = Staff.Size() / 2;

            Vector2 armPos = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, armRot) - Main.screenPosition;
            int dir = player.direction;

            Vector2 offset = player.Center + arcCurrentAngle.ToRotationVector2() * 14f;

            float swirlAlpha = (1f - animProgress);
            float swirlRot1 = Projectile.rotation - 1.5f;
            float swirlRot2 = Projectile.rotation + MathF.PI - 0.25f;

            float intensity = (float)Math.Sin(animProgress * Math.PI);

            Main.EntitySpriteDraw(Swirl, offset - Main.screenPosition, null, Color.HotPink with { A = 0 } * intensity* 0.4f, swirlRot1 + (dir == 1 ? 0f : 0.5f), Swirl.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None);
            Main.EntitySpriteDraw(Swirl, offset - Main.screenPosition, null, Color.DeepPink with { A = 0 } * intensity * 0.4f, swirlRot2 + (dir == 1 ? 0f : 4f), Swirl.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None);
            Main.EntitySpriteDraw(Swirl, offset - Main.screenPosition, null, Color.Pink with { A = 0 } * intensity * 0.4f, Projectile.rotation, Swirl.Size() / 2, Projectile.scale * 0.5f, SpriteEffects.None);


            Main.EntitySpriteDraw(Swirl, offset - Main.screenPosition, null, Color.DeepPink with { A = 0 } * intensity * 0.2f, swirlRot1 + (dir == 1 ? 0f : 0.5f), Swirl.Size() / 2, Projectile.scale * 0.35f, SpriteEffects.None);
            Main.EntitySpriteDraw(Swirl, offset - Main.screenPosition, null, Color.HotPink with { A = 0 } * intensity * 0.2f, swirlRot2 + (dir == 1 ? 0f : 4f), Swirl.Size() / 2, Projectile.scale * 0.35f, SpriteEffects.None);


            for (int i = 0; i < 4; i++)
            {
                float underGlowAdjustedRot = Projectile.rotation + MathHelper.PiOver4;
                Main.EntitySpriteDraw(White, offset - Main.screenPosition + Main.rand.NextVector2Circular(2f, 2f) + new Vector2(21f, -21f).RotatedBy(underGlowAdjustedRot), 
                    null, Color.Gold with { A = 0 } * alpha * 1f * intensity, underGlowAdjustedRot, White.Size() / 2, Projectile.scale, SpriteEffects.None);
            }

            Main.EntitySpriteDraw(Staff, offset - Main.screenPosition, null, lightColor * alpha, Projectile.rotation + MathHelper.PiOver4, origin, Projectile.scale, SpriteEffects.None);
            Main.EntitySpriteDraw(Glowmask, offset - Main.screenPosition, null, Color.White * alpha, Projectile.rotation + MathHelper.PiOver4, origin, Projectile.scale, SpriteEffects.None);

            return false;
        }

    }

    public class CerobaSkillStrikeFX : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;

            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 100;

            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;

        int timer = 0;
        public float scale = 1f;
        public float alpha = 1f;

        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.velocity = Vector2.Zero;
            }

            if (timer < 5)
            {
                scale += 0.15f;
            }
            else
            {
                float easeProg = Utils.GetLerpValue(0f, 1f, (timer - 5) / 16f, true);

                scale = MathHelper.Lerp(1.75f, 0f, Easings.easeOutCubic(easeProg));
                alpha = MathHelper.Lerp(1f, 0f, Easings.easeInQuad(easeProg));

                if (scale <= 0.1f)
                    Projectile.active = false;
            }
            
            timer++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Spike = Mod.Assets.Request<Texture2D>("Assets/TrailImages/CrispStarPMA").Value;

            Vector2 origin = new Vector2(Spike.Width / 2f, Spike.Height / 2f);
            Vector2 scale2 = new Vector2(1f, 0.5f) * Projectile.scale * 1.5f * scale;

            Main.EntitySpriteDraw(Spike, Projectile.Center - Main.screenPosition, null, Color.DeepPink with { A = 0 } * alpha, Projectile.rotation, origin, scale2, SpriteEffects.None);
            Main.EntitySpriteDraw(Spike, Projectile.Center - Main.screenPosition, null, Color.Pink with { A = 0 } * alpha, Projectile.rotation, origin, scale2 * 0.75f, SpriteEffects.None);

            Main.EntitySpriteDraw(Spike, Projectile.Center - Main.screenPosition, null, Color.DeepPink with { A = 0 } * alpha, Projectile.rotation + MathHelper.PiOver2, origin, scale2, SpriteEffects.None);
            Main.EntitySpriteDraw(Spike, Projectile.Center - Main.screenPosition, null, Color.Pink with { A = 0 } * alpha, Projectile.rotation + MathHelper.PiOver2, origin, scale2 * 0.75f, SpriteEffects.None);

            return false;
        }
    }

    public class CerobaSpinProj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.timeLeft = 9000;
            Projectile.scale = 1f;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles() { return false; }

        public override bool? CanDamage() { return false; }

        int timer = 0;

        float justShotValue = 0f;
        float goalAngle = 0f;
        float distanceMult = 1f;
        public override void AI()
        {
            if (timer == 0)
            {
                previousRotations = new List<float>();

                goalAngle = Projectile.velocity.ToRotation();
                Projectile.velocity = Vector2.Zero;
            }

            Player player = Main.player[Projectile.owner];

            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            player.heldProj = Projectile.whoAmI;
            Projectile.Center = player.Center;

            player.itemAnimation = 2;
            player.itemTime = 2;
            Projectile.timeLeft++;

            int dir = player.direction;
            float staffSpinProgress = Utils.GetLerpValue(0f, 1f, timer / 45f, true);
            float alphaProg = Utils.GetLerpValue(0f, 1f, timer / 15f, true);

            animProgress = staffSpinProgress;


            Projectile.rotation = MathHelper.Lerp(goalAngle + (2.1f * MathHelper.TwoPi) * -dir, goalAngle, Easings.easeOutCirc(staffSpinProgress));
            alpha = MathHelper.Lerp(0f, 1f, Easings.easeOutQuad(alphaProg));


            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            //Stab Out
            if (timer >= 45 && timer <= 55)
            {
                float easeVal = Utils.GetLerpValue(0f, 1f, (timer - 45) / 10f, true);

                distanceMult = MathHelper.Lerp(1f, 5f, Easings.easeOutCirc(easeVal));

                if (timer < 50)
                    justShotValue = 1f;
                else
                    justShotValue = MathHelper.Clamp(MathHelper.Lerp(justShotValue, -0.5f, 0.1f), 0f, 1f);

                if (timer == 46)
                {
                    SoundStyle stab = new SoundStyle("Terraria/Sounds/Custom/dd2_javelin_throwers_attack_1") with { Pitch = .5f, PitchVariance = .2f, MaxInstances = -1, Volume = 0.25f }; 
                    SoundEngine.PlaySound(stab, Projectile.Center);

                    for (int i = 0; i < 7; i++)
                    {
                        Vector2 vel = Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(10f, 20f);
                        Vector2 spawnPos = Projectile.Center + new Vector2(Main.rand.NextFloat(-10f, 20f), Main.rand.NextFloat(-10f, 10f)).RotatedBy(Projectile.rotation);

                        Dust p = Dust.NewDustPerfect(spawnPos, ModContent.DustType<LineSpark>(),
                            vel, newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.6f);

                        p.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 2, killEarlyTime: 5,
                            1f, 0.35f);
                    }

                }

            }
            else if (timer > 55)
            {
                float easeVal = Utils.GetLerpValue(0f, 1f, (timer - 55) / 30f, true);
                distanceMult = MathHelper.Lerp(4, 1f, Easings.easeOutCubic(easeVal));

                justShotValue = MathHelper.Clamp(MathHelper.Lerp(justShotValue, -0.5f, 0.1f), 0f, 1f);
            }

            if (timer == 80)
            {
                int idle = Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<CerobaIdleHeldProj>(), 0, 0, player.whoAmI);

                if (Main.projectile[idle].ModProjectile is CerobaIdleHeldProj cihp)
                {
                    cihp.startDone = true;
                }
                Projectile.active = false;
            }

            int trailCount = 6;
            previousRotations.Add(Projectile.rotation);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (timer % 2 == 0 && animProgress <= 0.4f)
            {
                Vector2 vel = (Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 5f * dir;
                float scale = Main.rand.NextFloat(0.75f, 1f);
                int leaf2 = Dust.NewDust(Projectile.Center + Projectile.rotation.ToRotationVector2() * 40f, 1, 1, DustID.Enchanted_Pink, vel.X, vel.Y, 150, Color.HotPink with { A = 0 }, scale);
                Main.dust[leaf2].noGravity = true;
            }

            if (timer % 10 == 0 && timer <= 15)
            {
                float pitch = timer == 0 ? 0.45f : 0.6f;
                float vol = timer == 0 ? 0.7f : 0.35f;

                SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_monk_staff_swing_1") with { Pitch = pitch, PitchVariance = .03f, Volume = vol * 0.6f }; 
                SoundEngine.PlaySound(style, Projectile.Center);

            }

            timer++;
        }

        float starRot = 0f;
        float alpha = 0f;
        float animProgress = 0f;
        public List<float> previousRotations;
        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0 || timer == 1)
                return false;
            
            string path = "Content/Items/Weapons/Misc/Magic/Ceroba/";
            Texture2D Staff = Mod.Assets.Request<Texture2D>(path + "CerobaStaffProj").Value;
            Texture2D Stick = Mod.Assets.Request<Texture2D>(path + "CerobaStaffStick").Value;

            Texture2D White = Mod.Assets.Request<Texture2D>(path + "CerobaStaffWhiteBell").Value;
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>(path + "CerobaStaffGlowMask").Value;

            Texture2D Swirl = Mod.Assets.Request<Texture2D>("Assets/TrailImages/TerraOrbC").Value;
            Texture2D SwirlD = Mod.Assets.Request<Texture2D>("Assets/TrailImages/TerraSwingD").Value;

            Texture2D Star = Mod.Assets.Request<Texture2D>("Assets/TrailImages/CrispStarPMA").Value;

            Player player = Main.player[Projectile.owner];
            Vector2 origin = Staff.Size() / 2;

            int dir = player.direction;
            SpriteEffects sFX = dir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float correctRot = Projectile.rotation + MathHelper.PiOver4;
            
            float leftRotBonus = dir == 1 ? 0f : MathHelper.PiOver2;

            Vector2 offset = player.Center + Projectile.rotation.ToRotationVector2() * 10f * distanceMult + new Vector2(0f, player.gfxOffY);

            float swirlAlpha = (1f - animProgress);
            float swirlRot1 = Projectile.rotation;
            float swirlRot2 = Projectile.rotation;

            SpriteEffects swirlFX = dir == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Main.EntitySpriteDraw(Swirl, offset - Main.screenPosition, null, Color.HotPink with { A = 0 } * Easings.easeInCubic(swirlAlpha) * 0.4f, swirlRot1 + (dir == 1 ? -0.5f : 0.5f), Swirl.Size() / 2, Projectile.scale * 0.5f, swirlFX);
            Main.EntitySpriteDraw(Swirl, offset - Main.screenPosition, null, Color.DeepPink with { A = 0 } * Easings.easeInCubic(swirlAlpha) * 0.4f, swirlRot2 + (dir == 1 ? -2f : 2f), Swirl.Size() / 2, Projectile.scale * 0.5f, swirlFX);

            Main.EntitySpriteDraw(SwirlD, offset - Main.screenPosition, null, Color.HotPink with { A = 0 } * Easings.easeInCubic(swirlAlpha) * 0.2f, swirlRot1 + (dir == 1 ? -0.5f : 0.5f), SwirlD.Size() / 2, Projectile.scale * 0.5f, swirlFX);
            Main.EntitySpriteDraw(SwirlD, offset - Main.screenPosition, null, Color.DeepPink with { A = 0 } * Easings.easeInCubic(swirlAlpha) * 0.2f, swirlRot2 + (dir == 1 ? -2f : 2f), SwirlD.Size() / 2, Projectile.scale * 0.5f, swirlFX);

            if (previousRotations != null)
            {
                for (int i = 0; i < previousRotations.Count; i++)
                {
                    float progress = (float)i / previousRotations.Count;

                    Color col = Color.Brown * progress;

                    float adjustedRot = previousRotations[i] + MathHelper.PiOver4;

                    Main.EntitySpriteDraw(Stick, offset - Main.screenPosition, null, col * 0.75f * alpha * swirlAlpha, adjustedRot + leftRotBonus, origin, Projectile.scale, sFX);

                    Main.EntitySpriteDraw(White, offset + new Vector2(21f, -21f).RotatedBy(adjustedRot) - Main.screenPosition, null, Color.Gold with { A = 0 } * 0.3f * alpha * swirlAlpha * Easings.easeOutCirc(progress), adjustedRot + leftRotBonus, White.Size() / 2f, Projectile.scale, sFX);
                }
            }

            #region Ribbons
            Texture2D RibbonBottom = Mod.Assets.Request<Texture2D>(path + "RibbonBottom").Value;
            Texture2D RibbonTop = Mod.Assets.Request<Texture2D>(path + "RibbonTop").Value;

            Vector2 bottomOrigin = new Vector2(9f, 0f);
            Vector2 topOrigin = new Vector2(RibbonTop.Width, 8f);

            float scalePercent = Easings.easeOutQuad(0f + (1f * animProgress));
            SpriteEffects ribbonSpriteFXTop = dir == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            SpriteEffects ribbonSpriteFXBottom = dir == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 ribbonBottomPos = offset - Main.screenPosition + (dir == 1 ? new Vector2(15f, -11f) : new Vector2(11f, -15f)).RotatedBy(correctRot);
            Vector2 ribbonTopPos = offset - Main.screenPosition + (dir == 1 ? new Vector2(11, -15f) : new Vector2(15f, -11f)) .RotatedBy(correctRot);


            Vector2 bottomScale = new Vector2((scalePercent * 0.85f) - (justShotValue * 0.2f), 1f);
            Vector2 topScale = new Vector2(1f, (scalePercent * 0.85f) - (justShotValue * 0.2f));

            float ribbonBottomRot = correctRot + (dir == 1 ? 0.15f + (justShotValue * 0.3f) : -0.15f + (justShotValue * -0.3f)) + (dir == 1 ? 0f : MathHelper.PiOver2);
            float ribbonTopRot = correctRot - (dir == 1 ? 0.15f + (justShotValue * 0.3f) : -0.15f + (justShotValue * -0.3f)) - (dir == 1 ? 0f : MathHelper.PiOver2);

            //float ribbonBottomRot =  + (dir == 1 ? 0f : MathHelper.PiOver2);
            //float ribbonTopRot = correctRot - 0.15f - (justShotValue * 0.2f) - (dir == 1 ? 0f : MathHelper.PiOver2);

            Main.EntitySpriteDraw(RibbonBottom, ribbonBottomPos, null, lightColor * alpha, ribbonBottomRot, bottomOrigin, bottomScale * Projectile.scale, ribbonSpriteFXBottom);
            Main.EntitySpriteDraw(RibbonTop, ribbonTopPos, null, lightColor * alpha, ribbonTopRot, topOrigin, topScale * Projectile.scale, ribbonSpriteFXTop);

            //Glow Ribbons
            float glowRibbonBonusScale = 1f + (justShotValue * 0.05f);
            Main.EntitySpriteDraw(RibbonBottom, ribbonBottomPos, null, Color.Pink with { A = 0 } * justShotValue * 0.75f, ribbonBottomRot, bottomOrigin, bottomScale * Projectile.scale * glowRibbonBonusScale, ribbonSpriteFXBottom);
            Main.EntitySpriteDraw(RibbonTop, ribbonTopPos, null, Color.Pink with { A = 0 } * justShotValue * 0.75f, ribbonTopRot, topOrigin, topScale * Projectile.scale * glowRibbonBonusScale, ribbonSpriteFXTop); ;

            #endregion

            Color underGlowColor = Color.Lerp(Color.Gold with { A = 0 } * 0.25f, Color.HotPink with { A = 0 } * 0.75f, justShotValue);
            float underGlowExtraScale = justShotValue * 0.2f;

            for (int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(White, offset - Main.screenPosition + Main.rand.NextVector2Circular(1.5f, 1.5f) + new Vector2(21f, -21f).RotatedBy(correctRot), null, underGlowColor * alpha, correctRot, White.Size() / 2, Projectile.scale + underGlowExtraScale, SpriteEffects.None);
            }

            float starAlpha = justShotValue * alpha;
            Vector2 starScale = new Vector2(1f, 0.75f) * justShotValue;
            Color middleGroundPink = Color.Lerp(Color.DeepPink, Color.HotPink, 0.25f);
           
            Main.EntitySpriteDraw(Star, offset - Main.screenPosition + new Vector2(21f, -21f).RotatedBy(correctRot), null, middleGroundPink with { A = 0 } * starAlpha, Projectile.rotation, Star.Size() / 2, starScale * 1.25f, SpriteEffects.None);
            Main.EntitySpriteDraw(Star, offset - Main.screenPosition + new Vector2(21f, -21f).RotatedBy(correctRot), null, Color.HotPink with { A = 0 } * starAlpha, Projectile.rotation, Star.Size() / 2, starScale, SpriteEffects.None);


            Main.EntitySpriteDraw(Staff, offset - Main.screenPosition, null, lightColor * alpha, Projectile.rotation + MathHelper.PiOver4 + leftRotBonus, origin, Projectile.scale, sFX);
            Main.EntitySpriteDraw(Glowmask, offset - Main.screenPosition, null, Color.White * alpha, Projectile.rotation + MathHelper.PiOver4 + leftRotBonus, origin, Projectile.scale, sFX);

            return false;
        }

    }

    public class CerobaMark : ModBuff
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;  // Is it a debuff?
            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
            BuffID.Sets.IsATagBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<CerobaMarkModNPC>().CerobaMarkActive = true;
        }
    }

    public class CerobaMarkModNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool CerobaMarkActive = false;
        public float CerobaMarkTime = 0f;

        public override void ResetEffects(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<CerobaMark>()))
            {
                CerobaMarkActive = false;
                CerobaMarkTime = 0;
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (CerobaMarkActive)
            {
                
                if (CerobaMarkTime % 2 == 0)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowPixelRise>(), Scale: 0.5f, newColor: Color.HotPink);
                    Main.dust[dust].velocity.Y *= 1f;
                    Main.dust[dust].velocity.X *= 0.5f;
                    Main.dust[dust].alpha = 2;
                    Main.dust[dust].noLight = true;


                    if (CerobaMarkTime % 4 == 0)
                    {
                        int dust2 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowPixelRise>(), Scale: 0.5f, newColor: Main.rand.NextBool() ? Color.DeepPink : Color.Pink);
                        Main.dust[dust2].velocity.Y *= 1f;
                        Main.dust[dust2].velocity.X *= 0.85f;
                        Main.dust[dust2].alpha = 2;
                        Main.dust[dust2].noLight = true;
                    }

                }

                CerobaMarkTime++;
            }
        }
    }
}

