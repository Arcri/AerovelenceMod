using AerovelenceMod.Common;
using AerovelenceMod.Common.Globals.SkillStrikes;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Projectiles.Other;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.Misc.Ranged.Guns.AdamantitePulsar
{   
    //Big Laser
    public class AdamantitePulseShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;
        public override void SetStaticDefaults()
        {
            //Draw even when offscreen
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 1000;
        }

        public const int MAX_PENETRATION = 5;
        public int enemiesHit = 0;
        public bool big = false;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.scale = 1f;
            Projectile.timeLeft = 110;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 3;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            if (timer < 60)
                Projectile.velocity *= 1.03f;

            int modVal = big ? 4 : 6;

            if (timer > 2 && timer % modVal == 0 && Main.rand.NextBool(2))
            {
                float scaleBonus = big ? 0.1f : 0f;
                int flare = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowFlare>(), newColor: new Color(255, 0, 0), Scale: 0.55f + Main.rand.NextFloat(0.2f) + scaleBonus);
                Main.dust[flare].customData = new GlowFlareBehavior(0.55f, 2.5f);
                
                Main.dust[flare].velocity += Projectile.velocity.RotateRandom(0.05f);
            }

            if (timer % 5 == 0 && Main.rand.NextBool(5))
            {
                Vector2 vel = Main.rand.NextVector2Circular(3f, 3f);

                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), vel, newColor: Color.Red, Scale: Main.rand.NextFloat(0.35f, 0.5f) * 0.45f);
                d.velocity += Projectile.velocity.RotatedByRandom(0.1f) * 1f;
            }

            //Opacity
            if (timer > 4)
                Projectile.ai[0] = Math.Clamp(MathHelper.Lerp(Projectile.ai[0], big ? 1.3f : 1f, 0.04f), 0, big ? 1.1f : 0.8f); //1.1f

            Lighting.AddLight(Projectile.Center, new Color(255, 20, 20).ToVector3() * 0.8f * Projectile.ai[0]);

            int timeForScaleIn = 25 * Projectile.extraUpdates;
            float fadeInTime = Math.Clamp((timer + 3f * Projectile.extraUpdates) / timeForScaleIn, 0f, 1f);
            overallScale = Easings.easeInOutBack(fadeInTime, 0f, 1.5f);

            timer++;
        }


        float overallScale = 1f;
        float overallAlpha = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0)
                return false;

            Vector2 vscale = new Vector2(0.5f * overallScale, Projectile.velocity.Length() * 0.15f) * Projectile.ai[0];
            Vector2 vscale2 = new Vector2(0.25f * overallScale, Projectile.velocity.Length() * 0.15f) * Projectile.ai[0];
            Vector2 vscale3 = new Vector2(0.6f * overallScale, Projectile.velocity.Length() * 0.3f) * Projectile.ai[0];

            Texture2D softGlow = CommonTextures.DiamondGlowPMA.Value;
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/Flare/GlowDartBlack").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -40;

            Main.spriteBatch.Draw(softGlow, drawPos + new Vector2(0f, 0f) + Projectile.velocity, null, Color.Red with { A = 0 } * overallAlpha * 0.5f, 
                Projectile.rotation, softGlow.Size() / 2, vscale3, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex, drawPos, null, Color.Red with { A = 0 } * 1f * overallAlpha, Projectile.rotation, Tex.Size() / 2, vscale * Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex, drawPos, null, Color.White with { A = 0 } * overallAlpha * 1f, Projectile.rotation, Tex.Size() / 2, vscale2 * Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            float dustScale1 = 0.55f + Main.rand.NextFloat(0, 0.10f) + (big ? 0.1f : 0f);
            float dustScale2 = 0.45f + Main.rand.NextFloat(0, 0.10f) + (big ? 0.1f : 0f);

            int maxDust1 = 3 + (big ? 1 : 0);
            int maxDust2 = 5 + (big ? 2 : 0);

            for (int i = enemiesHit; i < maxDust1; i++)
            {
                float side = (i + 1) > maxDust1 / 2 ? 1f : -1f;

                float velVal = Main.rand.NextFloat(6, 9);
                float bonus = 0.1f * (1 - ((velVal - 6) * 0.33f));

                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(0.1f * side) + bonus) * velVal;

                Dust p = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowPixelCross>(), vel, newColor: Color.Red, Scale: dustScale1);

                float shrinkVel = Main.rand.NextFloat(1.5f, 3f);
                float postVel = Main.rand.NextFloat(0.87f, 0.91f);

                p.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.1f, preSlowPower: 0.98f, postSlowPower: postVel, velToBeginShrink: shrinkVel, fadePower: 0.9f, shouldFadeColor: false);
            }

            for (int i = enemiesHit; i < maxDust2; i++)
            {
                float side = (i + 1) > maxDust2 / 2 ? 1f : -1f;

                float velVal = Main.rand.NextFloat(3, 6);
                Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(Main.rand.NextFloat(0.6f * side) + (0.25f * side)) * velVal;

                Dust p = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowPixelCross>(), vel.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.75f, 1.1f), newColor: Color.Red, Scale: dustScale2);

                float shrinkVel = Main.rand.NextFloat(1.5f, 3f);
                float postVel = Main.rand.NextFloat(0.87f, 0.91f);

                p.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.1f, preSlowPower: 0.98f, postSlowPower: postVel, velToBeginShrink: shrinkVel, fadePower: 0.9f, shouldFadeColor: false);
            }

            for (int i = 0; i < 6; i++)
            {
                float dustScale = 0.2f + Main.rand.NextFloat(0.15f);

                ColorSparkBehavior extraInfo = new ColorSparkBehavior();
                Vector2 vel = Projectile.velocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.75f, 1.25f);

                Dust d = Dust.NewDustPerfect(target.Center, ModContent.DustType<ColorSpark>(), vel, 50 + Main.rand.Next(-2, 5), Color.Red, dustScale);
                extraInfo.gravityIntensity = 0f;
                d.fadeIn = Main.rand.NextFloat(0.5f, 1f);
                d.customData = extraInfo;
            }

            //Only skill strike first two hits
            if (enemiesHit > 1 && Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike == true)
                Projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike = false;

            Projectile.damage = (int)(Projectile.damage * 0.8f);
            enemiesHit++;
        }

        public override bool? CanDamage()
        {
            if (enemiesHit >= MAX_PENETRATION)
                return false;
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player Player = Main.player[Projectile.owner];
            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;

            float scale = ((Projectile.velocity * (timer > 60 ? 1f : 1.03f)).Length() * 0.15f);

            //Texture is 512 height so doing height/3 here
            Vector2 tip = new Vector2(170, 0f).RotatedBy(Projectile.velocity.ToRotation()) * scale;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + tip * -1,
                Projectile.Center + tip, 10, ref point);
        }
    }

    public class AdamSmallShot : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public int timer = 0;

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.scale = 0.85f; //1f
            Projectile.timeLeft = 400;
            Projectile.extraUpdates = 4;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        float overallScale = 1f;
        float overallAlpha = 1f;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (timer % 7 == 0 && Main.rand.NextBool(6))
            {
                Vector2 vel = Main.rand.NextVector2Circular(3f, 3f);

                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelAlts>(), vel, newColor: Color.Red, Scale: Main.rand.NextFloat(0.35f, 0.5f) * 0.35f);
                d.velocity += Projectile.velocity.RotatedByRandom(0.1f) * 1f;
            }

            if (timer % 5 == 0 && Main.rand.NextBool(7))
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(2f, 2f);
                dustVel += Projectile.velocity * 1.5f;

                float dustScale = Main.rand.NextFloat(0.4f, 0.5f);

                Dust d = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(), dustVel, newColor: Color.Red, Scale: dustScale);
                d.customData = DustBehaviorUtil.AssignBehavior_GPCBase(timeBeforeSlow: 0, postSlowPower: 0.92f, velToBeginShrink: 10f, fadePower: 0.92f, shouldFadeColor: false);
                d.rotation = Main.rand.NextFloat(6.28f);
            }


            //Opacity
            if (timer > 0)
                Projectile.ai[0] = Math.Clamp(MathHelper.Lerp(Projectile.ai[0], 1.2f, 0.08f), 0, 1);

            int timeForScaleIn = 25 * Projectile.extraUpdates;
            float fadeInTime = Math.Clamp((timer + 9f * Projectile.extraUpdates) / timeForScaleIn, 0f, 1f);
            overallScale = Easings.easeInOutBack(fadeInTime, 0f, 2f);

            Lighting.AddLight(Projectile.position, new Color(255, 20, 20).ToVector3() * 0.8f * Projectile.ai[0]);

            timer++;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D softGlow = CommonTextures.feather_circle128PMA.Value;
            Texture2D Tex = Mod.Assets.Request<Texture2D>("Assets/Flare/GlowDartBlack").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.UnitX) * -40;

            Main.spriteBatch.Draw(Tex, drawPos, null, Color.Red with { A = 0 } * 1f * overallAlpha, Projectile.rotation, Tex.Size() / 2, new Vector2(0.35f * overallScale, 0.3f) * Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(softGlow, drawPos + (Projectile.velocity.SafeNormalize(Vector2.UnitX) * 30f * Projectile.scale), null, Color.Red with { A = 0 } * overallAlpha * 0.5f,
                Projectile.rotation, softGlow.Size() / 2, new Vector2(0.5f * overallScale, 2f) * 0.75f * Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(Tex, drawPos, null, Color.White with { A = 0 } * overallAlpha * 0.85f, Projectile.rotation, Tex.Size() / 2, new Vector2(0.25f * overallScale, 0.25f) * Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.GetGlobalNPC<AdamShotNPC>().AdamShotHitCounter >= 3)
                SkillStrikeUtil.setSkillStrike(Projectile, 2f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            target.GetGlobalNPC<AdamShotNPC>().AdamShotHitCounter += 1;

            if (target.GetGlobalNPC<AdamShotNPC>().AdamShotHitCounter >= 4)
            {
                int a = Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<H3Impact>(), 0, 0, Main.myPlayer);
                Main.projectile[a].rotation = Projectile.rotation;
                Main.projectile[a].scale = 0.75f;
                //if (Main.projectile[a].ModProjectile is H3Impact h3)

                //Circle Dust
                CirclePulseBehavior cpb2 = new CirclePulseBehavior(0.25f, true, 1, 0.6f, 0.6f);

                Dust d1 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowDusts.CirclePulse>(), Velocity: Vector2.Zero, newColor: Color.Red * 0.75f);
                d1.customData = cpb2;
                d1.velocity = new Vector2(-0.01f, 0f);
                d1.scale = 0.15f;

                Dust d2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Dusts.GlowDusts.CirclePulse>(), Velocity: Vector2.Zero, newColor: Color.Red * 0.75f);
                d2.customData = cpb2;
                d2.velocity = new Vector2(0.01f, 0f);
                d2.scale = 0.15f;

                //Sound
                SoundStyle stylecs = new SoundStyle("Terraria/Sounds/Item_109") with { Pitch = .82f, PitchVariance = .11f, Volume = 0.7f };
                SoundEngine.PlaySound(stylecs, target.Center);

                SoundStyle BlasterDirect = new SoundStyle("AerovelenceMod/Sounds/Effects/SplatoonDirect") with { Pitch = .20f, PitchVariance = .1f, Volume = 0.3f }; 
                SoundEngine.PlaySound(BlasterDirect, target.Center);


                target.GetGlobalNPC<AdamShotNPC>().AdamShotHitCounter = 0;
            }

        }

        public override void OnKill(int timeLeft)
        {
            SoundStyle style2 = new SoundStyle("Terraria/Sounds/Item_10") with { Pitch = .3f, PitchVariance = .28f, MaxInstances = -1, Volume = 0.6f };
            SoundEngine.PlaySound(style2, Projectile.Center);

            for (int i = 0; i < 4 + Main.rand.Next(1, 3); i++)
            {
                Dust p = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowPixelCross>(),
                    Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.55f) * Main.rand.NextFloat(1f, 8f),
                    newColor: Color.Red, Scale: Main.rand.NextFloat(0.3f, 0.5f));

                float shrinkVel = Main.rand.NextFloat(1.5f, 3f);
                float postVel = Main.rand.NextFloat(0.87f, 0.91f);

                p.customData = DustBehaviorUtil.AssignBehavior_GPCBase(preSlowPower: 0.98f, postSlowPower: postVel, velToBeginShrink: shrinkVel, fadePower: 0.9f, shouldFadeColor: false);
            }

            Color between = Color.Lerp(Color.Red, Color.Crimson, 0.15f);
            Dust d11 = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, ModContent.DustType<FeatheredGlowDust>(), Velocity: Vector2.Zero, newColor: between, Scale: 0.85f);

            FeatheredGlowBehavior fgb = new FeatheredGlowBehavior(AlphaChangeSpeed: 0.9f, timeToChangeAlpha: 0, ScaleChangeSpeed: 0.85f, timeToKill: 120, OverallAlpha: 0.15f);
            fgb.DrawWhiteCore = false;
            d11.customData = fgb;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + oldVelocity, oldVelocity, Projectile.width, Projectile.height);
            Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowStrong>(), oldVelocity * 0.35f, 0, Color.Red, 0.3f);

            return true;
        }

    }

    public class AdamShotNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int AdamShotHitCounter = 0;
        public int AdamShotTimer = 0;

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.type == ModContent.ProjectileType<AdamSmallShot>())
            {
                AdamShotHitCounter++;
                AdamShotTimer = 0;
            }
        }

        public override void PostAI(NPC npc)
        {
            if (AdamShotHitCounter > 0)
            {
                if (AdamShotTimer >= 35)
                    AdamShotHitCounter = 0;
                AdamShotTimer++;
            }

            base.PostAI(npc);
        }

    }
} 