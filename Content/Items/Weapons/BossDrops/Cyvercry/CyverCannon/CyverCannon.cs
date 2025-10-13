using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Terraria.DataStructures;
using static Terraria.NPC;
using ReLogic.Content;
using Terraria.GameContent.Drawing;
using Terraria.Physics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Terraria.Graphics;
using static tModPorter.ProgressUpdate;
using Terraria.UI;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common;
using AerovelenceMod.Common.Globals.Players;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry.CyverCannon
{
    public class CyverCannon : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 46;
            Item.knockBack = KnockbackTiers.Weak;

            Item.width = 28;
            Item.height = 28;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.shootSpeed = 12f;

            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<CyverCannonProjectile>();
            Item.rare = ItemRarities.RarePrePlant;
            Item.value = Item.buyPrice(0, 5, 0, 0);

            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (Main.player[player.whoAmI].GetModPlayer<CyverCannonPlayer>().barProgress < 1f)
                    return false;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                type = ModContent.ProjectileType<CyverCannonFinaleProjectile>();

            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
            return false;
        } 
    }

    public class CyverCannonProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;

        bool burstUp = false;
        Vector2 storedPosition = Vector2.Zero;
        float storedDirection = 0f;

        //How far away gun is held from the player
        float offset = 15f;

        public int timer = 0;
        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 6;
            }

            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            Player player = Main.player[Projectile.owner];

            if (!player.channel)
                Projectile.active = false;

            Vector2 mousePos = Vector2.Zero;
            if (Projectile.owner == Main.myPlayer)
                mousePos = Main.MouseWorld;

            float rotDir = (mousePos - Projectile.Center).ToRotation();

            offset = Math.Clamp(MathHelper.Lerp(offset, 30f, 0.05f), 0f, 20f);

            Projectile.Center = player.MountedCenter + rotDir.ToRotationVector2() * offset;
            Projectile.rotation = rotDir;

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(mousePos.X < player.Center.X ? -1 : 1);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotDir - MathHelper.PiOver2);

            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.timeLeft = 2;

            Color dustCol = Color.Lerp(Color.HotPink, Color.DeepPink, 0.5f);

            if (timer == 22 || timer == 26 || timer == 30)
            {
                justShotPower = 0.6f;
                if (timer == 22)
                {
                    storedPosition = player.Center + rotDir.ToRotationVector2() * 38f;
                    storedDirection = rotDir;
                }

                float shotRot = storedDirection;
                if (timer == 22)
                    shotRot += 0.07f * (burstUp ? -1f : 1f);
                else if (timer == 30)
                    shotRot += 0.07f * (burstUp ? 1f : -1f);

                int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), storedPosition, shotRot.ToRotationVector2(), ModContent.ProjectileType<CyverCannonLaser>(), 
                    Projectile.damage, 0f, player.whoAmI);

                Main.projectile[a].scale = 0.4f; //35

                //Dust
                for (int i = 0; i < 2 + Main.rand.Next(1, 4); i++)
                {
                    Vector2 dustVel = shotRot.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(6f, 22f);
                    Dust dp = Dust.NewDustPerfect(storedPosition, ModContent.DustType<LineSpark>(), dustVel, 
                        newColor: dustCol, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.55f);

                    dp.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 80,
                        1f, 0.5f, shouldFadeColor: false);
                }

                offset -= 2.5f;

                //Sound
                SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Hit_53") with { Volume = 0.3f, Pitch = 1.5f, PitchVariance = .5f, MaxInstances = 0, };
                SoundEngine.PlaySound(style);

                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .12f, Pitch = .4f, PitchVariance = .2f, MaxInstances = 1 };
                SoundEngine.PlaySound(style2);
            }

            if (timer == 52)
            {
                justShotPower = 1.25f;
                offset -= 10;

                int a = Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center + rotDir.ToRotationVector2() * 38f, rotDir.ToRotationVector2(), ModContent.ProjectileType<CyverCannonLaser>(),
                    (int)(Projectile.damage * 1.5f), 0f, player.whoAmI);
                Main.projectile[a].scale = 0.8f; //75
                (Main.projectile[a].ModProjectile as CyverCannonLaser).bigLaser = true;

                player.velocity -= Projectile.rotation.ToRotationVector2() * 2f;

                //Dust
                for (int i = 0; i < 6 + Main.rand.Next(1, 4); i++)
                {
                    Vector2 dustVel = rotDir.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.35f, 0.35f)) * Main.rand.NextFloat(6f, 22f);
                    Dust dp = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<LineSpark>(), dustVel,
                        newColor: dustCol, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.65f);

                    dp.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 80,
                        1f, 0.5f);
                }

                Vector2 pulsePos = player.Center + new Vector2(38f, 0f).RotatedBy(rotDir) + new Vector2(0f, player.gfxOffY);

                CirclePulseBehavior cpb2 = new CirclePulseBehavior(0.10f, true, 2, 0.6f, 0.6f); //0.8
                cpb2.drawLayer = RenderLayer.Dusts;

                Dust d1 = Dust.NewDustPerfect(pulsePos, ModContent.DustType<CirclePulse>(), Velocity: Vector2.Zero, newColor: Color.HotPink * 1f);
                d1.scale = 0.08f;
                d1.customData = cpb2;
                d1.velocity = new Vector2(-0.01f, 0f).RotatedBy(rotDir);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .12f, Pitch = .8f, MaxInstances = 1 };
                SoundEngine.PlaySound(style);

                timer = -1;
                burstUp = !burstUp;
            }

            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.5f, 0.08f), 0f, 1f);

            timer++;
        }

        float justShotPower = 0f;
        public float overallScale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Texture2D MainTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/CyverCannon/CyverCannonProj").Value;
            Texture2D GlowTexFull = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/CyverCannon/CyverCannonProjWhiteGlow").Value;
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/CyverCannon/CyverCannonProj_Glow").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, player.gfxOffY);

            int frameHeight = MainTex.Height / 6;
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, MainTex.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            SpriteEffects SE = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(MainTex, drawPos, sourceRectangle, lightColor, Projectile.rotation, origin, Projectile.scale * overallScale, SE);

            Main.EntitySpriteDraw(Glowmask, drawPos, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale * overallScale, SE);
            
            Main.EntitySpriteDraw(GlowTexFull, drawPos + Main.rand.NextVector2Circular(1f, 1f), sourceRectangle, Color.White with { A = 0 } * justShotPower, Projectile.rotation, origin, Projectile.scale * overallScale, SE);
            Main.EntitySpriteDraw(GlowTexFull, drawPos + Main.rand.NextVector2Circular(1f, 1f), sourceRectangle, Color.White with { A = 0 } * justShotPower, Projectile.rotation, origin, Projectile.scale * overallScale, SE);

            return false;
        }
    }

    //TODO: convert this to a high extra-updates projectile and add pierce damage falloff
    public class CyverCannonLaser : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 3500;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;

            Projectile.width = Projectile.height = 5;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 50;

            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            //Needed for DrawBehind
            Projectile.hide = true;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }

        public bool bigLaser = false;

        bool hasHitEnemy = false;

        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
                Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.velocity = Vector2.Zero;

            if (timer > 1)
                overallScale = Math.Clamp(MathHelper.Lerp(overallScale, -0.5f, 0.11f), 0f, 1f);

            if (overallScale <= 0.1f)
                Projectile.Kill();


            Vector2 startPos = Projectile.Center;
            Vector2 endPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * 1000;

            //Dust
            if (timer < 3)
            {
                int dustCount = bigLaser ? 11 : 8;
                float scaleMult = bigLaser ? 0.8f : 0.45f;

                for (int i = 0; i < dustCount; i++)
                {
                    Vector2 yOffset = new Vector2(0f, Main.rand.NextFloat(-20f, 20f)).RotatedBy(Projectile.rotation) * Projectile.scale;
                    Vector2 dustPos = Vector2.Lerp(startPos, endPos, Main.rand.NextFloat(0f, 1f)) + yOffset;

                    Vector2 dustVel = new Vector2(Main.rand.NextFloat(5f, 19f), 0f).RotatedBy(Projectile.rotation);

                    Dust d = Dust.NewDustPerfect(dustPos, ModContent.DustType<MuraLineBasic>(), dustVel, newColor: Color.DeepPink, Scale: Main.rand.NextFloat(0.15f, 0.55f) * scaleMult);
                    d.alpha = 12;
                }

                for (int i = 220; i < 30; i++)
                {
                    Vector2 yOffset = new Vector2(0f, Main.rand.NextFloat(-25f, 25f)).RotatedBy(Projectile.rotation) * Projectile.scale;
                    Vector2 dustPos = Vector2.Lerp(startPos, endPos, Main.rand.NextFloat(0f, 1f)) + yOffset;

                    Vector2 dustVel = new Vector2(Main.rand.NextFloat(5f, 10f), 0f).RotatedBy(Projectile.rotation);

                    Dust d = Dust.NewDustPerfect(dustPos, ModContent.DustType<MuraLineBasic>(), dustVel, newColor: Color.HotPink, Scale: Main.rand.NextFromList(0.35f, 0.85f) * scaleMult);
                    d.alpha = 12;
                }
            }

            //Lighting
            DelegateMethods.v3_1 = Color.HotPink.ToVector3() * 1f * Projectile.scale * (bigLaser ? 1.35f : 1f) * overallScale;
            Utils.PlotTileLine(startPos, endPos, 10f * Projectile.scale, DelegateMethods.CastLight);

            timer++;
        }


        float overallScale = 1f;
        float overallAlpha = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0)
                return false;

            ModContent.GetInstance<NewAdditivePixelationSystem>().QueueRenderAction(RenderLayer.OverPlayers, () =>
            {
                DrawLaser(true);
            });
            DrawLaser(false);

            //Star
            Texture2D StarTex = CommonTextures.CrispStarPMA.Value;
            Vector2 starPoint = Projectile.Center - Main.screenPosition;
            Color starColor = Color.Lerp(Color.DeepPink, Color.HotPink, 0.35f);
            Main.spriteBatch.Draw(StarTex, starPoint, null, starColor with { A = 0 }, Projectile.rotation, StarTex.Size() / 2f, Projectile.scale * 2.4f * overallScale, 0, 0);
            Main.spriteBatch.Draw(StarTex, starPoint, null, Color.White with { A = 0 }, Projectile.rotation, StarTex.Size() / 2f, Projectile.scale * 1.2f * overallScale, 0, 0);


            return false;
        }

        public void DrawLaser(bool giveUp)
        {
            if (giveUp)
                return;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            //Beam
            Texture2D BeamTex = Mod.Assets.Request<Texture2D>("Assets/Trails/Clear/GlowTrailClear").Value;
            Texture2D BeamTexBlack = CommonTextures.ThinGlowLine.Value;

            Vector2 beamOrigin = new Vector2(0f, BeamTex.Height / 2);
            Vector2 beamOriginBack = new Vector2(0f, BeamTexBlack.Height / 2);

            Vector2 beamScale = new Vector2(7.5f, 0.75f * overallScale * Projectile.scale);
            Vector2 beamScaleBack = new Vector2(7.5f, 0.75f * overallScale * Projectile.scale);


            Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/CheapScroll", AssetRequestMode.ImmediateLoad).Value;
            #region Shader Params
            myEffect.Parameters["sampleTexture1"].SetValue(CommonTextures.spark_07_Black.Value);
            myEffect.Parameters["sampleTexture2"].SetValue(CommonTextures.Extra_196_Black.Value);

            Color c1 = new Color(245, 15, 120) * 14f;//Color.DeepPink;
            Color c2 = new Color(245, 15, 120);//Color.DeepPink;

            myEffect.Parameters["Color1"].SetValue(c1.ToVector4());
            myEffect.Parameters["Color2"].SetValue(c2.ToVector4());
            myEffect.Parameters["Color1Mult"].SetValue(0.85f + (bigLaser ? 0.35f : 0.1f));
            myEffect.Parameters["Color2Mult"].SetValue(0.95f);
            myEffect.Parameters["totalMult"].SetValue(0.5f);

            myEffect.Parameters["tex1reps"].SetValue(0.75f);
            myEffect.Parameters["tex2reps"].SetValue(0.75f);
            myEffect.Parameters["satPower"].SetValue(0.6f); //0.8
            myEffect.Parameters["time1Mult"].SetValue(1f);
            myEffect.Parameters["time2Mult"].SetValue(1f);
            myEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.018f);
            #endregion

            Main.spriteBatch.Draw(BeamTexBlack, drawPos, null, c2 with { A = 0 }, Projectile.rotation, beamOriginBack, beamScaleBack, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(BeamTexBlack, drawPos, null, c2 with { A = 0 }, Projectile.rotation, beamOriginBack, new Vector2(beamScaleBack.X, beamScaleBack.Y * 0.5f), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, myEffect, Main.GameViewMatrix.TransformationMatrix);
            myEffect.CurrentTechnique.Passes[0].Apply();

            Main.spriteBatch.Draw(BeamTex, drawPos, null, Color.White, Projectile.rotation, beamOrigin, beamScale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(BeamTex, drawPos, null, Color.White, Projectile.rotation, beamOrigin, beamScale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(BeamTex, drawPos, null, Color.White, Projectile.rotation, beamOrigin, beamScale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (timer != 1) return false;

            Vector2 unit = Projectile.rotation.ToRotationVector2();
            float point = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center + unit * 1000, Projectile.Center, 22, ref point);
        }


        public override void OnHitNPC(NPC target, HitInfo hit, int damageDone)
        {
            int dustCount = bigLaser ? 9 : 4;
            float scaleMult = bigLaser ? 1f : 0.75f;

            for (int i = 0; i < dustCount; i++)
            {
                Vector2 dustVel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1.75f * scaleMult, 5f * scaleMult);
                Color middlePink = Color.Lerp(Color.HotPink, Color.DeepPink, 0.75f + Main.rand.NextFloat(-0.15f, 0.15f));

                Dust gd = Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowStarSharp>(), dustVel, newColor: middlePink, Scale: Main.rand.NextFloat(0.35f, 0.5f) * scaleMult);
                gd.customData = DustBehaviorUtil.AssignBehavior_GSSBase(rotPower: 0.15f, timeBeforeSlow: 5,
                    preSlowPower: 0.94f, postSlowPower: 0.90f, velToBeginShrink: 2f, fadePower: 0.92f, shouldFadeColor: false);
            }

            Dust softGlow = Dust.NewDustPerfect(target.Center, ModContent.DustType<SoftGlowDust>(), Vector2.Zero, newColor: Color.DeepPink, Scale: 0.2f * scaleMult);

            softGlow.customData = DustBehaviorUtil.AssignBehavior_SGDBase(timeToStartFade: 3, timeToChangeScale: 0, fadeSpeed: 0.8f, sizeChangeSpeed: 0.9f, timeToKill: 10,
                overallAlpha: 0.25f, DrawWhiteCore: true, 1f, 1f);

            target.immune[Projectile.owner] = 2; //Collision only lasts for 1 frame so it doesn't matter that it is this low 

            if (bigLaser && !hasHitEnemy)
            {
                if (Main.player[Projectile.owner].GetModPlayer<CyverCannonPlayer>().barProgress < 1f)
                {
                    Main.player[Projectile.owner].GetModPlayer<CyverCannonPlayer>().barProgress += 0.15f;
                    Main.player[Projectile.owner].GetModPlayer<CyverCannonPlayer>().justShotPower = 1f;
                }
            }


            //float pierceDamageDropoff = bigLaser ? 0.9f : 0.8f;
            //Projectile.damage = (int)(Projectile.damage * pierceDamageDropoff);

            hasHitEnemy = true;
            base.OnHitNPC(target, hit, damageDone);
        }
    }

    public class CyverCannonFinaleProjectile : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 140;
            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;
        public override bool? CanCutTiles() => false;


        public Projectile laser = null;

        int timeBetweenFrames = 5;

        public int timer = 0;
        public override void AI()
        {
            if (timer != 0 && timer % 20 == 0)
                timeBetweenFrames = Math.Clamp(timeBetweenFrames - 1, 1, 5);


            Projectile.frameCounter++;
            if (Projectile.frameCounter >= timeBetweenFrames)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % 6;
            }

            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);
            Player player = Main.player[Projectile.owner];

            Projectile.rotation = Projectile.velocity.ToRotation();

            Projectile.Center = player.MountedCenter + Projectile.rotation.ToRotationVector2() * 20f;

            player.heldProj = Projectile.whoAmI;
            player.ChangeDir(Projectile.Center.X < player.Center.X ? -1 : 1);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            player.itemTime = 2;

            Projectile.timeLeft = 2;

            float lerpValue = (shootingLaser ? 0.02f : 0.13f); //015 | 13
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, player.DirectionTo(Main.MouseWorld), lerpValue);

            if (timer == 90)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CyverCannonHyperBeam>(), 
                    Projectile.damage, Projectile.knockBack, player.whoAmI);

                laser = Main.projectile[proj];
                shootingLaser = true;

                Main.player[player.whoAmI].GetModPlayer<CyverCannonPlayer>().barProgress = 0f;
                Main.player[player.whoAmI].GetModPlayer<CyverCannonPlayer>().barVisualProgress = 1f;
            }
            else if (timer > 90 && timer < 140)
            {
                float barProg = MathHelper.Lerp(1f, 0f, Utils.GetLerpValue(91, 139, timer, true));
                Main.player[player.whoAmI].GetModPlayer<CyverCannonPlayer>().barVisualProgress = barProg;

                justShotPower = 1f;
            }

            if (laser != null)
            {
                laser.rotation = Projectile.rotation;
                laser.Center = Projectile.Center;
            }


            if (timer == 150)
            {
                Projectile.active = false;
                laser.active = false;
            }

            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.5f, 0.08f), 0f, 1f);

            timer++;
        }

        bool shootingLaser = false;
        float justShotPower = 0f;
        public float overallScale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            if (timer == 0 || timer == 1)
                return false;
            
            Player player = Main.player[Projectile.owner];

            Texture2D MainTex = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/CyverCannon/CyverCannonProj").Value;
            Texture2D GlowTexFull = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/CyverCannon/CyverCannonProjWhiteGlow").Value;
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/CyverCannon/CyverCannonProj_Glow").Value;
            Texture2D PureWhie = Mod.Assets.Request<Texture2D>("Content/Items/Weapons/BossDrops/Cyvercry/CyverCannon/CyverCannonProjPureWhite").Value;

            float randomShakePower = Utils.GetLerpValue(0f, 90f, timer, true) * (shootingLaser ? justShotPower : 1f);

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, player.gfxOffY) + (Main.rand.NextVector2Circular(5f, 5f) * randomShakePower);

            int frameHeight = MainTex.Height / 6;
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, MainTex.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            SpriteEffects SE = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;


            Color underGlowColor = Color.Lerp(Color.DeepPink, Color.HotPink, randomShakePower);
            for (int i = 0; i < 5; i++)
            {
                Main.EntitySpriteDraw(PureWhie, drawPos + Main.rand.NextVector2CircularEdge(2.5f, 2.5f), sourceRectangle, underGlowColor with { A = 0 } * randomShakePower * 0.2f, 
                    Projectile.rotation, origin, Projectile.scale * overallScale, SE);
            }

            Main.EntitySpriteDraw(MainTex, drawPos, sourceRectangle, lightColor, Projectile.rotation, origin, Projectile.scale * overallScale, SE);

            Main.EntitySpriteDraw(Glowmask, drawPos, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale * overallScale, SE);

            Main.EntitySpriteDraw(GlowTexFull, drawPos + Main.rand.NextVector2Circular(1f, 1f), sourceRectangle, Color.White with { A = 0 } * justShotPower, Projectile.rotation, origin, Projectile.scale * overallScale, SE);
            Main.EntitySpriteDraw(GlowTexFull, drawPos + Main.rand.NextVector2Circular(1f, 1f), sourceRectangle, Color.White with { A = 0 } * justShotPower, Projectile.rotation, origin, Projectile.scale * overallScale, SE);


            if (timer < 90)
                return false;

            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                //Orb
                Texture2D orb = CommonTextures.flare_12.Value;

                Vector2 orbPos = drawPos + Projectile.rotation.ToRotationVector2() * 25f;

                Color col1 = Color.White * 0.75f * justShotPower;
                Color col2 = Color.DeepPink * 0.525f * justShotPower;
                Color col3 = Color.SkyBlue * 0.375f * justShotPower;

                float scale1 = 1.1f; //0.95
                float scale2 = 1.75f;
                float scale3 = 2.25f;
                float scale = 0.13f; //0.13

                float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.07f) * 0.15f;
                float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.13f) * 0.1f;

                Main.EntitySpriteDraw(orb, orbPos, null, col1 with { A = 0 }, (float)Main.timeForVisualEffects * 0.05f, orb.Size() / 2f, scale1 * scale, SpriteEffects.None);
                Main.EntitySpriteDraw(orb, orbPos, null, col2 with { A = 0 }, (float)Main.timeForVisualEffects * 0.02f, orb.Size() / 2f, scale2 * scale * sineScale1, SpriteEffects.None);
                Main.EntitySpriteDraw(orb, orbPos, null, col3 with { A = 0 }, (float)Main.timeForVisualEffects * -0.01f, orb.Size() / 2f, scale3 * scale * sineScale2, SpriteEffects.None);
            });

            return false;
        }
    }

    public class CyverCannonHyperBeam : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 7500;
        }
        public override void SetDefaults()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 22900;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (overallScale < 0.15f)
                return false;
            
            //Check collision in a radius for every 4 positions
            int i = 0;
            foreach (Vector2 vec in trailPositions)
            {
                if (i % 4 == 0 && targetHitbox.Distance(vec) < 25)
                    return true;
                i++;

            }
            return false;
        }

        int timer = 0;
        public override void AI()
        {
            if (timer == 0)
            {
                for (int i = 0; i < 25; i++)
                {
                    Color col = Main.rand.NextBool() ? Color.SkyBlue : Color.HotPink;


                    Vector2 vel = Projectile.rotation.ToRotationVector2().RotatedByRandom(1f) * Main.rand.NextFloat(3f, 20f);

                    Dust p = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<WindLine>(), vel, newColor: col, Scale: Main.rand.NextFloat(0.5f, 0.65f) * 1.15f);
                    p.customData = new WindLineBehavior(VelFadePower: 0.92f, TimeToStartShrink: 11, YScale: 0.5f);
                }

                //Sound
                SoundStyle style32 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_fire") with { Volume = 0.4f, Pitch = 0f, MaxInstances = -1, PitchVariance = 0.1f };
                SoundEngine.PlaySound(style32, Projectile.Center);

                SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorShot") with { Volume = .15f, Pitch = 0.2f, MaxInstances = -1 };
                SoundEngine.PlaySound(style, Projectile.Center);

                SoundStyle style5 = new SoundStyle("AerovelenceMod/Sounds/Effects/Misc/astrolotl") with { Volume = .18f, Pitch = .20f, MaxInstances = -1 };
                SoundEngine.PlaySound(style5, Projectile.Center);

                SoundStyle style2 = new SoundStyle("AerovelenceMod/Sounds/Effects/water_blast_projectile_spell_03") with { Volume = .35f, Pitch = .65f, MaxInstances = -1 };
                SoundEngine.PlaySound(style2, Projectile.Center);

                SoundStyle style6 = new SoundStyle("AerovelenceMod/Sounds/Effects/AnnihilatorCharge") with { Volume = .4f, Pitch = .65f, }; 
                SoundEngine.PlaySound(style6, Projectile.Center);

                FlashSystem.SetCAFlashEffect(0.4f, 30, 1f, 0.85f, true, true);

                Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = 60f;
            }
            
            ProjectileExtensions.KillHeldProjIfPlayerDeadOrStunned(Projectile);

            //Populate Points
            if (true)
            {
                trailPositions.Clear();
                trailRotations.Clear();

                int pointsToCreate = 350;
                float distance = 2000f;

                for (int i = 0; i < pointsToCreate; i++)
                {
                    float distUnit = distance / (float)pointsToCreate;

                    trailPositions.Add(Projectile.Center + new Vector2(distUnit * i, 0f).RotatedBy(Projectile.rotation));
                    trailRotations.Add(Projectile.rotation);
                }

                trailPositions.Add(Projectile.Center + new Vector2(distance, 0f).RotatedBy(Projectile.rotation));
                trailRotations.Add(0f);

            }



            if (timer > 2 && timer < 45)
            {
                for (int i = 100; i < 2000 * 0.9f; i += 125)
                {
                    Vector2 pos = Projectile.Center + new Vector2(i, 0f).RotatedBy(Projectile.rotation);
                    float rot = Projectile.rotation;

                    Color rainbow = Main.rand.NextBool() ? Color.HotPink : Color.SkyBlue;


                    if (Main.rand.NextBool(3))
                    {
                        Vector2 offset = rot.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-35, 35) * overallScale;
                        Vector2 vel = rot.ToRotationVector2().RotatedByRandom(0.05f) * Main.rand.NextFloat(2, 7);

                        if (!Main.rand.NextBool(3))
                        {
                            Dust d = Dust.NewDustPerfect(pos + offset, ModContent.DustType<GlowFlare>(), vel * 2.5f, newColor: rainbow * 1f, Scale: Main.rand.NextFloat(0.5f, 1.5f) * 0.5f);
                            d.noLight = false;
                            d.customData = new GlowFlareBehavior(0.4f, 2.5f);
                        }
                        else
                        {
                            Dust d = Dust.NewDustPerfect(pos + offset, ModContent.DustType<GlowPixelCross>(), vel * 3.5f, newColor: rainbow * 1f, Scale: Main.rand.NextFloat(0.5f, 1.4f) * 0.35f);
                            d.noLight = false;
                            d.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.17f, postSlowPower: 0.88f, velToBeginShrink: 5f, fadePower: 0.9f, shouldFadeColor: false);
                        }
                    }
                }


                float shakePower = Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower;
                Main.player[Projectile.owner].GetModPlayer<AeroPlayer>().ScreenShakePower = Math.Clamp(shakePower, 10f, 60f);


                Main.player[Projectile.owner].velocity -= Projectile.rotation.ToRotationVector2() * 0.3f;
            }

            if (timer < 45)
                overallScale = Math.Clamp(MathHelper.Lerp(overallScale, 1.5f, 0.09f), 0f, 1.15f);
            else
                overallScale = Math.Clamp(MathHelper.Lerp(overallScale, -0.5f, 0.11f), 0f, 1.15f);

            initialBoost = 0f;// Math.Clamp(MathHelper.Lerp(initialBoost, -0.5f, 0.06f), 0f, 10f);

            timer++;
        }

        float initialBoost = 1f;
        float overallScale = 0f;
        float overallAlpha = 1f;

        public List<float> trailRotations = new List<float>();
        public List<Vector2> trailPositions = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {
            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.UnderProjectiles, () =>
            {
                RainbowLaser();
            });
            return false;
        }

        Effect laserEffect = null;
        public void RainbowLaser()
        {
            Vector2 startPoint = Projectile.Center;
            Vector2 endPoint = startPoint + new Vector2(2000f, 0f);

            Vector2[] pos_arr = trailPositions.ToArray();
            float[] rot_arr = trailRotations.ToArray();


            Color StripColor(float progress) => Color.White;

            VertexStrip vertexStrip1 = new VertexStrip();
            vertexStrip1.PrepareStrip(pos_arr, rot_arr, StripColor, StripWidth, -Main.screenPosition, includeBacksides: true);


            #region Params
            if (laserEffect == null)
                laserEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/Scroll/ComboLaserVertexGradient", AssetRequestMode.ImmediateLoad).Value;

            laserEffect.Parameters["WorldViewProjection"].SetValue(Main.GameViewMatrix.NormalizedTransformationmatrix);

            laserEffect.Parameters["onTex"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Trails/Clear/GlowTrailClear").Value); //ThinLineGlowClear
            laserEffect.Parameters["gradientTex"].SetValue(ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad").Value);
            laserEffect.Parameters["baseColor"].SetValue(Color.White.ToVector3() * overallScale);
            laserEffect.Parameters["satPower"].SetValue(0.8f); //0.9f

            laserEffect.Parameters["sampleTexture1"].SetValue(CommonTextures.ThinGlowLine.Value);
            laserEffect.Parameters["sampleTexture2"].SetValue(CommonTextures.spark_06.Value);
            laserEffect.Parameters["sampleTexture3"].SetValue(CommonTextures.Extra_196_Black.Value);
            laserEffect.Parameters["sampleTexture4"].SetValue(CommonTextures.Trail5Loop.Value);

            laserEffect.Parameters["grad1Speed"].SetValue(1f); //1f
            laserEffect.Parameters["grad2Speed"].SetValue(1f); //1f
            laserEffect.Parameters["grad3Speed"].SetValue(1f); //1f
            laserEffect.Parameters["grad4Speed"].SetValue(1f); //1f

            laserEffect.Parameters["tex1Mult"].SetValue(1.5f);
            laserEffect.Parameters["tex2Mult"].SetValue(1.5f);
            laserEffect.Parameters["tex3Mult"].SetValue(1.15f);
            laserEffect.Parameters["tex4Mult"].SetValue(2.5f); //1.5
            laserEffect.Parameters["totalMult"].SetValue(1.25f);

            float dist = (endPoint - startPoint).Length();
            float repVal = dist / 2000f;
            laserEffect.Parameters["gradientReps"].SetValue(0.75f * repVal); //1f
            laserEffect.Parameters["tex1reps"].SetValue(1.15f * repVal);
            laserEffect.Parameters["tex2reps"].SetValue(1.15f * repVal);
            laserEffect.Parameters["tex3reps"].SetValue(1.15f * repVal);
            laserEffect.Parameters["tex4reps"].SetValue(1f * repVal);

            laserEffect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * -0.026f); //0.006
            #endregion

            laserEffect.CurrentTechnique.Passes["MainPS"].Apply();
            vertexStrip1.DrawTrail();
            Main.pixelShader.CurrentTechnique.Passes[0].Apply();
        }

        public float StripWidth(float progress)
        {
            float size = 1f * overallScale;
            float start = (float)Math.Cbrt(Utils.GetLerpValue(0f, 0.5f, progress, true));// Math.Clamp(1f * (float)Math.Pow(progress, 0.5f), 0f, 1f);
            float cap = (float)Math.Cbrt(Utils.GetLerpValue(1f, 0.95f, progress, true));
            return (140 * overallScale * start) + (150 * initialBoost); //150

        }

    }

    public class CyverCannonPlayer : ModPlayer
    {
        public float barProgress = 0f;

        public float justShotPower = 0f;

        public float barVisualProgress = 0f;

        public float barFadeIn = 0f;

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
            if (Player.HeldItem.type != ModContent.ItemType<CyverCannon>())
                barFadeIn = 0f;
            else
                barFadeIn = Math.Clamp(MathHelper.Lerp(barFadeIn, 1.2f, 0.12f), 0f, 1f);

            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.5f, 0.08f), 0f, 1f);
        }
    }

    public class CyverCannonBarDrawLayer : PlayerDrawLayer
    {
        public override Position GetDefaultPosition()
        {
            return new AfterParent(PlayerDrawLayers.HeldItem);
        }
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player Player = drawInfo.drawPlayer;

            if (Player.HeldItem.type != ModContent.ItemType<CyverCannon>())
                return;

            float barProgress = Player.GetModPlayer<CyverCannonPlayer>().barProgress;
            float barVisualProgress = Player.GetModPlayer<CyverCannonPlayer>().barVisualProgress;
            float justShotPower = Player.GetModPlayer<CyverCannonPlayer>().justShotPower;
            float barFadeIn = Player.GetModPlayer<CyverCannonPlayer>().barFadeIn;


            CyverBarUtils.DrawCyverBar(drawInfo, barProgress, barVisualProgress, justShotPower, barFadeIn);
        }
    }
}