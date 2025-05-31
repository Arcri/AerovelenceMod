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
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles;
using Terraria.Audio;
using static Terraria.NPC;
using System.IO;
using static AerovelenceMod.Common.Utilities.DustBehaviorUtil;
using AerovelenceMod.Common.Systems;
using System.Runtime.Intrinsics.Arm;
using Terraria.ModLoader.IO;
using ReLogic.Content;
using AerovelenceMod.Common;
using static tModPorter.ProgressUpdate;


namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry
{
    public class Oblivion : ModItem
    {
        bool tick = false;
        public override void SetDefaults()
        {
            Item.damage = 89;
            Item.knockBack = KnockbackTiers.Average;

            Item.width = 60;
            Item.height = 68;
            Item.crit = 2;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            //Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = 0.5f, Pitch = 0.8f };
            Item.useStyle = ItemUseStyleID.Swing;
            
            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<OblivionHeldProjectile>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            tick = !tick;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));
            return false;
        }

    }
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

            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.scale = 1f;
            Projectile.ownerHitCheck = true;
            Projectile.extraUpdates = 3;
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f) && justHitTime <= -1;
            return shouldDamage;
        }

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
                SoundEngine.PlaySound(SoundID.Item71 with { Pitch = -0.35f, PitchVariance = 0.15f, Volume = 0.65f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing with { Volume = 0.7f, Pitch = 0.5f, PitchVariance = 0.1f }, Projectile.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/GGS/Swing_Sword_Sharp_M_a") with { Pitch = -.82f, PitchVariance = .16f, Volume = 0.10f };
                SoundEngine.PlaySound(style, Projectile.Center);
                playedSound = true;
            }

            if (timer % 1 == 0 && justHitTime <= 0)
            {
                previousRotations.Add(Projectile.rotation);

                if (previousRotations.Count > 17)
                    previousRotations.RemoveAt(0);
            }

            //Dust
            int dustMod = 1;// (int)Math.Clamp(4f - (2f * (Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1f)), 0, 5);
            if (timer % dustMod == 0 && (getProgress(easingProgress) >= 0.2f && getProgress(easingProgress) <= 0.8f) && justHitTime <= 0)
            {
                Dust d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + currentAngle.ToRotationVector2() * Main.rand.NextFloat(50f, 100f), ModContent.DustType<PixelGlowOrb>(),
                    currentAngle.ToRotationVector2().RotatedByRandom(0.3f).RotatedBy(MathHelper.PiOver2 * (Projectile.ai[0] > 0 ? 1 : -1)) * -Main.rand.NextFloat(2f, 5f),
                    0, newColor: Color.DeepPink, Main.rand.NextFloat(0.45f, 0.65f));
                d.scale *= Projectile.scale;

                d.customData = DustBehaviorUtil.AssignBehavior_PGOBase(postSlowPower: 0.9f, velToBeginShrink: 2.5f, fadePower: 0.9f);
            }

            justHitTime--;
        }

        public List<float> previousRotations = new List<float>();

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion").Value;
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/OblivionHeldProj_Glow").Value;
            Texture2D White = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/OblivionWhite").Value;

            float progBoost = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            //float x = MathF.Sin((float)Main.timeForVisualEffects * 0.01f) * 10;

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle);
            Vector2 otherOffset = new Vector2(
                Projectile.spriteDirection == 1 ? 4 : 0, 
                Projectile.spriteDirection == 1 ? -8 : -10).RotatedBy(currentAngle);

            Vector2 drawPos = armPosition + otherOffset - Main.screenPosition + new Vector2(0f, Main.player[Projectile.owner].gfxOffY);

            Vector2 origin = Projectile.ai[0] != 1 ? new Vector2(0f, Texture.Height) : new Vector2(Texture.Width, Texture.Height);
            float rotationOffset = Projectile.ai[0] != 1 ? 0f : MathHelper.PiOver2;
            SpriteEffects effects = Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float scaleBoost = ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f);

            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                SlashDraw();
            });


            for (int i = 0; i < 5; i++)
            {
                Vector2 randPos = Main.rand.NextVector2Circular(5f, 5f);
                Main.spriteBatch.Draw(White, drawPos + randPos, null, Color.HotPink with { A = 0 } * progBoost * 0.65f, Projectile.rotation + rotationOffset, origin, Projectile.scale + scaleBoost, effects, 0f);
            }

            Main.spriteBatch.Draw(Texture, drawPos, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + scaleBoost, effects, 0f);
            Main.spriteBatch.Draw(Glowmask, drawPos, null, Color.White, Projectile.rotation + rotationOffset, origin, Projectile.scale + scaleBoost, effects, 0f);

            return false;
        }

        public void SlashDraw()
        {
            Texture2D Texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/Oblivion").Value;
            Texture2D WhiteGlow = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/OblivionWhiteGlow").Value;

            float progBoost = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            //float x = MathF.Sin((float)Main.timeForVisualEffects * 0.01f) * 10;

            Vector2 armPosition = Main.player[Projectile.owner].GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, currentAngle);
            Vector2 otherOffset = new Vector2(
                Projectile.spriteDirection == 1 ? 4 : 0,
                Projectile.spriteDirection == 1 ? -8 : -10).RotatedBy(currentAngle);

            Vector2 drawPos = armPosition + otherOffset - Main.screenPosition + new Vector2(0f, Main.player[Projectile.owner].gfxOffY);

            Vector2 origin = Projectile.ai[0] != 1 ? new Vector2(0f, Texture.Height) : new Vector2(Texture.Width, Texture.Height);
            float rotationOffset = Projectile.ai[0] != 1 ? 0f : MathHelper.PiOver2;
            SpriteEffects effects = Projectile.ai[0] != 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float scaleBoost = ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f);

            #region slash
            Texture2D Slash = Mod.Assets.Request<Texture2D>("Assets/Slash/pixelKennySlashBlack").Value;
            Vector2 SlashPos = Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(10f * (float)Math.Sin(MathHelper.Pi * getProgress(easingProgress)), 0).RotatedBy(originalAngle);


            float slashScale = 0.65f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 1.1f);
            slashScale *= 0.5f;

            Color betweenPink = Color.Lerp(Color.DeepPink, Color.HotPink, 0.25f);
            Color slashColor = Color.Lerp(Color.Black * 0.3f, betweenPink, Easings.easeInOutCirc(progBoost));

            Main.spriteBatch.Draw(Slash, SlashPos, null, slashColor with { A = 0 } * progBoost, originalAngle + MathHelper.PiOver2, Slash.Size() / 2, slashScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Slash, SlashPos, null, Color.Pink with { A = 0 } * progBoost * 0.15f, originalAngle + MathHelper.PiOver2, Slash.Size() / 2, slashScale, SpriteEffects.None, 0f);
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
            justHitTime = (7 - (int)((Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1) * 7f)) * Projectile.extraUpdates; //6

            float currentShakePower = Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower;
            Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = currentShakePower > 1 ? Math.Clamp(currentShakePower, 3, 7) : 7;

            Vector2 orthToSwing = (MathHelper.PiOver2 + currentAngle).ToRotationVector2() * (Projectile.ai[0] == 1 ? -1 : 1f);

            for (int i = 0; i < 7 + Main.rand.Next(0, 5); i++)
            {

                Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<PixelGlowOrb>(), newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.65f, 0.9f));
                d.velocity = orthToSwing * Main.rand.NextFloat(0.5f, 4f);
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-2.05f, 2.05f));

                d.customData = AssignBehavior_PGOBase(rotPower: 0.04f, timeBeforeSlow: 5, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.8f, colorFadePower: 1f, glowIntensity: 0.4f);

                //StarDustDrawInfo info = new StarDustDrawInfo(true, false, true, true, false, 1f);
                //d.customData = AssignBehavior_GSSBase(rotPower: 0.04f, timeBeforeSlow: 5, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.8f, shouldFadeColor: false, sdci: info);

            }

            for (int i = 0; i < 7; i++)
            {

                //Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<RoaParticle>(), newColor: Color.DeepPink, Scale: 0.55f + Main.rand.NextFloat(-0.2f, 0.2f));
                //d.velocity = orthToSwing * Main.rand.NextFloat(1f, 5f);
                //d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-1.05f, 1.05f));

                Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<MuraLineBasic>(), newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.3f, 0.45f));
                d.velocity = orthToSwing * Main.rand.NextFloat(2f, 6f);
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
                d.alpha = 10 + Main.rand.Next(-5, 5);

            }

            SoundEngine.PlaySound(SoundID.Item94 with { Volume = 0.35f, Pitch = 0.4f, PitchVariance = 0.4f }, target.Center);

            SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/ElectricExplode") with { Volume = 0.18f, Pitch = 0.45f, PitchVariance = 0.1f, MaxInstances = -1, };
            SoundEngine.PlaySound(style, Projectile.Center);

            Color betweenPink = Color.Lerp(Color.DeepPink, Color.HotPink, 0.75f);

            float fxRot = (target.Center - Main.player[Projectile.owner].Center).ToRotation();

            Dust star1 = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowStarSharp>(), Velocity: Vector2.Zero, newColor: Color.DeepPink, Scale: 2.25f);
            Dust star2 = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowStarSharp>(), Velocity: Vector2.Zero, newColor: Color.White, Scale: 1.5f);
            star1.rotation = star2.rotation = fxRot;

            star1.customData = star2.customData = DustBehaviorUtil.AssignBehavior_GSSBase(fadePower: 0.88f);


            CirclePulseBehavior cpb2 = new CirclePulseBehavior(0.55f, true, 3, 0.8f, 0.8f);

            Dust d1 = Dust.NewDustPerfect(target.Center, ModContent.DustType<CirclePulse>(), Velocity: Vector2.Zero, newColor: betweenPink * 0.35f);
            d1.scale = 0.2f;
            d1.customData = cpb2;
            d1.velocity = new Vector2(-0.01f, 0f).RotatedBy(fxRot);

            Dust d2 = Dust.NewDustPerfect(target.Center, ModContent.DustType<CirclePulse>(), Velocity: Vector2.Zero, newColor: betweenPink * 0.35f);
            d2.customData = cpb2;
            d2.velocity = new Vector2(0.01f, 0f).RotatedBy(fxRot);
        }


        // Find the start and end of the sword and use a line collider to check for collision with enemies
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Main.player[Projectile.owner].MountedCenter;
            Vector2 end = start + currentAngle.ToRotationVector2() * ((Projectile.Size.Length() * 1.2f) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
        }

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
            Projectile.extraUpdates = 6;
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.3f && getProgress(easingProgress) <= 0.75f) && justHitTime <= -1;
            return shouldDamage;
        }

        bool playedSound = false;
        public override void AI()
        {
            SwingHalfAngle = 190;
            easingAdditionAmount = 0.024f / Projectile.extraUpdates;
            frameToStartSwing = 8 * Projectile.extraUpdates;
            timeAfterEnd = 5;
            startingProgress = 0.01f;
            offset = 55;

            StandardHeldProjCode();
            StandardSwingUpdate();

            if (timer == 2)
            {
                for (int i = 0; i < 20; i++)
                {
                    float progress = (float)i / 20f;

                    Vector2 spawnPos = Projectile.Center + new Vector2(1f, 0f).RotatedBy(currentAngle) * Main.rand.NextFloat(0, 280f * progress);
                    Vector2 smvel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(3f, 18f * (1f - progress));

                    Dust sm = Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlowPixelAlts>(), smvel, newColor: Color.Pink * 1f, Scale: Main.rand.NextFloat(0.45f, 0.7f));
                    sm.alpha = 10;

                    sm.velocity.X *= 0.75f;
                    if (smvel.Y > 0)
                        sm.velocity.Y *= -1;

                    sm.velocity = sm.velocity.RotatedBy(currentAngle + MathHelper.PiOver2);

                    //GlowPixelAltBehavior bev = new GlowPixelAltBehavior();
                    //bev.base_fadeOutPower = 0.9f;
                    //sm.customData = bev;
                }
            }

            //Sound
            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Killed_56") with { Volume = 0.25f, Pitch = 0.9f, PitchVariance = .11f, MaxInstances = -1 };
                SoundEngine.PlaySound(style, Projectile.Center);
                SoundStyle style2 = new SoundStyle("Terraria/Sounds/NPC_Killed_55") with { Volume = 0.25f, Pitch = 0.55f, PitchVariance = .15f, MaxInstances = -1 };
                SoundEngine.PlaySound(style2, Projectile.Center);

                SoundStyle style3 = new SoundStyle("AerovelenceMod/Sounds/Effects/EvilEnergy") with { Volume = 0.4f, Pitch = 1f, PitchVariance = 0.05f, MaxInstances = -1 }; 
                SoundEngine.PlaySound(style3, Projectile.Center);

                playedSound = true;
            }

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
            int dustMod = 2;// (int)Math.Clamp(4f - (2f * (Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1f)), 0, 5);
            if (timer % dustMod == 0 && (getProgress(easingProgress) >= 0.1f && getProgress(easingProgress) <= 0.9f) && justHitTime <= 0)
            {
                Dust d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + currentAngle.ToRotationVector2() * Main.rand.NextFloat(50f, 215f), ModContent.DustType<GlowPixelCross>(),
                    currentAngle.ToRotationVector2().RotatedByRandom(0.3f).RotatedBy(MathHelper.PiOver2 * (Projectile.ai[0] > 0 ? 1 : -1)) * -Main.rand.NextFloat(2f, 5f),
                    0, newColor: Color.DeepPink, Main.rand.NextFloat(0.45f, 0.65f));
                d.scale *= Projectile.scale;

                d.customData = AssignBehavior_GPCBase(postSlowPower: 0.9f, velToBeginShrink: 2.5f, fadePower: 0.9f);
            }

            justHitTime--;
        }

        public List<float> previousRotations = new List<float>();

        float overallAlpha = 1f;
        float overallWidth = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            float progBoost = (float)Math.Sin(getProgress(easingProgress) * Math.PI);


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

            SlashDraw();

            //Use Dusts layer so we can draw on top of black underglow
            ModContent.GetInstance<NewAdditivePixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                BladeDraw();
            });

            return false;
        }

        public void SlashDraw()
        {
            float progBoost = (float)Math.Sin(getProgress(easingProgress) * Math.PI);

            #region slash
            Texture2D Slash = Mod.Assets.Request<Texture2D>("Assets/Slash/pixelKennySlashBlack").Value;
            Vector2 SlashPos = Main.player[Projectile.owner].Center - Main.screenPosition + new Vector2(10f * (float)Math.Sin(MathHelper.Pi * getProgress(easingProgress)), 0).RotatedBy(currentAngle);


            float slashScale = 0.65f + ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 1.1f);
            slashScale *= 1f;

            Color betweenPink = Color.Lerp(Color.DeepPink, Color.Pink, 0.45f);
            Color slashColor = Color.Lerp(Color.Black * 0.3f, betweenPink, Easings.easeInOutCirc(progBoost));

            Main.spriteBatch.Draw(Slash, SlashPos, null, slashColor with { A = 0 } * Easings.easeInCirc(progBoost) * 0.4f, currentAngle + MathHelper.PiOver2, Slash.Size() / 2, slashScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Slash, SlashPos, null, slashColor with { A = 0 } * Easings.easeInCirc(progBoost) * 0.4f, currentAngle + MathHelper.PiOver2, Slash.Size() / 2, slashScale * 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(Slash, SlashPos, null, slashColor with { A = 0 } * Easings.easeInCirc(progBoost) * 0.4f, currentAngle + MathHelper.PiOver2, Slash.Size() / 2, slashScale * 0.5f, SpriteEffects.None, 0f);

            #endregion
        }

        Effect myEffect = null;
        public void BladeDraw()
        {
            Texture2D Glorb = circle_053.Value;
            Texture2D Spike = muzzle_flash_12.Value;
            Texture2D Star = star_07.Value;

            float ySinVal = (float)Math.Sin(Main.timeForVisualEffects * 0.22f) * 0.15f;
            float xSinVal = (float)Math.Sin(Main.timeForVisualEffects * 0.22f) * 0.05f;

            //re-name these 
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
            
            Main.spriteBatch.Draw(slash, drawPos + rot.ToRotationVector2() * (20f * slashOpacity), null, Color.White, slashRot, slash.Size() / 2f, slashScale, SlashSE, 0f);
            Main.spriteBatch.Draw(slash, drawPos + rot.ToRotationVector2() * (20f * slashOpacity), null, Color.White, slashRot, slash.Size() / 2f, slashScale, SlashSE, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        public override void OnHitNPC(NPC target, HitInfo hit, int damageDone)
        {
            //Want less hitpause at higher attack speeds
            justHitTime = (12 - (int)((Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1) * 12f)) * Projectile.extraUpdates; //6

            float currentShakePower = Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower;
            Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = currentShakePower > 1 ? Math.Clamp(currentShakePower, 13, 17) : 17;

            Vector2 orthToSwing = (MathHelper.PiOver2 + currentAngle).ToRotationVector2() * (Projectile.ai[0] == 1 ? -1 : 1f);

            for (int i = 0; i < 17 + Main.rand.Next(0, 5); i++)
            {

                Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<PixelGlowOrb>(), newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.65f, 0.9f));
                d.velocity = orthToSwing * Main.rand.NextFloat(0.5f, 8f);
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-2.05f, 2.05f));

                d.customData = AssignBehavior_PGOBase(rotPower: 0.04f, timeBeforeSlow: 5, postSlowPower: 0.89f, velToBeginShrink: 1f, fadePower: 0.8f, colorFadePower: 1f, glowIntensity: 0.4f);
            }

            for (int i = 0; i < 17; i++)
            {
                Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<MuraLineBasic>(), newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.3f, 0.45f));
                d.velocity = orthToSwing * Main.rand.NextFloat(2f, 9f);
                d.velocity = d.velocity.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
                d.alpha = 10 + Main.rand.Next(-5, 5);

            }

           
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

}
