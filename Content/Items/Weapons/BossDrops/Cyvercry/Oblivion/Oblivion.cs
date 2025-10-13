using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using static Terraria.NPC;
using ReLogic.Content;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Common.Systems;
using Terraria.Graphics;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Interfaces;
using AerovelenceMod.Common;
using AerovelenceMod.Content.Items.Weapons.Misc.Magic.Ceroba;


namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry.Oblivion
{
    //Dear god this is a mess
    //TODO: Better asset loading, sell price
    public class Oblivion : ModItem
    {
        bool tick = false;
        public override void SetDefaults()
        {
            Item.damage = 113;
            Item.knockBack = KnockbackTiers.Average;

            Item.width = 60;
            Item.height = 68;
            Item.useAnimation = 18;
            Item.useTime = 18;

            Item.DamageType = DamageClass.Melee;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shoot = ModContent.ProjectileType<OblivionHeldProjectile>();
            Item.rare = ItemRarities.RarePrePlant;

            Item.shootSpeed = 1f;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (Main.player[player.whoAmI].GetModPlayer<OblivionBarPlayer>().barProgress < 1f)
                    return false;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                type = ModContent.ProjectileType<OblivionActivateAnimation>();
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
            else
            {
                tick = !tick;

                if (player.GetModPlayer<OblivionBarPlayer>().decreaseBar)
                {
                    type = ModContent.ProjectileType<OblivionFinaleSwing>();
                    damage *= 3;
                }


                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
            }

            return false;
        }
    }
    //Basic Swing
    public class OblivionHeldProjectile : BaseSwingSwordProj
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.timeLeft = 10000;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 70;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.extraUpdates = 3;
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f) && justHitTime <= -1;
            return shouldDamage;
        }

        bool hasHitEnemy = false;
        bool playedSound = false;
        public override void AI()
        {
            SwingHalfAngle = 185;
            easingAdditionAmount = 0.011f; //0.01
            offset = 55;
            frameToStartSwing = 3 * 3;
            timeAfterEnd = 5 * 3;

            StandardHeldProjCode();
            StandardSwingUpdate();

            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                SoundStyle styleaa = new SoundStyle("AerovelenceMod/Sounds/Effects/Tech/ShittySword2") with { Volume = 0.45f, Pitch = 0f, PitchVariance = 0.15f }; 
                SoundEngine.PlaySound(styleaa, Projectile.Center);

                playedSound = true;
            }

            if (timer % 1 == 0 && justHitTime <= 0 && getProgress(easingProgress) > 0.1f)
            {
                previousRotations.Add(Projectile.rotation);

                if (previousRotations.Count > 17)
                    previousRotations.RemoveAt(0);
            }

            //Dust
            int dustMod = 1;
            if (timer % dustMod == 0 && (getProgress(easingProgress) >= 0.2f && getProgress(easingProgress) <= 0.8f) && justHitTime <= 0)
            {
                Dust d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + currentAngle.ToRotationVector2() * Main.rand.NextFloat(50f, 100f), ModContent.DustType<PixelGlowOrb>(),
                    currentAngle.ToRotationVector2().RotatedByRandom(0.3f).RotatedBy(MathHelper.PiOver2 * (Projectile.ai[0] > 0 ? 1 : -1)) * -Main.rand.NextFloat(2f, 5f),
                    0, newColor: Color.DeepPink, Main.rand.NextFloat(0.45f, 0.65f));
                d.scale *= Projectile.scale;

                d.noLight = false;
                d.customData = DustBehaviorUtil.AssignBehavior_PGOBase(postSlowPower: 0.9f, velToBeginShrink: 2.5f, fadePower: 0.9f);
            }

            float colVal = 0.65f + (float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.25f;
            Lighting.AddLight(Projectile.Center, Color.DeepPink.ToVector3() * colVal);

            justHitTime--;
        }

        public List<float> previousRotations = new List<float>();

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion/Oblivion").Value;
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion/OblivionHeldProj_Glow").Value;
            Texture2D GlowmaskWhite = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion/OblivionHeldProj_GlowWhite").Value;
            Texture2D White = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion/OblivionWhite").Value;

            //Boost during middle of swing
            float progBoost = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle);
            Vector2 otherOffset = new Vector2(
                Projectile.spriteDirection == 1 ? 4 : 0, 
                Projectile.spriteDirection == 1 ? -8 : -10).RotatedBy(currentAngle);

            Vector2 drawPos = armPosition + otherOffset - Main.screenPosition + new Vector2(0f, Main.player[Projectile.owner].gfxOffY);

            Vector2 origin = Projectile.ai[0] != 1 ? new Vector2(0f, Texture.Height) : new Vector2(Texture.Width, Texture.Height);
            float rotationOffset = Projectile.ai[0] != 1 ? 0f : MathHelper.PiOver2;
            SpriteEffects effects = Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float scaleBoost = (float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f;

            //Draw the slash texture and after images first
            SlashDraw(false);

            //Glowing Border
            for (int i = 0; i < 5; i++)
            {
                Vector2 randPos = Main.rand.NextVector2Circular(5f, 5f);
                Main.spriteBatch.Draw(White, drawPos + randPos, null, Color.HotPink with { A = 0 } * progBoost * 0.65f, Projectile.rotation + rotationOffset, origin, Projectile.scale + scaleBoost, effects, 0f);
            }

            Main.spriteBatch.Draw(Texture, drawPos, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + scaleBoost, effects, 0f);
            Main.spriteBatch.Draw(Glowmask, drawPos, null, Color.White, Projectile.rotation + rotationOffset, origin, Projectile.scale + scaleBoost, effects, 0f);

            //Over glow during hitpause
            if (justHitTime > 0)
                Main.spriteBatch.Draw(GlowmaskWhite, drawPos, null, Color.White with { A = 0 }, Projectile.rotation + rotationOffset, origin, Projectile.scale + scaleBoost, effects, 0f);

            return false;
        }

        public void SlashDraw(bool giveUp)
        {
            Texture2D BaseTexture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion/Oblivion").Value;
            Texture2D WhiteGlow = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion/OblivionWhiteGlow").Value;

            float progBoost = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle);
            Vector2 otherOffset = new Vector2(
                Projectile.spriteDirection == 1 ? 4 : 0,
                Projectile.spriteDirection == 1 ? -8 : -10).RotatedBy(currentAngle);

            Vector2 drawPos = armPosition + otherOffset - Main.screenPosition + new Vector2(0f, Main.player[Projectile.owner].gfxOffY);

            Vector2 origin = Projectile.ai[0] != 1 ? new Vector2(0f, BaseTexture.Height) : new Vector2(BaseTexture.Width, BaseTexture.Height);
            float rotationOffset = Projectile.ai[0] != 1 ? 0f : MathHelper.PiOver2;
            SpriteEffects effects = Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float scaleBoost = ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f);

            #region slash
            Texture2D Slash = Mod.Assets.Request<Texture2D>("Assets/Slash/FullSlashTinyBlack").Value;
            Vector2 SlashPos = Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(10f * (float)Math.Sin(MathHelper.Pi * getProgress(easingProgress)), 0).RotatedBy(originalAngle);

            float slashScale = 0.65f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 1.1f);

            Color betweenPink = Color.Lerp(Color.DeepPink, Color.HotPink, 0.25f);
            Color slashColor = Color.Lerp(Color.Black * 0.3f, betweenPink, Easings.easeInOutCirc(progBoost));

            Main.spriteBatch.Draw(Slash, SlashPos, null, slashColor with { A = 0 } * progBoost, originalAngle + MathHelper.PiOver2, Slash.Size() / 2, slashScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Slash, SlashPos, null, Color.HotPink with { A = 0 } * progBoost * 0.15f, originalAngle + MathHelper.PiOver2, Slash.Size() / 2, slashScale, SpriteEffects.None, 0f);
            #endregion

            #region afterImage
            for (int afterI = 0; afterI < previousRotations.Count; afterI++)
            {
                float progress = (float)afterI / previousRotations.Count;

                float size = Projectile.scale + scaleBoost;
                size *= (0.75f + (progress * 0.25f));

                Main.spriteBatch.Draw(WhiteGlow, drawPos, null, Color.HotPink with { A = 0 } * progress * progBoost * 0.5f, previousRotations[afterI] + rotationOffset, origin, size, effects, 0f);
            }
            #endregion
        }

        public override void OnHitNPC(NPC target, HitInfo hit, int damageDone)
        {
            //Want less hitpause at higher attack speeds
            justHitTime = (8 - (int)((Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1) * 8f)) * Projectile.extraUpdates; //7

            float currentShakePower = Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower;
            Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = currentShakePower > 1 ? Math.Clamp(currentShakePower, 2.5f, 5f) : 5f;

            if (!hasHitEnemy)
            {
                //Strike all other enemies within a radius
                GeneralUtils.strikeNPCsInRadius(target.Center, 75f, Projectile.damage * 0.75f, Projectile.knockBack * 0.5f, target.whoAmI);

                Vector2 orthToSwing = (MathHelper.PiOver2 + currentAngle).ToRotationVector2() * (Projectile.ai[0] == 1 ? -1 : 1f);
                Vector2 impactCenter = target.Center;

                #region Dust
                Color between = Color.Lerp(Color.DeepPink, Color.HotPink, 0f);
                Dust d11 = Dust.NewDustPerfect(impactCenter, ModContent.DustType<FeatheredGlowDust>(), Velocity: Vector2.Zero, newColor: between, Scale: 1.25f);

                FeatheredGlowBehavior fgb = new FeatheredGlowBehavior(AlphaChangeSpeed: 0.65f, timeToChangeAlpha: 6, ScaleChangeSpeed: 1.1f, timeToKill: 120, OverallAlpha: 0.5f);
                fgb.DrawWhiteCore = true;
                d11.customData = fgb;


                //Impact Dust
                Color betweenPink = Color.Lerp(Color.DeepPink, Color.HotPink, 0.6f);
                Color betweenPink2 = Color.Lerp(Color.DeepPink, Color.HotPink, 1f);

                CirclePulseBehavior cpb2 = new CirclePulseBehavior(0.55f, true, 3, 0.8f, 0.8f);

                Dust d1 = Dust.NewDustPerfect(impactCenter, ModContent.DustType<CirclePulse>(), Velocity: Vector2.Zero, newColor: betweenPink * 0.25f);
                d1.customData = cpb2;
                d1.velocity = new Vector2(-0.01f, 0f);
                d1.fadeIn = 0.5f;

                Dust d2 = Dust.NewDustPerfect(impactCenter, ModContent.DustType<CirclePulse>(), Velocity: Vector2.Zero, newColor: betweenPink2 * 0.25f);
                d2.customData = cpb2;
                d2.velocity = new Vector2(0.01f, 0f);
                d2.fadeIn = 0.5f;


                int crossCount = 6;
                for (int i = 0; i < crossCount; i++)
                {
                    float dir = (MathHelper.TwoPi / (float)crossCount) * i;

                    Vector2 dustVel = dir.ToRotationVector2() * Main.rand.NextFloat(3.5f, 7f);
                    dustVel = dustVel.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f));

                    Color middleBlue = Color.Lerp(Color.DeepPink, Color.HotPink, 0.25f + Main.rand.NextFloat(-0.15f, 0.15f));

                    Dust gd = Dust.NewDustPerfect(impactCenter, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: middleBlue, Scale: Main.rand.NextFloat(0.25f, 0.55f));
                    gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 5,
                        preSlowPower: 0.94f, postSlowPower: 0.9f, velToBeginShrink: 1.5f, fadePower: 0.92f, shouldFadeColor: false);
                }

                int pgoCount = 7;
                for (int i = 0; i < pgoCount; i++)
                {
                    float dir = (MathHelper.TwoPi / (float)pgoCount) * i;

                    Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<PixelGlowOrb>(), newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.55f, 0.8f));
                    d.velocity = dir.ToRotationVector2() * Main.rand.NextFloat(0.5f, 6f);
                    d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));

                    d.customData = DustBehaviorUtil.AssignBehavior_PGOBase(rotPower: 0.04f, timeBeforeSlow: 4, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.8f, colorFadePower: 1f, glowIntensity: 0.4f);
                }

                int windFX = Projectile.NewProjectile(null, impactCenter, Vector2.Zero, ModContent.ProjectileType<CerobaSkillStrikeFX>(), 0, 0, Main.myPlayer);
                #endregion


                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = 0.018f, Pitch = 0.3f, PitchVariance = 0.1f, MaxInstances = 1, };
                SoundEngine.PlaySound(style, target.Center);

                SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/Tech/MagicPunch") with { Volume = 0.65f, Pitch = -0.1f, PitchVariance = .1f, MaxInstances = 1 };
                SoundEngine.PlaySound(style3, target.Center);

                if (Main.player[Projectile.owner].GetModPlayer<OblivionBarPlayer>().barProgress < 1f)
                {
                    Main.player[Projectile.owner].GetModPlayer<OblivionBarPlayer>().barProgress += 0.15f;
                    Main.player[Projectile.owner].GetModPlayer<OblivionBarPlayer>().justShotPower = 1f;
                }

                hasHitEnemy = true;
            }
        }


        // Find the start and end of the sword and use a line collider to check for collision with enemies
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Main.player[Projectile.owner].MountedCenter;
            Vector2 end = start + currentAngle.ToRotationVector2() * ((Projectile.Size.Length() * 1.25f) * Projectile.scale); //1.2f
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint); //15f
        }

        //Sword easing
        public override float getProgress(float x)
        {
            float toReturn = 0f;

            #region easeExpo

            //pre 0.5
            if (x <= 0.5f)
            {
                toReturn = (float)(Math.Pow(2, (16 * x) - 8)) / 2;
            }
            else if (x > 0.5)
            {
                toReturn = (float)(2 - ((Math.Pow(2, (-16 * x) + 8)))) / 2;
            }

            //post 0.5
            if (x == 0)
                toReturn = 0;
            if (x == 1)
                toReturn = 1;

            return toReturn;


            #endregion;
        }
    }

    //Empowered Swing
    public class OblivionFinaleSwing : BaseSwingSwordProj
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        #region Loading
        public static Asset<Texture2D> circle_053 = null;
        public static Asset<Texture2D> muzzle_flash_12 = null;
        public static Asset<Texture2D> star_07 = null;
        public static Asset<Texture2D> circle_053Black = null;

        public override void Load()
        {
            circle_053 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/MuzzleFlashes/circle_053");
            muzzle_flash_12 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/MuzzleFlashes/muzzle_flash_12");
            star_07 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Flare/star_07");
            circle_053Black = ModContent.Request<Texture2D>("AerovelenceMod/Assets/MuzzleFlashes/circle_053Black");
        }

        public override void Unload()
        {
            circle_053 = null;
            muzzle_flash_12 = null;
            star_07 = null;
            circle_053Black = null;
        }
        #endregion


        public override void SetDefaults()
        {
            Projectile.timeLeft = 10000;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = Projectile.height = 70;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 10;
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f) && justHitTime <= -1;
            return shouldDamage;
        }

        bool hasHitEnemy = false;
        int timeSinceHitEnemy = 0;
        bool playedSound = false;
        public override void AI()
        {
            SwingHalfAngle = 190;
            easingAdditionAmount = 0.021f / Projectile.extraUpdates; //0.024
            frameToStartSwing = 9 * Projectile.extraUpdates; //12
            timeAfterEnd = 1 * Projectile.extraUpdates;
            startingProgress = 0.005f; //0.01

            offset = 55;

            StandardHeldProjCode();
            StandardSwingUpdate();

            Player player = Main.player[Projectile.owner];

            Color betweenPink = Color.Lerp(Color.DeepPink, Color.HotPink, 0.5f);

            //Trail info
            Vector2 gfxOffset = new Vector2(0, Main.player[Projectile.owner].gfxOffY);
            if (justHitTime <= 0f)
            {
                int trailCount = 35;  //14
                relativeRots.Add(currentAngle + MathHelper.PiOver2); //
                relativePoss.Add((Projectile.Center + currentAngle.ToRotationVector2() * 105f) - player.Center - gfxOffset); //90

                if (relativeRots.Count > trailCount)
                    relativeRots.RemoveAt(0);

                if (relativePoss.Count > trailCount)
                    relativePoss.RemoveAt(0);
            }
            //Do screenshake while sword is paused
            else
            {
                player.GetModPlayer<AeroPlayer>().ScreenShakePower = Math.Clamp(player.GetModPlayer<AeroPlayer>().ScreenShakePower, 9f, 100f);
            }
            
            //Stuff so the trail is relative to the player's position
            trailPoss.Clear();
            foreach (Vector2 pos in relativePoss)
            {
                trailPoss.Add(pos + player.Center);
            }

            //Spawn burst of dust as sword appears
            if (timer == 2)
            {
                for (int i = 0; i < 20; i++)
                {
                    float progress = (float)i / 20f;

                    Vector2 spawnPos = Projectile.Center + new Vector2(1f, 0f).RotatedBy(currentAngle) * Main.rand.NextFloat(0, 280f * progress);
                    Vector2 smvel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(3f, 18f * (1f - progress));

                    Dust sm = Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlowPixelAlts>(), smvel, newColor: betweenPink * 1f, Scale: Main.rand.NextFloat(0.45f, 0.7f));
                    sm.alpha = 10;

                    sm.velocity.X *= 0.75f;
                    if (smvel.Y > 0)
                        sm.velocity.Y *= -1;

                    sm.velocity = sm.velocity.RotatedBy(currentAngle + MathHelper.PiOver2);
                }
            }

            //Play sound partway into the swing
            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower += 18;

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/Tech/ShittySword") with { Volume = 0.55f, Pitch = -0.2f, PitchVariance = 0.2f, }; 
                SoundEngine.PlaySound(style, player.Center);

                SoundStyle style22 = new SoundStyle("AerovelenceMod/Sounds/Effects/Tech/UltraBlade3") with { Volume = 0.65f, Pitch = 0.25f, PitchVariance = 0.25f }; 
                SoundEngine.PlaySound(style22, player.Center);

                playedSound = true;
            }

            //Control width of sword and trail
            if (getProgress(easingProgress) < 0.95f)
            {
                float progress = Math.Clamp((timer + (5f * Projectile.extraUpdates)) / (20f * Projectile.extraUpdates), 0f, 1f); //timer / 50
                overallWidth = MathHelper.Lerp(0f, 1f, Easings.easeInOutBack(progress, 0f, 2.5f));
            }
            else
            {
                overallWidth = Math.Clamp(MathHelper.Lerp(overallWidth, -0.15f, 0.06f), 0f, 1f);
            }

            //Dust
            int dustMod = 4;
            if (timer % dustMod == 0 && (getProgress(easingProgress) >= 0.1f && getProgress(easingProgress) <= 0.9f) && justHitTime <= 0)
            {
                Dust d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + currentAngle.ToRotationVector2() * Main.rand.NextFloat(50f, 215f), ModContent.DustType<GlowPixelCross>(),
                    currentAngle.ToRotationVector2().RotatedByRandom(0.3f).RotatedBy(MathHelper.PiOver2 * (Projectile.ai[0] > 0 ? 1 : -1)) * -Main.rand.NextFloat(2f, 5f),
                    0, newColor: betweenPink, Main.rand.NextFloat(0.45f, 0.65f));
                d.scale *= Projectile.scale;

                d.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, postSlowPower: 0.9f, velToBeginShrink: 2.5f, fadePower: 0.85f, shouldFadeColor: false);
            }

            //Cast Light along the sword
            DelegateMethods.v3_1 = Color.HotPink.ToVector3() * overallWidth;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + new Vector2(180f, 0f).RotatedBy(currentAngle), 10f * overallWidth, DelegateMethods.CastLight);

            justHitTime--;
        }

        public List<float> previousRotations = new List<float>();

        float overallAlpha = 1f;
        float overallWidth = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            float progBoost = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            //General stuff to make sure sword position is consistent to player arm
            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle);
            
            Vector2 otherOffset = new Vector2(
                Projectile.spriteDirection == 1 ? 14 : 8, 
                Projectile.spriteDirection == 1 ? -8 : -14).RotatedBy(currentAngle);

            Vector2 drawPos = armPosition + otherOffset - Main.screenPosition + new Vector2(0f, Main.player[Projectile.owner].gfxOffY);
            float rot = currentAngle;

            Texture2D glow1 = circle_053.Value;
            Texture2D glow4 = circle_053Black.Value;

            Vector2 newScale = new Vector2(1.5f, 1f * overallWidth) * 0.5f; //sword
            newScale *= 0.55f;
            Vector2 newScale2 = new Vector2(1f, 1.5f * overallWidth) * 0.5f; //sword
            newScale2 *= 0.55f;
            Vector2 newScale3 = new Vector2(1.5f, 0.35f * overallWidth) * 0.5f; //sword
            newScale3 *= 0.55f;

            Vector2 origin1 = new Vector2(0f, glow1.Height / 2f);

            //Black Base
            Main.spriteBatch.Draw(glow1, drawPos + new Vector2(-50f, 0f).RotatedBy(rot), null, Color.Black * 0.15f * overallWidth, rot, origin1, newScale3, SpriteEffects.None, 0f);

            //Bloom
            Main.spriteBatch.Draw(glow4, drawPos + new Vector2(-50f, 0f).RotatedBy(rot), null, Color.DeepPink with { A = 0 } * 0.15f, rot, origin1, newScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow4, drawPos + new Vector2(-50f, 0f).RotatedBy(rot), null, Color.DeepPink with { A = 0 } * 0.15f, rot, origin1, newScale2, SpriteEffects.None, 0f);

            #region slash Texture
            Texture2D Slash = Mod.Assets.Request<Texture2D>("Assets/Slash/FullSlashBlack").Value;
            Vector2 SlashPos = Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(10f * (float)Math.Sin(MathHelper.Pi * getProgress(easingProgress)), 0).RotatedBy(currentAngle);

            float slashScale = 0.65f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 1.1f);
            slashScale *= 1f;

            Color betweenPink = Color.Lerp(Color.DeepPink, Color.Pink, 0.45f);
            Color slashColor = Color.Lerp(Color.Black * 0.3f, betweenPink, Easings.easeInOutCirc(progBoost));

            Main.spriteBatch.Draw(Slash, SlashPos, null, slashColor with { A = 0 } * Easings.easeInCirc(progBoost) * 0.3f, currentAngle + MathHelper.PiOver2, Slash.Size() / 2, slashScale * 0.75f, SpriteEffects.None, 0f);
            #endregion

            //Draw Trail pixelated
            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                TrailDraw();
            });

            //Draw energy sword pixelated
            //Use Dusts layer so we can draw on top of black underglow
            ModContent.GetInstance<NewAdditivePixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                BladeDraw();
            });


            return false;
        }

        Effect myEffect = null;
        public void BladeDraw()
        {
            Texture2D Glorb = circle_053.Value;
            Texture2D Spike = muzzle_flash_12.Value;
            Texture2D Star = star_07.Value;

            float ySinVal = (float)Math.Sin(Main.timeForVisualEffects * 0.22f) * 0.15f;
            float xSinVal = (float)Math.Sin(Main.timeForVisualEffects * 0.22f) * 0.05f;

            Vector2 BladeScale = new Vector2(1.5f, 1f * overallWidth) * 0.5f; //sword
            Vector2 SpikeScale = new Vector2(0.75f, (1.3f + ySinVal) * overallWidth) * (0.5f + xSinVal); //spiky
            Vector2 HiltScale = new Vector2(0.25f * overallWidth, 0.25f); //Hilt

            BladeScale *= 0.51f;
            SpikeScale *= 0.51f;
            HiltScale *= 0.55f;

            Vector2 origin1 = new Vector2(0f, Glorb.Height / 2f);
            Vector2 origin2 = new Vector2(0f, Spike.Height / 2f);

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle);

            Vector2 otherOffset = new Vector2(
                Projectile.spriteDirection == 1 ? 14 : 8, 
                Projectile.spriteDirection == 1 ? -8 :-14).RotatedBy(currentAngle);

            Vector2 drawPos = armPosition + otherOffset - Main.screenPosition + new Vector2(0f, Main.player[Projectile.owner].gfxOffY);
            float rot = currentAngle;

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaser", AssetRequestMode.ImmediateLoad).Value;

            #region ShaderParams
            myEffect.Parameters["sampleTexture1"].SetValue(CommonTextures.Extra_196_Black.Value);
            myEffect.Parameters["sampleTexture2"].SetValue(CommonTextures.Trail5Loop.Value);
            myEffect.Parameters["sampleTexture3"].SetValue(CommonTextures.FlameTrail.Value);
            myEffect.Parameters["sampleTexture4"].SetValue(CommonTextures.ThinGlowLine.Value);

            Color c1 = Color.DeepPink;
            Color c2 = Color.DeepPink;
            Color c3 = Color.DeepPink;
            Color c4 = Color.DeepPink;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color3"].SetValue(c3.ToVector4());
            myEffect.Parameters["Color4"].SetValue(c4.ToVector4());

            myEffect.Parameters["Color1Mult"].SetValue(1.5f);
            myEffect.Parameters["Color2Mult"].SetValue(1.5f);
            myEffect.Parameters["Color3Mult"].SetValue(1.5f); //1.5
            myEffect.Parameters["Color4Mult"].SetValue(1.1f);
            myEffect.Parameters["totalMult"].SetValue(1f * overallAlpha);

            myEffect.Parameters["tex1reps"].SetValue(1f);
            myEffect.Parameters["tex2reps"].SetValue(1f);
            myEffect.Parameters["tex3reps"].SetValue(1f);
            myEffect.Parameters["tex4reps"].SetValue(1f);

            myEffect.Parameters["satPower"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.03f);
            #endregion

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.EffectMatrix);

            #region blade
            //MainBlade
            Main.spriteBatch.Draw(Glorb, drawPos + new Vector2(-60f, 0f).RotatedBy(rot), null, Color.White, rot, origin1, BladeScale, SpriteEffects.None, 0f);

            //Spiky part near guard
            Main.spriteBatch.Draw(Spike, drawPos + Main.rand.NextVector2Circular(1f, 1f), null, Color.White, rot, origin2, SpikeScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Spike, drawPos + Main.rand.NextVector2Circular(1f, 1f), null, Color.White, rot, origin2, SpikeScale * 0.5f, SpriteEffects.FlipVertically, 0f);

            //"Hilt"
            Vector2 off = rot.ToRotationVector2() * 8f;
            Main.spriteBatch.Draw(Star, drawPos + Main.rand.NextVector2Circular(1f, 1f) + off, null, Color.White, rot + MathHelper.PiOver2, Star.Size() / 2, HiltScale, SpriteEffects.FlipVertically, 0f);
            Main.spriteBatch.Draw(Star, drawPos + Main.rand.NextVector2Circular(1f, 1f) + off, null, Color.White, rot - MathHelper.PiOver2, Star.Size() / 2, HiltScale, SpriteEffects.FlipVertically, 0f);
            #endregion

            //Slash
            Texture2D slash = Mod.Assets.Request<Texture2D>("Assets/Slash/HalfSlashBig").Value;
            
            float progBoost = (float)Math.Sin(getProgress(easingProgress) * Math.PI);
            float slashOpacity = progBoost;
            
            float slashRot = Projectile.ai[0] == 1 ? rot + MathHelper.Pi : rot;
            Vector2 slashScale = new Vector2(1f, 2f) * 0.75f * slashOpacity;
            SpriteEffects SlashSE = Projectile.ai[0] == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            
            Main.spriteBatch.Draw(slash, drawPos + rot.ToRotationVector2() * (10f * slashOpacity), null, Color.White, slashRot, slash.Size() / 2f, slashScale, SlashSE, 0f);
            //Main.spriteBatch.Draw(slash, drawPos + rot.ToRotationVector2() * (20f * slashOpacity), null, Color.White, slashRot, slash.Size() / 2f, slashScale, SlashSE, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        List<float> relativeRots = new List<float>();
        List<Vector2> relativePoss = new List<Vector2>();

        List<Vector2> trailPoss = new List<Vector2>();

        float trailWidth = 1f;
        Effect trailEffect = null;
        public void TrailDraw()
        {
            Texture2D trailTexture = CommonTextures.ThinGlowLine.Value; //
            Texture2D trailTexture2 = CommonTextures.Extra_196_Black.Value; //

            if (trailEffect == null)
                trailEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;


            //Convert lists to arrays for use in vertex strip
            Vector2[] pos_arr = trailPoss.ToArray();
            float[] rot_arr = relativeRots.ToArray();


            float sineWidthMult = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.3f) * 0f;


            Color StripColor(float progress) => Color.White * Easings.easeInSine(progress) * overallAlpha;

            float StripWidth(float progress)
            {
                float toReturn = 0f;
                if (progress < 0.5f) //back half
                {
                    float LV = Utils.GetLerpValue(0f, 0.5f, progress, true);
                    toReturn = Easings.easeOutSine(LV);
                }
                else //Front half
                {
                    float LV = Utils.GetLerpValue(0.5f, 1f, progress, true);
                    toReturn = 1f;// Easings.easeOutSine(1f - LV);
                }

                return toReturn * sineWidthMult * trailWidth * 90f * 1.25f; //50
            }

            float StripWidth2(float progress)
            {
                float toReturn = 0f;
                if (progress < 0.5f) //back half
                {
                    float LV = Utils.GetLerpValue(0f, 0.5f, progress, true);
                    toReturn = Easings.easeOutSine(LV);
                }
                else //Front half
                {
                    float LV = Utils.GetLerpValue(0.5f, 1f, progress, true);
                    toReturn = 1f;// Easings.easeOutSine(1f - LV);
                }

                return toReturn * sineWidthMult * trailWidth * 120f * 1.25f; //50 | 130
            }


            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStrip2 = new VertexStrip();
            vertexStrip2.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth2, -Main.screenPosition, includeBacksides: true);


            trailEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            trailEffect.Parameters["progress"].SetValue((float)Main.timeForVisualEffects * 0.035f); //0.02
            trailEffect.Parameters["reps"].SetValue(1f);


            //Over layer
            Color betweenPink = Color.Lerp(Color.DeepPink, Color.HotPink, 0.15f);
            trailEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            trailEffect.Parameters["ColorOne"].SetValue(betweenPink.ToVector3() * 2f); //2f
            trailEffect.Parameters["glowThreshold"].SetValue(0.85f);
            trailEffect.Parameters["glowIntensity"].SetValue(1.2f);
            trailEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip.DrawTrail();

            Color between = Color.Lerp(Color.OrangeRed, Color.Orange, 1f);
            //UnderLayer
            trailEffect.Parameters["TrailTexture"].SetValue(trailTexture2);
            trailEffect.Parameters["glowThreshold"].SetValue(1f);
            trailEffect.Parameters["glowIntensity"].SetValue(1f);
            trailEffect.Parameters["ColorOne"].SetValue(Color.HotPink.ToVector3() * 4.5f); //Hotpink4.5
            trailEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip2.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        public override void OnHitNPC(NPC target, HitInfo hit, int damageDone)
        {
            //Only do effect on first enemy hit
            if (!hasHitEnemy)
            {
                //Hit all other enemies in radius
                GeneralUtils.strikeNPCsInRadius(Projectile.Center, 200f, Projectile.damage, Projectile.knockBack * 0.5f, target.whoAmI);

                //Want less hitpause at higher attack speeds
                justHitTime = (19 - (int)((Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1) * 19f)) * Projectile.extraUpdates; //10

                Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower += 18;

                //The dust....
                int pgoCount = 12;
                for (int i = 0; i < pgoCount; i++)
                {
                    float dir = (MathHelper.TwoPi / (float)pgoCount) * i;

                    Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowStarSharp>(), newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.55f, 0.8f));
                    d.velocity = dir.ToRotationVector2() * Main.rand.NextFloat(0.75f, 6f) * 2f;
                    d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f));

                    d.customData = DustBehaviorUtil.AssignBehavior_GSSBase(rotPower: 0.04f, timeBeforeSlow: 4, postSlowPower: 0.89f, velToBeginShrink: 1.5f, fadePower: 0.8f, colorFadePower: 1f);
                }

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<OblivionExplosionPulse>(), 0, 0);

                FlashSystem.SetCAFlashEffect(0.075f, 35, 1f, 0.35f, true, true);

                Dust d11 = Dust.NewDustPerfect(target.Center, ModContent.DustType<FeatheredGlowDust>(), Velocity: Vector2.Zero, newColor: Color.DeepPink, Scale: 2.85f);

                FeatheredGlowBehavior fgb = new FeatheredGlowBehavior(AlphaChangeSpeed: 0.65f, timeToChangeAlpha: 6, ScaleChangeSpeed: 1.1f, timeToKill: 120, OverallAlpha: 0.5f);
                fgb.DrawWhiteCore = true;
                d11.customData = fgb;

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/Tech/CyverBurst") with { Volume = 0.6f, PitchVariance = 0.1f };
                SoundEngine.PlaySound(style, target.Center);
            }


            hasHitEnemy = true;
        }

        // Find the start and end of the sword and use a line collider to check for collision with enemies
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Main.player[Projectile.owner].MountedCenter + currentAngle.ToRotationVector2() * (20f * Projectile.scale);
            Vector2 end = start + currentAngle.ToRotationVector2() * (240f * Projectile.scale);

            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 30f * Projectile.scale, ref collisionPoint);
        }

        public override float getProgress(float x)
        {
            float toReturn = 0f;

            #region easeExpo

            //pre 0.5
            if (x <= 0.5f)
            {
                toReturn = (float)(Math.Pow(2, (20 * x) - 10)) / 2;
            }
            else if (x > 0.5)
            {
                toReturn = (float)(2 - Math.Pow(2, (-20 * x) + 10)) / 2;
            }

            //post 0.5
            if (x == 0)
                toReturn = 0;
            if (x == 1)
                toReturn = 1;

            return toReturn;


            #endregion;
        }
    }

    //Empowered On-Hit Pulse (just visual)
    public class OblivionExplosionPulse : ModProjectile, IDrawAdditive
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.hostile = false;
            Projectile.penetrate = -1;

            Projectile.scale = 1f;

            Projectile.timeLeft = 800;
            Projectile.tileCollide = false;
            Projectile.width = 20;
            Projectile.height = 20;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;

        public bool isBigOne = true;

        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
                Projectile.rotation = Main.rand.NextFloat(6.28f);

            int timeForPulse = 20;
            if (timer <= timeForPulse)
                overallScale = MathHelper.Lerp(0f, 1f, Easings.easeOutCubic((float)timer / (float)timeForPulse));

            if (timer >= 0)
            {
                if (timer >= (timeForPulse * 0.5f))
                    overallAlpha -= 0.1f;

                if (overallAlpha <= 0)
                    Projectile.active = false;

                Projectile.rotation += 0.12f;
            }

            timer++;
        }

        float overallScale = 0f;
        float overallAlpha = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                DrawShit(false);
            });
            DrawShit(true);

            return false;
        }

        public void DrawShit(bool giveUp)
        {
            if (giveUp)
                return;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;


            Texture2D Ball = CommonTextures.feather_circle128PMA.Value;
            Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.Black * 0.5f * overallAlpha, Projectile.rotation, Ball.Size() / 2, overallScale * 0.5f, SpriteEffects.None, 0f);

            #region Orb
            Texture2D orb = Mod.Assets.Request<Texture2D>("Assets/Orbs/whiteFireEye").Value;

            Color[] cols = { Color.Pink * 0.75f, Color.HotPink * 0.525f, Color.Lerp(Color.DeepPink, Color.HotPink, 0.25f) * 0.375f };
            float[] OrbScales = { 0.85f * overallScale, 1.6f * Easings.easeInSine(overallScale), 2.5f * Easings.easeInCubic(overallScale) };

            float orbScale = 0.5f;

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.07f) * 0.15f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.13f) * 0.1f;

            Main.EntitySpriteDraw(orb, drawPos, null, cols[0] with { A = 0 } * overallAlpha * 1.3f, (float)Main.timeForVisualEffects * 0.05f, orb.Size() / 2f, OrbScales[0] * orbScale, SpriteEffects.None);
            Main.EntitySpriteDraw(orb, drawPos, null, cols[1] with { A = 0 } * overallAlpha * 1.3f, (float)Main.timeForVisualEffects * 0.02f, orb.Size() / 2f, OrbScales[1] * orbScale * sineScale1, SpriteEffects.None);
            Main.EntitySpriteDraw(orb, drawPos, null, cols[2] with { A = 0 } * overallAlpha * 1.3f, (float)Main.timeForVisualEffects * -0.01f, orb.Size() / 2f, OrbScales[2] * orbScale * sineScale2, SpriteEffects.None);
            #endregion

            #region Flare
            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Flare/flare_4Black").Value;

            float[] FlareScales = { 1f, 0.75f, 0.35f };
            float additiveScale = overallScale * 2f;

            Main.spriteBatch.Draw(Ball, drawPos, null, Color.DeepPink with { A = 0 } * 0.3f * overallAlpha, Projectile.rotation, Ball.Size() / 2, additiveScale * FlareScales[0], SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Flare, drawPos, null, Color.DeepPink with { A = 0 } * overallAlpha, Projectile.rotation * 0.8f, Flare.Size() / 2, additiveScale * FlareScales[1], SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, drawPos, null, Color.HotPink with { A = 0 } * overallAlpha, Projectile.rotation * -0.6f, Flare.Size() / 2, additiveScale * FlareScales[1], SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Flare, drawPos, null, Color.White with { A = 0 } * overallAlpha, Projectile.rotation * 0.6f, Flare.Size() / 2, additiveScale * FlareScales[2], SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Flare, drawPos, null, Color.White with { A = 0 } * overallAlpha, Projectile.rotation * -0.8f, Flare.Size() / 2, additiveScale * FlareScales[2], SpriteEffects.None, 0f);
            #endregion

        }

        public void DrawAdditive(SpriteBatch sb)
        {
            #region Flare
            Texture2D Flare = Mod.Assets.Request<Texture2D>("Assets/Flare/flare_4").Value;

            Texture2D Ball = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle").Value;

            float[] scales = { 1f, 0.75f, 0.35f };
            float additiveScale = overallScale * 2f;

            //Main.spriteBatch.Draw(Ball, Projectile.Center - Main.screenPosition, null, Color.DeepPink * 0.3f * overallAlpha, Projectile.rotation, Ball.Size() / 2, additiveScale * scales[0], SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.DeepPink * overallAlpha, Projectile.rotation * 0.8f, Flare.Size() / 2, additiveScale * scales[1], SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.HotPink * overallAlpha, Projectile.rotation * -0.6f, Flare.Size() / 2, additiveScale * scales[1], SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White * overallAlpha, Projectile.rotation * 0.6f, Flare.Size() / 2, additiveScale * scales[2], SpriteEffects.None, 0f);
            //Main.spriteBatch.Draw(Flare, Projectile.Center - Main.screenPosition, null, Color.White * overallAlpha, Projectile.rotation * -0.8f, Flare.Size() / 2, additiveScale * scales[2], SpriteEffects.None, 0f);
            #endregion
        }
    }

    //Activate Animation
    public class OblivionActivateAnimation : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        #region Loading
        public static Asset<Texture2D> circle_053 = null;
        public static Asset<Texture2D> muzzle_flash_12 = null;
        public static Asset<Texture2D> star_07 = null;
        public static Asset<Texture2D> circle_053Black = null;

        public override void Load()
        {
            circle_053 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/MuzzleFlashes/circle_053");
            muzzle_flash_12 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/MuzzleFlashes/muzzle_flash_12");
            star_07 = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Flare/star_07");
            circle_053Black = ModContent.Request<Texture2D>("AerovelenceMod/Assets/MuzzleFlashes/circle_053Black");
        }

        public override void Unload()
        {
            circle_053 = null;
            muzzle_flash_12 = null;
            star_07 = null;
            circle_053Black = null;
        }
        #endregion

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = false;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Melee;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;


        public int timer = 0;
        public override void AI()
        {
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);
            Player player = Main.player[Projectile.owner];

            int timeForSpinIn = 20;

            #region swordSpinIn
            float easeProg = Utils.GetLerpValue(0, timeForSpinIn, timer, true);

            float goalRotation = player.direction == 1 ? MathHelper.ToRadians(-90) : MathHelper.ToRadians(270);
            Vector2 goalPos = player.MountedCenter + goalRotation.ToRotationVector2() * 50;

            float startRot = MathHelper.PiOver2;

            if (easeProg < 1f)
            {
                if (timer % 7 == 0 && timer < 10)
                {
                    SoundStyle style = new SoundStyle("Terraria/Sounds/Item_7") with { Pitch = -0.25f, PitchVariance = 0.2f, MaxInstances = -1 };

                    SoundEngine.PlaySound(style, Projectile.Center);
                }

                float scaleEaseValue = Easings.easeInOutBack(easeProg, 0f, 0f);
                swordScale = MathHelper.Lerp(0.55f, 1f, scaleEaseValue);

                float alphaEaseValue = Easings.easeOutSine(easeProg);
                swordAlpha = alphaEaseValue;
            } 

            float offsetRot = MathHelper.Lerp(startRot, goalRotation, Easings.easeOutCubic(easeProg));
            Vector2 offsetPos = player.MountedCenter + offsetRot.ToRotationVector2() * 50f * Easings.easeOutQuad(easeProg);

            float rotationEaseValue = Easings.easeOutCubic(easeProg);
            float easedRot = goalRotation + MathHelper.Lerp(MathHelper.TwoPi * 2.75f * player.direction, 0f, rotationEaseValue);


            #endregion

            #region pulseAnim

            int timeForChargeUp = 30;
            int timeBeforeChargeUp = 0;
            chargeUpProgress = Utils.GetLerpValue(timeForSpinIn + timeBeforeChargeUp, timeBeforeChargeUp + timeForSpinIn + timeForChargeUp, timer, true);

            int eswordTime = timer - (timeBeforeChargeUp + timeForSpinIn + timeForChargeUp);

            if (eswordTime == 0)
            {
                FlashSystem.SetCAFlashEffect(0.3f, 35, 1f, 0.9f, true, true);
                Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 80;

                SoundStyle style4 = new SoundStyle("AerovelenceMod/Sounds/Effects/EvilEnergy") with { Volume = .3f, Pitch = 0.3f, MaxInstances = -1 };
                SoundEngine.PlaySound(style4, player.Center);

                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .1f, Pitch = 0.15f, PitchVariance = .2f, MaxInstances = 1 };
                SoundEngine.PlaySound(style2, player.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/Tech/CyverBurst") with { Volume = 0.75f, Pitch = .15f, };
                SoundEngine.PlaySound(style, player.Center);

                SoundStyle style22 = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = .025f, Pitch = -.11f, };
                SoundEngine.PlaySound(style22, player.Center);

                //Dust
                for (int i = 0; i < 30; i++)
                {
                    float progress = (float)i / 30f;

                    Vector2 spawnPos = Projectile.Center + new Vector2(1f, 0f).RotatedBy(easedRot) * Main.rand.NextFloat(0, 270f * progress);
                    Vector2 smvel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(3f, 18f * (1f - progress));

                    Dust sm = Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlowPixelAlts>(), smvel, newColor: Color.HotPink * 1f, Scale: Main.rand.NextFloat(0.45f, 0.7f));
                    sm.alpha = 10;

                    sm.velocity.X *= 0.75f;
                    if (smvel.Y > 0)
                        sm.velocity.Y *= -1;

                    sm.velocity = sm.velocity.RotatedBy(easedRot + MathHelper.PiOver2);
                }

                for (int i = 0; i < 10; i++)
                {
                    float progress = (float)i / 10f;

                    Vector2 spawnPos = Projectile.Center + new Vector2(1f, 0f).RotatedBy(easedRot) * Main.rand.NextFloat(-30, 240f * progress);
                    Vector2 smvel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(7f, 18f * (1f - progress));

                    Dust d = Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlowStarSharp>(), smvel, newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.45f, 0.65f));
                    d.customData = DustBehaviorUtil.AssignBehavior_GSSBase(rotPower: 0.05f, timeBeforeSlow: 4, postSlowPower: 0.89f, velToBeginShrink: 1.5f, fadePower: 0.8f, colorFadePower: 1f);

                    d.velocity.X *= 0.75f;
                    if (smvel.Y > 0)
                        d.velocity.Y *= -1;

                    d.velocity = d.velocity.RotatedBy(easedRot + MathHelper.PiOver2);
                }

                player.GetModPlayer<OblivionBarPlayer>().barProgress = 0f;
                player.GetModPlayer<OblivionBarPlayer>().barVisualProgress = 1f;

                player.GetModPlayer<OblivionBarPlayer>().decreaseBar = true;
            }

            if (eswordTime > 0)
            {
                player.GetModPlayer<OblivionBarPlayer>().justShotPower = 1f;

                float eswordProg = Math.Clamp((eswordTime + 8f) / 25f, 0f, 1f); //timer / 50

                if (timer < 75)
                    energySwordWidth = MathHelper.Lerp(0f, 1f, Easings.easeInOutBack(eswordProg, 0f, 5f));
                else
                {
                    energySwordWidth = Math.Clamp(MathHelper.Lerp(energySwordWidth, -0.35f, 0.07f), 0f, 1f);
                    energySwordAlpha = Math.Clamp(MathHelper.Lerp(energySwordAlpha, -1f, 0.07f), 0f, 1f);
                }

                if (energySwordWidth > 0f)
                    swordAlpha = 0f;
                else
                    Projectile.active = false;
            }
            #endregion

            Projectile.rotation = easedRot;
            Projectile.Center = offsetPos;

            player.heldProj = Projectile.whoAmI;
            //player.ChangeDir(Projectile.Center.X < player.Center.X ? -1 : 1);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, offsetRot - MathHelper.PiOver2);
            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.timeLeft = 2;

            Projectile.velocity = Vector2.Zero;

            

            timer++;
        }

        float chargeUpProgress = 0f;

        float swordAlpha = 0f;
        float swordScale = 1f;


        float energySwordAlpha = 1f;
        float energySwordWidth = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Vector2 swordDrawPos = Projectile.Center - Main.screenPosition + new Vector2(-5f * player.direction, 0f);

            //Swirl
            if (timer < 20)
            {
                Texture2D Swirl = Mod.Assets.Request<Texture2D>("Assets/Slash/TerraOrbC").Value;
                Texture2D SwirlD = Mod.Assets.Request<Texture2D>("Assets/Slash/TerraSwingD").Value;

                //Texture2D Twirl = CommonTextures.PixelSwirl.Value;
                float twirlAlpha = Easings.easeInQuad(Utils.GetLerpValue(16, 4, timer, true)) * Easings.easeOutQuad(swordAlpha);

                float swirlRot1 = Projectile.rotation - 2f;
                float swirlRot2 = Projectile.rotation - 0.25f;

                Main.EntitySpriteDraw(Swirl, swordDrawPos, null, Color.DeepPink with { A = 0 } * twirlAlpha * 0.4f, swirlRot1, Swirl.Size() / 2, swordScale * 0.45f, 0);
                Main.EntitySpriteDraw(Swirl, swordDrawPos, null, Color.DeepPink with { A = 0 } * twirlAlpha * 0.4f, swirlRot2, Swirl.Size() / 2, swordScale * 0.45f, 0);

                Main.EntitySpriteDraw(SwirlD, swordDrawPos, null, Color.DeepPink with { A = 0 } * twirlAlpha * 0.2f, swirlRot1, SwirlD.Size() / 2, swordScale * 0.45f, 0);
                Main.EntitySpriteDraw(SwirlD, swordDrawPos, null, Color.DeepPink with { A = 0 } * twirlAlpha * 0.2f, swirlRot2, SwirlD.Size() / 2, swordScale * 0.45f, 0);

            }

            Texture2D Texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion/Oblivion").Value;
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion/OblivionHeldProj_Glow").Value;
            Texture2D GlowmaskWhite = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion/OblivionHeldProj_GlowWhite").Value;
            Texture2D White = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion/OblivionWhite").Value;

            float rot = Projectile.rotation + MathHelper.PiOver4;
            rot += player.direction == 1 ? 0 : -MathHelper.PiOver2;

            SpriteEffects SE = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            //Border
            Color between = Color.Lerp(Color.Pink, Color.HotPink, 0.5f);
            Color between2 = Color.Lerp(Color.DeepPink, Color.HotPink, 0.5f);


            swordDrawPos += Main.rand.NextVector2Circular(8f, 8f) * Easings.easeInQuad(chargeUpProgress);

            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(White, swordDrawPos + Main.rand.NextVector2CircularEdge(3f, 3f) * chargeUpProgress, null, between2 with { A = 0 } * chargeUpProgress * swordAlpha, rot, Texture.Size() / 2f, swordScale, SE, 0.0f);
            }

            Main.spriteBatch.Draw(Texture, swordDrawPos, null, lightColor * swordAlpha, rot, Texture.Size() / 2f, 1f * swordScale, SE, 0f);
            Main.spriteBatch.Draw(Glowmask, swordDrawPos, null, Color.White * swordAlpha, rot, Texture.Size() / 2f, 1f * swordScale, SE, 0f);

            Main.spriteBatch.Draw(White, swordDrawPos, null, Color.Pink with { A = 0 } * Easings.easeInQuad(chargeUpProgress) * swordAlpha * 2f, rot, Texture.Size() / 2f, 1f * swordScale, SE, 0f);



            #region energyBlade
            int playerDir = Main.player[Projectile.owner].direction;

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation);

            Vector2 otherOffset = new Vector2(playerDir == 1 ? 14 : 8,
                playerDir == 1 ? -8 : -14).RotatedBy(Projectile.rotation);

            Vector2 drawPos = armPosition + otherOffset - Main.screenPosition + new Vector2(0f, Main.player[Projectile.owner].gfxOffY);
            drawPos += new Vector2(-50f, 0f).RotatedBy(Projectile.rotation);

            Texture2D glow1 = circle_053.Value;
            Texture2D glow4 = circle_053Black.Value;

            Vector2 newScale = new Vector2(1.5f, 1f * energySwordWidth) * 0.5f; //sword
            newScale *= 0.55f;
            Vector2 newScale2 = new Vector2(1f, 1.5f * energySwordWidth) * 0.5f; //sword
            newScale2 *= 0.55f;
            Vector2 newScale3 = new Vector2(1.5f, 0.35f * energySwordWidth) * 0.5f; //sword
            newScale3 *= 0.55f;

            Vector2 origin1 = new Vector2(0f, glow1.Height / 2f);


            //Black Base
            Main.spriteBatch.Draw(glow1, drawPos, null, Color.Black * 0.15f * energySwordAlpha, Projectile.rotation, origin1, newScale3, SpriteEffects.None, 0f);

            //Bloom
            Main.spriteBatch.Draw(glow4, drawPos, null, Color.DeepPink with { A = 0 } * 0.15f * energySwordAlpha, Projectile.rotation, origin1, newScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(glow4, drawPos, null, Color.DeepPink with { A = 0 } * 0.15f * energySwordAlpha, Projectile.rotation, origin1, newScale2, SpriteEffects.None, 0f);

            //Use Dusts layer so we can draw on top of black underglow
            ModContent.GetInstance<NewAdditivePixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                BladeDraw();
            });
            #endregion



            return false;
        }

        Effect myEffect = null;
        public void BladeDraw()
        {
            if (energySwordAlpha == 0 || energySwordWidth == 0)
                return;

            Texture2D Glorb = circle_053.Value;
            Texture2D Spike = muzzle_flash_12.Value;
            Texture2D Star = star_07.Value;

            float ySinVal = (float)Math.Sin(Main.timeForVisualEffects * 0.22f) * 0.15f;
            float xSinVal = (float)Math.Sin(Main.timeForVisualEffects * 0.22f) * 0.05f;

            //re-name these 
            Vector2 BladeScale = new Vector2(1.5f, 1f * energySwordWidth) * 0.5f; //sword
            Vector2 SpikeScale = new Vector2(0.75f, (1.3f + ySinVal) * energySwordWidth) * (0.5f + xSinVal); //spiky
            Vector2 HiltScale = new Vector2(0.25f * energySwordWidth, 0.25f); //Hilt

            BladeScale *= 0.51f;
            SpikeScale *= 0.51f;
            HiltScale *= 0.55f;

            Vector2 origin1 = new Vector2(0f, Glorb.Height / 2f);
            Vector2 origin2 = new Vector2(0f, Spike.Height / 2f);

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation);

            int playerDir = Main.player[Projectile.owner].direction;
            Vector2 otherOffset = new Vector2(playerDir == 1 ? 14 : 8,
                playerDir == 1 ? -8 : -14).RotatedBy(Projectile.rotation);

            Vector2 drawPos = armPosition + otherOffset - Main.screenPosition + new Vector2(0f, Main.player[Projectile.owner].gfxOffY);
            float rot = Projectile.rotation;

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaser", AssetRequestMode.ImmediateLoad).Value;

            #region ShaderParams
            myEffect.Parameters["sampleTexture1"].SetValue(CommonTextures.Extra_196_Black.Value);
            myEffect.Parameters["sampleTexture2"].SetValue(CommonTextures.Trail5Loop.Value);
            myEffect.Parameters["sampleTexture3"].SetValue(CommonTextures.FlameTrail.Value);
            myEffect.Parameters["sampleTexture4"].SetValue(CommonTextures.ThinGlowLine.Value);

            Color c1 = Color.DeepPink;
            Color c2 = Color.DeepPink;
            Color c3 = Color.DeepPink;
            Color c4 = Color.DeepPink;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color3"].SetValue(c3.ToVector4());
            myEffect.Parameters["Color4"].SetValue(c4.ToVector4());

            myEffect.Parameters["Color1Mult"].SetValue(1.5f);
            myEffect.Parameters["Color2Mult"].SetValue(1.5f);
            myEffect.Parameters["Color3Mult"].SetValue(1.5f); //1.5
            myEffect.Parameters["Color4Mult"].SetValue(1.1f);
            myEffect.Parameters["totalMult"].SetValue(1f * energySwordAlpha);

            myEffect.Parameters["tex1reps"].SetValue(1f);
            myEffect.Parameters["tex2reps"].SetValue(1f);
            myEffect.Parameters["tex3reps"].SetValue(1f);
            myEffect.Parameters["tex4reps"].SetValue(1f);

            myEffect.Parameters["satPower"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.03f);
            #endregion

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.EffectMatrix);

            #region blade
            //MainBlade
            Main.spriteBatch.Draw(Glorb, drawPos + new Vector2(-60f, 0f).RotatedBy(rot), null, Color.White, rot, origin1, BladeScale, SpriteEffects.None, 0f);

            //Spiky part near guard
            Main.spriteBatch.Draw(Spike, drawPos + Main.rand.NextVector2Circular(1f, 1f), null, Color.White, rot, origin2, SpikeScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Spike, drawPos + Main.rand.NextVector2Circular(1f, 1f), null, Color.White, rot, origin2, SpikeScale * 0.5f, SpriteEffects.FlipVertically, 0f);

            //"Hilt"
            Vector2 off = rot.ToRotationVector2() * 8f;
            Main.spriteBatch.Draw(Star, drawPos + Main.rand.NextVector2Circular(1f, 1f) + off, null, Color.White, rot + MathHelper.PiOver2, Star.Size() / 2, HiltScale, SpriteEffects.FlipVertically, 0f);
            Main.spriteBatch.Draw(Star, drawPos + Main.rand.NextVector2Circular(1f, 1f) + off, null, Color.White, rot - MathHelper.PiOver2, Star.Size() / 2, HiltScale, SpriteEffects.FlipVertically, 0f);
            #endregion

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }
    }

    //For storing information about the Oblivion's Bar
    public class OblivionBarPlayer : ModPlayer
    {
        public float barProgress = 0f;

        public float justShotPower = 0f;

        public float barVisualProgress = 0f;

        public float barFadeIn = 0f;

        public bool decreaseBar = false;

        public int decreaseTimer = 0;

        //public override void ResetEffects() { ResetVariables(); }
        public override void UpdateDead() { ResetVariables(); }
        private void ResetVariables()
        {
            barProgress = 0f;
            barFadeIn = 0f;
        }

        public override void PostUpdateMiscEffects() { Update(); }

        private void Update()
        {
            if (Player.HeldItem.type != ModContent.ItemType<Oblivion>())
                barFadeIn = 0f;
            else
                barFadeIn = Math.Clamp(MathHelper.Lerp(barFadeIn, 1.2f, 0.12f), 0f, 1f);

            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.5f, 0.06f), 0f, 1f); //-0.5 | 0.08

            if (decreaseBar)
            {
                barVisualProgress = 1f - Utils.GetLerpValue(0, 300, decreaseTimer, true); //360 -> 300

                decreaseTimer++;

                if (barVisualProgress == 0f)
                {
                    decreaseTimer = 0;
                    decreaseBar = false;
                }
            }
        }
    }

    //Drawing the bar
    public class OblivionBarDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.HeldItem);
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Player.HeldItem.type != ModContent.ItemType<Oblivion>())
                return;

            float barProgress = Player.GetModPlayer<OblivionBarPlayer>().barProgress;
            float barVisualProgress = Player.GetModPlayer<OblivionBarPlayer>().barVisualProgress;
            float justShotPower = Player.GetModPlayer<OblivionBarPlayer>().justShotPower;
            float barFadeIn = Player.GetModPlayer<OblivionBarPlayer>().barFadeIn;


            CyverBarUtils.DrawCyverBar(drawInfo, barProgress, barVisualProgress, justShotPower, barFadeIn);
        }
    }

}
