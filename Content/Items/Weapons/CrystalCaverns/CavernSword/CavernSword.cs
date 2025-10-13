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
using Terraria.Graphics;
using AerovelenceMod.Content.Projectiles;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common;
using AerovelenceMod.Common.Globals.Players;


namespace AerovelenceMod.Content.Items.Weapons.CrystalCaverns.CavernSword
{
    public class CavernSword : ModItem
    {
        bool tick = false;
        public override void SetDefaults()
        {
            Item.damage = 89;
            Item.knockBack = 4f;// KnockbackTiers.Average;

            Item.width = 60;
            Item.height = 68;
            Item.crit = 2;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            //Item.UseSound = SoundID.DD2_MonkStaffSwing with { Volume = 0.5f, Pitch = 0.8f };
            Item.useStyle = ItemUseStyleID.Swing;

            Item.channel = true;

            Item.shootSpeed = 1f;
            Item.shoot = ModContent.ProjectileType<CavernSwordProj>();
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
                type = ModContent.ProjectileType<CavernSwordProj>();
            else
                type = ModContent.ProjectileType<CavernSwordWhatDoICallThis>();

            tick = !tick;

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, (tick ? 1 : 0));

            return false;
        }
    }
    public class CavernSwordProj : BaseSwingSwordProj
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
            Projectile.extraUpdates = 10;
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.2f && getProgress(easingProgress) <= 0.75f) && justHitTime <= -1;
            return shouldDamage;
        }

        bool playedSound = false;
        public override void AI()
        {
            SwingHalfAngle = 155; //125
            easingAdditionAmount = 0.075f / Projectile.extraUpdates; //0.09
            frameToStartSwing = 1 * Projectile.extraUpdates; //3
            timeAfterEnd = 1 * Projectile.extraUpdates; //2
            startingProgress = 0.02f; //0.02

            offset = 55;

            StandardHeldProjCode();
            StandardSwingUpdate();

            Player player = Main.player[Projectile.owner];


            //Trail info
            Vector2 gfxOffset = new Vector2(0, Main.player[Projectile.owner].gfxOffY);

            if (justHitTime <= 0)
            {
                int trailCount = 30;
                relativeRots.Add(currentAngle + MathHelper.PiOver2); //
                relativePoss.Add((Projectile.Center + currentAngle.ToRotationVector2() * 10f) - player.Center - gfxOffset); //90

                if (relativeRots.Count > trailCount)
                    relativeRots.RemoveAt(0);

                if (relativePoss.Count > trailCount)
                    relativePoss.RemoveAt(0);
            }

            trailPoss.Clear();
            foreach (Vector2 pos in relativePoss)
            {
                trailPoss.Add(pos + player.Center + gfxOffset);
            }


            //Sound
            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {
                

                playedSound = true;
            }


            //Dust
            int dustMod = 5;
            if (timer % dustMod == 0 && (getProgress(easingProgress) >= 0.15f && getProgress(easingProgress) <= 0.85f) && justHitTime <= 0)
            {
                //Dust d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + currentAngle.ToRotationVector2() * Main.rand.NextFloat(35f, 85f), ModContent.DustType<GlowPixelCross>(),
                //    currentAngle.ToRotationVector2().RotatedByRandom(0.18f).RotatedBy(MathHelper.PiOver2 * (Projectile.ai[0] > 0 ? 1 : -1)) * -Main.rand.NextFloat(2f, 5f),
                //    0, newColor: Color.DeepSkyBlue, Main.rand.NextFloat(0.25f, 0.35f));

                //d.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, postSlowPower: 0.9f, velToBeginShrink: 2.5f, fadePower: 0.85f, shouldFadeColor: false);

                if (Main.rand.NextBool(2))
                {
                    Color between = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.5f);

                    Dust d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + currentAngle.ToRotationVector2() * Main.rand.NextFloat(35f, 85f), ModContent.DustType<GlowPixelCross>(),
                        currentAngle.ToRotationVector2().RotatedByRandom(0.18f).RotatedBy(MathHelper.PiOver2 * (Projectile.ai[0] > 0 ? 1 : -1)) * -Main.rand.NextFloat(2f, 5f),
                        0, newColor: between, Main.rand.NextFloat(0.25f, 0.35f));

                    d.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, postSlowPower: 0.9f, velToBeginShrink: 2.5f, fadePower: 0.85f, shouldFadeColor: false);
                }
                else
                {

                    Dust d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + currentAngle.ToRotationVector2() * Main.rand.NextFloat(35f, 75f), ModContent.DustType<ElectricSparkGlow>(),
                        currentAngle.ToRotationVector2().RotatedByRandom(0.18f).RotatedBy(MathHelper.PiOver2 * (Projectile.ai[0] > 0 ? 1 : -1)) * -Main.rand.NextFloat(2f, 5f),
                        0, newColor: Color.DeepSkyBlue, Main.rand.NextFloat(0.5f, 0.75f) * 2f);

                    ElectricSparkBehavior esb = new ElectricSparkBehavior(FadeAlphaPower: 0.89f, FadeScalePower: 0.87f, FadeVelPower: 0.92f, Pixelize: true, XScale: 1f, YScale: 0.75f); //0.91
                    d.customData = esb;
                }
            }

            justHitTime--;
        }

        public List<float> previousRotations = new List<float>();

        float overallAlpha = 1f;
        float overallScale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/CrystalCaverns/CavernSword/CavernSword").Value;

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

            float scaleBoost = (float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f;


            #region swingTex
            Texture2D SwingTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Slash/FullSlashTinyBlack");

            float glowIntensity = 1f;
            if (getProgress(easingProgress) <= 0.30f)
                glowIntensity = getProgress(easingProgress) / 0.30f;
            else if (getProgress(easingProgress) >= 0.70f)
                glowIntensity = 0.2f - ((getProgress(easingProgress) - 0.7f) / 0.30f);

            float easedGlowIntensity = getProgress(easingProgress) <= 0.5f ? Easings.easeInCirc(glowIntensity) : Easings.easeOutCirc(glowIntensity);

            Color rainbowCol = Color.DeepSkyBlue with { A = 0 } * easedGlowIntensity * 0.25f;

            //SwingTex
            if (getProgress(easingProgress) > 0.0f && getProgress(easingProgress) < 0.99f)
            {
                Vector2 swingTexPos = drawPos + new Vector2(2f, 0).RotatedBy(currentAngle);

                float[] slashScales = { 1f + scaleBoost, 1.1f + scaleBoost, 0.7f + scaleBoost };

                float swingRot = currentAngle + MathHelper.PiOver2;

                Main.spriteBatch.Draw(SwingTex, swingTexPos, null, rainbowCol * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.75f), swingRot, SwingTex.Size() / 2, slashScales[0], SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(SwingTex, swingTexPos, null, rainbowCol * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.5f), swingRot, SwingTex.Size() / 2, slashScales[1], SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(SwingTex, swingTexPos, null, rainbowCol * ((float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f), swingRot, SwingTex.Size() / 2, slashScales[2], SpriteEffects.None, 0f);
            }
            #endregion


            for (int i = 0; i < 5; i++)
            {
                Vector2 randPos = Main.rand.NextVector2Circular(5f, 5f);
                Main.spriteBatch.Draw(Texture, drawPos + randPos, null, Color.SkyBlue with { A = 0 } * progBoost * 0.65f, Projectile.rotation + rotationOffset, origin, Projectile.scale + scaleBoost, effects, 0f);
            }

            Main.spriteBatch.Draw(Texture, drawPos, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + scaleBoost, effects, 0f);

            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                TrailDraw(false);
            });
            TrailDraw(true);

            return false;
        }

        List<float> relativeRots = new List<float>();
        List<Vector2> relativePoss = new List<Vector2>();

        List<Vector2> trailPoss = new List<Vector2>();

        float trailWidth = 1f;
        float trailAlpha = 1f;
        Effect trailEffect = null;
        public void TrailDraw(bool giveUp)
        {
            if (giveUp)
                return;

            Texture2D trailTexture = Mod.Assets.Request<Texture2D>("Assets/Trails/EvenThinnerGlowLine").Value; //
            Texture2D trailTexture2 = CommonTextures.Extra_196_Black.Value; //

            if (trailEffect == null)
                trailEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;


            //Convert lists to arrays for use in vertex strip
            Vector2[] pos_arr = trailPoss.ToArray();
            float[] rot_arr = relativeRots.ToArray();


            float sineWidthMult = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.3f) * 0f;


            Color StripColor(float progress) => Color.White * Easings.easeInSine(progress);

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

                return toReturn * sineWidthMult * trailWidth * 45f; //50
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

                return toReturn * sineWidthMult * trailWidth * 60f; //50 | 130
            }


            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStrip2 = new VertexStrip();
            vertexStrip2.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth2, -Main.screenPosition, includeBacksides: true);


            trailEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            trailEffect.Parameters["progress"].SetValue((float)Main.timeForVisualEffects * 0.035f); //0.02
            trailEffect.Parameters["reps"].SetValue(1f);


            //Over layer
            Color betweenPink = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.15f);
            trailEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            trailEffect.Parameters["ColorOne"].SetValue(betweenPink.ToVector3() * 2f); //2f
            trailEffect.Parameters["glowThreshold"].SetValue(1f);
            trailEffect.Parameters["glowIntensity"].SetValue(1f);
            trailEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip.DrawTrail();

            //UnderLayer
            trailEffect.Parameters["TrailTexture"].SetValue(trailTexture2);
            trailEffect.Parameters["glowThreshold"].SetValue(1f);
            trailEffect.Parameters["glowIntensity"].SetValue(1f);
            trailEffect.Parameters["ColorOne"].SetValue(Color.SkyBlue.ToVector3() * 4.5f);
            trailEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip2.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        public override void OnHitNPC(NPC target, HitInfo hit, int damageDone)
        {
            //Want less hitpause at higher attack speeds
            justHitTime = (4 - (int)((Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1) * 4f)) * Projectile.extraUpdates; //10

            Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower += 4;

            //The dust....
            int crossCount = 12;
            for (int i = 220; i < crossCount; i++)
            {
                float dir = (MathHelper.TwoPi / (float)crossCount) * i;

                Vector2 dustVel = dir.ToRotationVector2() * Main.rand.NextFloat(2.5f, 5f);
                dustVel = dustVel.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f));

                Color middleBlue = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.25f + Main.rand.NextFloat(-0.15f, 0.15f));

                Dust gd = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: middleBlue, Scale: Main.rand.NextFloat(0.25f, 0.55f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 0,
                    preSlowPower: 0.94f, postSlowPower: 0.9f, velToBeginShrink: 2.5f, fadePower: 0.89f, shouldFadeColor: false);
            }
        }


        // Find the start and end of the sword and use a line collider to check for collision with enemies
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Main.player[Projectile.owner].MountedCenter + currentAngle.ToRotationVector2() * (20f * Projectile.scale);
            Vector2 end = start + currentAngle.ToRotationVector2() * (60f * Projectile.scale);

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
                toReturn = (float)(Math.Pow(2, (10 * x) - 5)) / 2;
            }
            else if (x > 0.5)
            {
                toReturn = (float)(2 - Math.Pow(2, (-10 * x) + 5)) / 2;
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

    public class CavernSwordWhatDoICallThis : ModProjectile
    {
        int timer = 0;
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;

            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;

        //How far away the projectile will be held by the player
        float offsetAmount = 0f;

        //The progress of the fade in animation (1f = done)
        float fadeInProgress = 0f;

        float recoilProg = 0f; //1f = most recoil

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            #region startAnim
            int timeForFadeInAnim = 14;

            //Starting Animation
            if (fadeInProgress < 1f)
            {
                fadeInProgress = (float)timer / timeForFadeInAnim;

                float offsetEaseValue = Easings.easeOutQuart(fadeInProgress);
                offsetAmount = MathHelper.Lerp(-10f, 34f, offsetEaseValue);

                float scaleEaseValue = Easings.easeInOutBack(fadeInProgress, 0f, 2f);
                overallScale = MathHelper.Lerp(0.6f, 1f, scaleEaseValue);

                float alphaEaseValue = Easings.easeOutQuart(fadeInProgress);
                overallAlpha = alphaEaseValue;
            }
            #endregion

            #region chargeUpAnim
            int timeForChargeUp = 40;
            int timeAtFullCharge = 30;
            int timeToUse = (timer - timeForFadeInAnim);

            chargeVal = Utils.GetLerpValue(timeForFadeInAnim, timeForFadeInAnim + timeForChargeUp, timeToUse, true);

            //Shoot Proj
            if (timer == timeForFadeInAnim + timeForChargeUp + timeAtFullCharge)
            {
                recoilProg = 1f;
                player.GetModPlayer<AeroPlayer>().ScreenShakePower = 14;

                Vector2 shotVel = Projectile.rotation.ToRotationVector2() * 12;
                int a = Projectile.NewProjectile(null, player.Center, shotVel, ModContent.ProjectileType<CavernSwordElecShot>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
            }

            #endregion

            recoilProg = Math.Clamp(recoilProg - 0.08f, 0f, 1f);

            //Held Proj Code
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            if (timer == 110)
                Projectile.active = false;

            Projectile.velocity = Vector2.Zero;

            Vector2 mousePos = Vector2.Zero;
            if (Projectile.owner == Main.myPlayer)
                mousePos = Main.MouseWorld;

            float rotDir = (mousePos - player.Center).ToRotation();

            //Recoil = move back for 15% of the duration, ease back in for the other 85%
            float recoilOffset = 0f;
            if (recoilProg > 0.85f)
            {
                float recoilLerp = Utils.GetLerpValue(1f, 0.85f, recoilProg, true);
                recoilOffset = MathHelper.Lerp(0f, -16f, Easings.easeOutCubic(recoilLerp)); //-10f
            }
            else
            {
                float recoilLerp = Utils.GetLerpValue(0.85f, 0f, recoilProg, true);
                recoilOffset = MathHelper.Lerp(-16f, 0f, Easings.easeInQuad(recoilLerp)); //-10f
            }

            Projectile.Center = player.MountedCenter + rotDir.ToRotationVector2() * (offsetAmount + recoilOffset);
            Projectile.rotation = rotDir;

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(mousePos.X < player.Center.X ? -1 : 1);
            //player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotDir - MathHelper.PiOver2);

            //Use this if you are not doing composite arms
            //player.itemRotation = MathHelper.WrapAngle(rotDir + (player.direction != 1 ? -3.14f : 0f));

            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.timeLeft = 2;


            float armEase = Easings.easeOutQuad(fadeInProgress);
            if (armEase > 0.75f)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotDir - MathHelper.PiOver2);
            else if (armEase > 0.5f)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.ThreeQuarters, rotDir - MathHelper.PiOver2);
            else if (armEase > 0.25f)
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, rotDir - MathHelper.PiOver2);
            else
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.None, rotDir - MathHelper.PiOver2);


            //Vector2 lightPos = Projectile.Center + (rotDir.ToRotationVector2() * offsetAmount);
            //Lighting.AddLight(lightPos, Color.SkyBlue.ToVector3() * overallAlpha * 0.4f);

            timer++;
        }

        float chargeVal = 0f;

        float overallAlpha = 1f;
        float overallScale = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/CrystalCaverns/CavernSword/CavernSword").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, player.gfxOffY);
            drawPos += Main.rand.NextVector2Circular(3f, 3f) * Easings.easeInQuad(chargeVal);

            //Main Texture + Glowmask
            Vector2 origin = texture.Size() / 2f;
            SpriteEffects SE = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float extraRot = player.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver4 * -1; //Sprite is diagonal, so make it straight

            Color between = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.5f);
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(texture, drawPos + Main.rand.NextVector2CircularEdge(2f, 2f) * chargeVal, null, between with { A = 0 } * chargeVal * 1f, Projectile.rotation + extraRot, origin, Projectile.scale * overallScale, SE, 0.0f);
            }

            Main.spriteBatch.Draw(texture, drawPos, null, lightColor * overallAlpha, Projectile.rotation + extraRot, origin, Projectile.scale * overallScale, SE, 0.0f);

            Main.spriteBatch.Draw(texture, drawPos, null, between with { A = 0 } * chargeVal * 1f, Projectile.rotation + extraRot, origin, Projectile.scale * overallScale, SE, 0.0f);

            return false;
        }

    }

    public class CavernSwordElecChain : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 22900;
        }

        public override bool? CanCutTiles() => false;
        public override bool? CanDamage() => false;

        public Vector2 startPoint;
        public Vector2 endPoint;
        public float direction;

        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
            {
                direction = (endPoint - Projectile.Center).ToRotation();

                Dust p2 = Dust.NewDustPerfect(endPoint, ModContent.DustType<SoftGlowDust>(), Vector2.Zero, newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.5f, 0.65f) * 0.3f);
                p2.customData = DustBehaviorUtil.AssignBehavior_SGDBase(overallAlpha: 0.02f);

                Dust p3 = Dust.NewDustPerfect(endPoint, ModContent.DustType<SoftGlowDust>(), Vector2.Zero, newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.5f, 0.65f) * 0.15f);
                p3.customData = DustBehaviorUtil.AssignBehavior_SGDBase(overallAlpha: 0.03f);

                //Sound
                int soundVar = Main.rand.Next(0, 3);
                SoundStyle style5 = new SoundStyle("Terraria/Sounds/Custom/dd2_lightning_bug_zap_" + soundVar) with { Volume = 0.35f, Pitch = 0.51f, PitchVariance = 0.15f, MaxInstances = -1 };
                SoundEngine.PlaySound(style5, endPoint);

                SoundStyle stylea = new SoundStyle("AerovelenceMod/Sounds/Effects/lightning_flash_01") with { Volume = 0.08f, Pitch = 1f, PitchVariance = 0.2f, MaxInstances = -1 };
                SoundEngine.PlaySound(stylea, endPoint);
            }

            Projectile.spriteDirection = direction.ToRotationVector2().X > 0 ? 1 : -1;

            if (timer == 0)
            {
                float length = (endPoint - startPoint).Length();
                int relativeMidpoints = 1 + (int)(2 * (length / 400f));

                if (length < 100)
                    relativeMidpoints = 0;

                int numberOfMidpoints = relativeMidpoints;// 3 + (Main.rand.NextBool() ? 1 : 0); //Make relative to distance between start and end later

                //Add the start point
                trailPositions.Add(startPoint);

                //Add the midpoints
                float distance = startPoint.Distance(endPoint);
                for (int i = 1; i <= numberOfMidpoints; i++)
                {
                    float distanceBetweenMidpoints = distance * (1f / (numberOfMidpoints + 1f));

                    Vector2 newMidPointBasePosition = startPoint + direction.ToRotationVector2() * (distanceBetweenMidpoints * i);

                    //Offset the position vertically by a random amount (rotated by direction)
                    float verticalOffset = Main.rand.NextFloat(-25f, 25f);
                    float horizontalOffset = Main.rand.NextFloat(-15f, 15f);

                    newMidPointBasePosition += new Vector2(horizontalOffset, verticalOffset).RotatedBy(direction);

                    trailPositions.Add(newMidPointBasePosition);
                }

                //Add the end point
                trailPositions.Add(endPoint);

                //Calculate point rotations
                for (int i = 0; i < trailPositions.Count - 1; i++)
                {
                    Vector2 thisPoint = trailPositions[i];
                    Vector2 nextPoint = trailPositions[i + 1];

                    float rot = (nextPoint - thisPoint).ToRotation();
                    trailRotations.Add(rot);
                }

                //Add final rotation
                trailRotations.Add(trailRotations[trailRotations.Count - 1]);
                originalPoints = trailPositions;

                //Do dust
                //Dust from orb
                for (int i = 0; i < 3 + Main.rand.Next(0, 3); i++) //4 //2,2
                {
                    Vector2 vel = Main.rand.NextVector2Circular(5f, 5f) * 1f;
                    vel += trailRotations[0].ToRotationVector2() * 9f;

                    Dust p = Dust.NewDustPerfect(startPoint, ModContent.DustType<PixelatedLineSpark>(), vel, newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.9f, 1.15f) * 0.4f);

                    p.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.9f, preShrinkPower: 0.99f, postShrinkPower: 0.89f, timeToStartShrink: 8 + Main.rand.Next(-5, 5), killEarlyTime: 40,
                        0.75f, 0.5f, shouldFadeColor: false);
                }

                //End dust
                for (int i = 0; i < 6 + Main.rand.Next(0, 4); i++) //4 //2,2
                {
                    Vector2 vel = Main.rand.NextVector2Circular(7f, 7f) * 1f;

                    Dust p = Dust.NewDustPerfect(endPoint, ModContent.DustType<PixelatedLineSpark>(), vel,
                        newColor: Color.DeepSkyBlue, Scale: Main.rand.NextFloat(0.9f, 1.15f) * 0.4f);

                    p.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.89f, preShrinkPower: 0.99f, postShrinkPower: 0.89f, timeToStartShrink: 4 + Main.rand.Next(-5, 5), killEarlyTime: 40,
                        0.75f, 0.5f, shouldFadeColor: false);

                }
            }

            if (timer % 1 == 0 && timer != 0)
            {
                for (int i = 1; i < trailRotations.Count - 1; i++)
                {
                    trailPositions[i] = originalPoints[i] + Main.rand.NextVector2Circular(10f, 10f);

                }
            }

            if (timer < 9)
                Lighting.AddLight(endPoint, Color.DeepSkyBlue.ToVector3() * 0.9f);

            if (timer == 15) //10
                Projectile.active = false;

            timer++;
        }

        public List<float> trailRotations = new List<float>();
        public List<Vector2> trailPositions = new List<Vector2>();

        public List<Vector2> originalPoints = new List<Vector2>();


        public override bool PreDraw(ref Color lightColor)
        {
            if (trailPositions.Count == 0)
                return false;

            Texture2D flare = Mod.Assets.Request<Texture2D>("Assets/Pixel/CrispStarPMA").Value;

            Vector2 startPos = trailPositions[0] - Main.screenPosition;
            Vector2 endPos = trailPositions[trailPositions.Count - 1] - Main.screenPosition;
            float startRot = trailRotations[0];
            float endRot = trailRotations[trailPositions.Count - 1];


            float vfxTime = (float)Main.timeForVisualEffects;
            float elboost = Easings.easeInOutBack(Utils.GetLerpValue(0, 7, timer, true), 0f, 5f) * Utils.GetLerpValue(15, 10, timer, true); //8
            elboost = Math.Clamp(elboost, 0.2f, 10f);

            Vector2 vec2Scale = new Vector2(1f, 0.85f) * 1.5f * elboost;


            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                Color betweenBlue = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.7f);

                Main.EntitySpriteDraw(flare, startPos, null, betweenBlue with { A = 0 }, startRot, flare.Size() / 2f, vec2Scale * 0.5f, SpriteEffects.None);
                Main.EntitySpriteDraw(flare, startPos, null, Color.White with { A = 0 }, startRot, flare.Size() / 2f, vec2Scale * 0.25f, SpriteEffects.None);

                Main.EntitySpriteDraw(flare, endPos, null, betweenBlue with { A = 0 }, endRot + (vfxTime * 0.2f * Projectile.spriteDirection), flare.Size() / 2f, vec2Scale * 0.5f, SpriteEffects.None);
                Main.EntitySpriteDraw(flare, endPos, null, Color.White with { A = 0 }, endRot, flare.Size() / 2f, vec2Scale * 0.25f, SpriteEffects.None);

                DrawTrail();

            });

            return false;
        }

        Effect myEffect = null;
        public void DrawTrail()
        {
            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;

            Color betweenBlue = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.75f);

            #region shaderPrep
            Texture2D trailTexture = Mod.Assets.Request<Texture2D>("Assets/Trails/RealLightning3").Value; //|spark_06 | Extra_196_Black
            Texture2D trailTexture2 = Mod.Assets.Request<Texture2D>("Assets/Trails/ThinGlowLine").Value;

            Vector2[] pos_arr = trailPositions.ToArray();
            float[] rot_arr = trailRotations.ToArray();

            float sineWidthMult = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.3f) * 0.15f;
            float elboost = Easings.easeInOutBack(Utils.GetLerpValue(0, 7, timer, true), 0f, 8f) * Utils.GetLerpValue(15, 10, timer, true);
            elboost = Math.Clamp(elboost, 0.2f, 10f);

            Color StripColor(float progress) => Color.White * 1f;
            float StripWidth(float progress) => 35f * 1f * sineWidthMult * 0.5f * elboost;
            float StripWidth2(float progress) => 100f * 1f * sineWidthMult * 0.5f * elboost;

            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStrip2 = new VertexStrip();
            vertexStrip2.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth2, -Main.screenPosition, includeBacksides: true);
            #endregion

            #region Shader

            float dist = (endPoint - startPoint).Length();
            float repValue = dist / 400f;
            myEffect.Parameters["reps"].SetValue(repValue * 0.8f);

            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            myEffect.Parameters["progress"].SetValue((float)Main.timeForVisualEffects * -0.02f);

            //UnderLayer
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture2);
            myEffect.Parameters["glowThreshold"].SetValue(1f);
            myEffect.Parameters["glowIntensity"].SetValue(1f);
            myEffect.Parameters["ColorOne"].SetValue(betweenBlue.ToVector3() * 1f);
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip2.DrawTrail();

            //Over layer
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            myEffect.Parameters["ColorOne"].SetValue(Color.SkyBlue.ToVector3() * 2f);
            myEffect.Parameters["glowThreshold"].SetValue(0.8f);
            myEffect.Parameters["glowIntensity"].SetValue(1.27f);
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();

            #endregion
        }
    }


    public class CavernSwordElecShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 1500;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.hostile = false;
            Projectile.friendly = false;

            Projectile.tileCollide = false;
            Projectile.timeLeft = 250; //180
            Projectile.extraUpdates = 1;
        }



        int timer = 0;
        public float overallAlpha = 1f;
        public float overallScale = 1f;

        public override void AI()
        {
            if (timer == 0)
            {
                Projectile.ai[0] = 1f;
                Projectile.velocity *= 1.5f;
            }

            int trailCount = 60;  //35
            Vector2 trailPos = Projectile.Center;
            previousRotations.Add(Projectile.velocity.ToRotation()); //
            previousPositions.Add(trailPos);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPositions.Count > trailCount)
                previousPositions.RemoveAt(0);

            previousRotations.Add(Projectile.velocity.ToRotation());
            previousPositions.Add(trailPos + Projectile.velocity * 0.5f);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (previousPositions.Count > trailCount)
                previousPositions.RemoveAt(0);

            //Projectile.velocity = Projectile.velocity.RotatedBy(0.045f * Projectile.ai[0]); //0.045

            float rotVal = MathF.Cos((float)timer * 0.3f) * 0.015f;
            //Projectile.velocity = Projectile.velocity.RotatedBy(rotVal); //0.045

            //Projectile.velocity = Projectile.velocity.RotatedBy(0.04f * Projectile.ai[0]); //0.045

            if (timer % 8 == 0)
                Projectile.ai[0] *= -1f;


            float timeForPopInAnim = 33; //33
            float animProgress = Math.Clamp((timer + 6) / timeForPopInAnim, 0f, 1f);

            overallScale = MathHelper.Lerp(0f, 1f, Easings.easeInOutBack(animProgress, 0f, 1.75f)) * 1f;

            //previousRotations.Add(Projectile.velocity.ToRotation()); //
            //previousPositions.Add(trailPos + Projectile.velocity);

            //if (previousRotations.Count > trailCount)
            //    previousRotations.RemoveAt(0);

            //if (previousPositions.Count > trailCount)
            //    previousPositions.RemoveAt(0);

            timer++;
        }

        public List<float> previousRotations = new List<float>();
        public List<Vector2> previousPositions = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {
            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.OverPlayers, () =>
            {
                DrawVertexTrail(false);
            });
            DrawVertexTrail(true);

            return false;
        }

        Effect myEffect = null;
        public void DrawVertexTrail(bool giveUp)
        {
            if (giveUp)
                return;

            Texture2D trailTexture = Mod.Assets.Request<Texture2D>("Assets/Trails/ThinGlowLine").Value; //
            Texture2D trailTexture2 = Mod.Assets.Request<Texture2D>("Assets/Trails/RealLightningBloom").Value; //

            if (myEffect == null)
                myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;


            //Convert lists to arrays for use in vertex strip
            Vector2[] pos_arr = previousPositions.ToArray();
            float[] rot_arr = previousRotations.ToArray();


            float sineWidthMult = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.3f) * 0.1f;


            Color StripColor(float progress) => Color.White * Easings.easeInSine(progress) * overallAlpha;

            float StripWidth(float progress)
            {
                float toReturn = 0f;
                if (progress < 0.65f) //back half
                {
                    float LV = Utils.GetLerpValue(0f, 0.65f, progress, true);
                    toReturn = Easings.easeInSine(LV);
                }
                else //Front half
                {
                    float LV = Utils.GetLerpValue(0.65f, 1f, progress, true);
                    toReturn = Easings.easeOutSine(1f - LV);
                }

                return toReturn * sineWidthMult * overallScale * 120f; //50
            }

            float StripWidth2(float progress)
            {
                float toReturn = 0f;
                if (progress < 0.65f) //back half
                {
                    float LV = Utils.GetLerpValue(0f, 0.65f, progress, true);
                    toReturn = Easings.easeInSine(LV);
                }
                else //Front half
                {
                    float LV = Utils.GetLerpValue(0.65f, 1f, progress, true);
                    toReturn = Easings.easeOutSine(1f - LV);
                }

                return toReturn * overallScale * 40; //50
            }


            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStrip2 = new VertexStrip();
            vertexStrip2.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth2, -Main.screenPosition, includeBacksides: true);


            myEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            myEffect.Parameters["progress"].SetValue((float)Main.timeForVisualEffects * 0.02f); //0.02
            myEffect.Parameters["reps"].SetValue(1f);


            //Over layer
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            myEffect.Parameters["ColorOne"].SetValue(Color.SkyBlue.ToVector3() * 2f);
            myEffect.Parameters["glowThreshold"].SetValue(1f);
            myEffect.Parameters["glowIntensity"].SetValue(1.2f);
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip.DrawTrail();

            Color between = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.5f);
            //UnderLayer
            myEffect.Parameters["TrailTexture"].SetValue(trailTexture2);
            myEffect.Parameters["glowThreshold"].SetValue(1f);
            myEffect.Parameters["glowIntensity"].SetValue(1f);
            myEffect.Parameters["ColorOne"].SetValue(between.ToVector3() * 4.5f); //Hotpink4.5
            myEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip2.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

    }


    public class CavernSwordProjAlt : BaseSwingSwordProj
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
            Projectile.extraUpdates = 10;
        }

        public override bool? CanDamage()
        {
            bool shouldDamage = (getProgress(easingProgress) >= 0.2f && getProgress(easingProgress) <= 0.75f) && justHitTime <= -1;
            return shouldDamage;
        }

        bool playedSound = false;
        public override void AI()
        {
            SwingHalfAngle = 155; //125
            easingAdditionAmount = 0.04f / Projectile.extraUpdates; //0.035
            frameToStartSwing = 0 * Projectile.extraUpdates; //3 | 0 with expo ease
            timeAfterEnd = 2 * Projectile.extraUpdates; //2
            startingProgress = 0.05f; //0.02

            offset = 55;

            StandardHeldProjCode();
            StandardSwingUpdate();

            Player player = Main.player[Projectile.owner];


            //Trail info
            if (true || justHitTime <= 0)//(justHitTime <= 0 && getProgress(easingProgress) > 0.1f)
            {
                Vector2 gfxOffset = new Vector2(0, Main.player[Projectile.owner].gfxOffY);

                if (justHitTime <= 0)
                {
                    int trailCount = 30;
                    relativeRots.Add(currentAngle + MathHelper.PiOver2); //
                    relativePoss.Add((Projectile.Center + currentAngle.ToRotationVector2() * 10f) - player.Center - gfxOffset); //90

                    if (relativeRots.Count > trailCount)
                        relativeRots.RemoveAt(0);

                    if (relativePoss.Count > trailCount)
                        relativePoss.RemoveAt(0);
                }


                trailPoss.Clear();

                foreach (Vector2 pos in relativePoss)
                {
                    trailPoss.Add(pos + player.Center + gfxOffset);
                }

            }




            //Sound
            if (getProgress(easingProgress) >= 0.3f && !playedSound)
            {


                playedSound = true;
            }


            //Dust
            int dustMod = 5;
            if (timer % dustMod == 0 && (getProgress(easingProgress) >= 0.1f && getProgress(easingProgress) <= 0.9f) && justHitTime <= 0)
            {
                Dust d = Dust.NewDustPerfect(Main.player[Projectile.owner].Center + currentAngle.ToRotationVector2() * Main.rand.NextFloat(25f, 85f), ModContent.DustType<GlowPixelCross>(),
                    currentAngle.ToRotationVector2().RotatedByRandom(0.3f).RotatedBy(MathHelper.PiOver2 * (Projectile.ai[0] > 0 ? 1 : -1)) * -Main.rand.NextFloat(2f, 5f),
                    0, newColor: Color.DeepSkyBlue, Main.rand.NextFloat(0.25f, 0.55f));

                d.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.3f, postSlowPower: 0.9f, velToBeginShrink: 2.5f, fadePower: 0.85f, shouldFadeColor: false);
            }

            justHitTime--;
        }

        public List<float> previousRotations = new List<float>();

        float overallAlpha = 1f;
        float overallScale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Texture = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/CrystalCavers/CavernSword/CavernSword").Value;

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

            float scaleBoost = (float)Math.Sin(getProgress(easingProgress) * Math.PI) * 0.3f;



            for (int i = 0; i < 5; i++)
            {
                Vector2 randPos = Main.rand.NextVector2Circular(5f, 5f);
                Main.spriteBatch.Draw(Texture, drawPos + randPos, null, Color.SkyBlue with { A = 0 } * progBoost * 0.65f, Projectile.rotation + rotationOffset, origin, Projectile.scale + scaleBoost, effects, 0f);
            }

            Main.spriteBatch.Draw(Texture, drawPos, null, lightColor, Projectile.rotation + rotationOffset, origin, Projectile.scale + scaleBoost, effects, 0f);

            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                TrailDraw(false);
            });
            TrailDraw(true);

            return false;
        }

        List<float> relativeRots = new List<float>();
        List<Vector2> relativePoss = new List<Vector2>();

        List<Vector2> trailPoss = new List<Vector2>();

        float trailWidth = 1f;
        float trailAlpha = 1f;
        Effect trailEffect = null;
        public void TrailDraw(bool giveUp)
        {
            if (giveUp)
                return;

            Texture2D trailTexture = Mod.Assets.Request<Texture2D>("Assets/Trails/ThinGlowLine").Value; //
            Texture2D trailTexture2 = Mod.Assets.Request<Texture2D>("Assets/Trails/Extra_196_Black").Value; //

            if (trailEffect == null)
                trailEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/TrailShaders/TendrilShader", AssetRequestMode.ImmediateLoad).Value;


            //Convert lists to arrays for use in vertex strip
            Vector2[] pos_arr = trailPoss.ToArray();
            float[] rot_arr = relativeRots.ToArray();


            float sineWidthMult = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.3f) * 0f;


            Color StripColor(float progress) => Color.White * Easings.easeInSine(progress);

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

                return toReturn * sineWidthMult * trailWidth * 45f; //50
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

                return toReturn * sineWidthMult * trailWidth * 60f; //50 | 130
            }


            VertexStrip vertexStrip = new VertexStrip();
            vertexStrip.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);

            VertexStrip vertexStrip2 = new VertexStrip();
            vertexStrip2.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth2, -Main.screenPosition, includeBacksides: true);


            trailEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);
            trailEffect.Parameters["progress"].SetValue((float)Main.timeForVisualEffects * 0.035f); //0.02
            trailEffect.Parameters["reps"].SetValue(1f);


            //Over layer
            Color betweenPink = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.15f);
            trailEffect.Parameters["TrailTexture"].SetValue(trailTexture);
            trailEffect.Parameters["ColorOne"].SetValue(betweenPink.ToVector3() * 2f); //2f
            trailEffect.Parameters["glowThreshold"].SetValue(0.85f);
            trailEffect.Parameters["glowIntensity"].SetValue(1.2f);
            trailEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip.DrawTrail();

            //UnderLayer
            trailEffect.Parameters["TrailTexture"].SetValue(trailTexture2);
            trailEffect.Parameters["glowThreshold"].SetValue(1f);
            trailEffect.Parameters["glowIntensity"].SetValue(1f);
            trailEffect.Parameters["ColorOne"].SetValue(Color.SkyBlue.ToVector3() * 4.5f);
            trailEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip2.DrawTrail();

            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        public override void OnHitNPC(NPC target, HitInfo hit, int damageDone)
        {
            //Want less hitpause at higher attack speeds
            justHitTime = (19 - (int)((Main.player[Projectile.owner].GetTotalAttackSpeed(DamageClass.Melee) - 1) * 19f)) * Projectile.extraUpdates; //10

            Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower += 24;

            //The dust....
            int crossCount = 12;
            for (int i = 0; i < crossCount; i++)
            {
                float dir = (MathHelper.TwoPi / (float)crossCount) * i;

                Vector2 dustVel = dir.ToRotationVector2() * Main.rand.NextFloat(3.5f, 7f) * 2f;
                dustVel = dustVel.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f));

                Color middleBlue = Color.Lerp(Color.DodgerBlue, Color.DeepSkyBlue, 0.25f + Main.rand.NextFloat(-0.15f, 0.15f));

                Dust gd = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: middleBlue, Scale: Main.rand.NextFloat(0.25f, 0.55f));
                gd.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, timeBeforeSlow: 0,
                    preSlowPower: 0.94f, postSlowPower: 0.9f, velToBeginShrink: 2.5f, fadePower: 0.92f, shouldFadeColor: false);
            }
        }


        // Find the start and end of the sword and use a line collider to check for collision with enemies
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Main.player[Projectile.owner].MountedCenter + currentAngle.ToRotationVector2() * (20f * Projectile.scale);
            Vector2 end = start + currentAngle.ToRotationVector2() * (140f * Projectile.scale);

            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 30f * Projectile.scale, ref collisionPoint);
        }

        public override float getProgress(float x)
        {
            float toReturn = 0f;

            return Easings.easeInOutExpo(x);


            #region easeExpo

            //pre 0.5
            if (x <= 0.5f)
            {
                toReturn = (float)(Math.Pow(2, (10 * x) - 5)) / 2;
            }
            else if (x > 0.5)
            {
                toReturn = (float)(2 - Math.Pow(2, (-10 * x) + 5)) / 2;
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

    public class HoverTestThing : ModProjectile
    {
        int timer = 0;
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;

            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override bool? CanDamage() => false;

        public override bool? CanCutTiles() => false;

        Vector2 storedVel = Vector2.Zero;
        float storedHorizontalVel = 0f; 
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (timer == 0)
            {
                storedVel = player.velocity;
                storedHorizontalVel = player.velocity.X > 0 ? 25 : -25;
            }    

            //Held Proj Code
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            //if (!player.channel)
           //     Projectile.active = false;

            Projectile.velocity = Vector2.Zero;

            Vector2 mousePos = Vector2.Zero;
            if (Projectile.owner == Main.myPlayer)
                mousePos = Main.MouseWorld;

            float rotDir = (mousePos - player.Center).ToRotation();

            Projectile.Center = player.MountedCenter;
            Projectile.rotation = rotDir;

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(mousePos.X < player.Center.X ? -1 : 1);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotDir - MathHelper.PiOver2);

            //Use this if you are not doing composite arms
            //player.itemRotation = MathHelper.WrapAngle(rotDir + (player.direction != 1 ? -3.14f : 0f));

            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.timeLeft = 2;


            player.velocity.X = storedHorizontalVel;
            player.velocity.Y = -0.1f;

            storedHorizontalVel *= 0.96f;

            timer++;
        }

        float chargeVal = 0f;

        float overallAlpha = 1f;
        float overallScale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Utils.DrawBorderString(Main.spriteBatch, "" + storedHorizontalVel, player.Center + new Vector2(0f, -100f) - Main.screenPosition, Color.White);

            Texture2D texture = Mod.Assets.Request<Texture2D>("Content/VFXTest/Aero/Sword/CavernSword").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, player.gfxOffY);
            drawPos += Main.rand.NextVector2Circular(3f, 3f) * Easings.easeInQuad(chargeVal);

            //Main Texture + Glowmask
            Vector2 origin = texture.Size() / 2f;
            SpriteEffects SE = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            float extraRot = player.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver4 * -1; //Sprite is diagonal, so make it straight

            Color between = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.5f);
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(texture, drawPos + Main.rand.NextVector2CircularEdge(2f, 2f) * chargeVal, null, between with { A = 0 } * chargeVal * 1f, Projectile.rotation + extraRot, origin, Projectile.scale * overallScale, SE, 0.0f);
            }

            Main.spriteBatch.Draw(texture, drawPos, null, lightColor * overallAlpha, Projectile.rotation + extraRot, origin, Projectile.scale * overallScale, SE, 0.0f);

            Main.spriteBatch.Draw(texture, drawPos, null, between with { A = 0 } * chargeVal * 1f, Projectile.rotation + extraRot, origin, Projectile.scale * overallScale, SE, 0.0f);

            return false;
        }

    }


}
