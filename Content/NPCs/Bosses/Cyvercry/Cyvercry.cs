using AerovelenceMod.Common.Globals.Worlds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry;
using AerovelenceMod.Content.Projectiles;
using System;
using Terraria.GameContent.Bestiary;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry //Change me
{
    public class EnergyBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Ball");
            Main.projFrames[Projectile.type] = 8;

            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 3;
        }
        public float strength = 1f;
        int fakeTimeLeft = 540;
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.timeLeft = 540;
            Projectile.penetrate = -1;
            Projectile.damage = 120;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        TrailInfo trail1 = new TrailInfo();
        TrailInfo trail2 = new TrailInfo();
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.9f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.7f / 255f);
            Projectile.rotation = MathHelper.ToRadians(180) + Projectile.velocity.ToRotation();
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            float approaching = ((540f - Projectile.timeLeft) / 540f) * strength;
            Lighting.AddLight(Projectile.Center, 0.5f, 0.65f, 0.75f);

            Player player = Main.player[(int)Projectile.ai[0]];
            //int dust = Dust.NewDust(Projectile.Center + new Vector2(0, -4), 0, 0, DustID.Electric, 0, 0, Projectile.alpha, default, 0.5f);
            //Main.dust[dust].noGravity = true;
            //Main.dust[dust].velocity += Projectile.velocity;
            //Main.dust[dust].velocity *= 0.1f;
            //Main.dust[dust].scale *= 0.7f;
            if (player.active)
            {
                float x = Main.rand.Next(-10, 11) * 0.005f * approaching;
                float y = Main.rand.Next(-10, 11) * 0.005f * approaching;
                Vector2 toPlayer = Projectile.Center - player.Center;
                toPlayer = toPlayer.SafeNormalize(Vector2.Zero);
                Projectile.velocity += -toPlayer * (strength * (0.155f * Projectile.timeLeft / 540f)) + new Vector2(x, y);
            }

            if (Projectile.timeLeft == 380)
                Projectile.Kill();


            int trailVersion = 1;
            if (trailVersion == 1)
            {
                trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/spark_07_Black").Value;
                trail1.trailColor = new Color(78, 225, 245) * 0.75f;
                trail1.trailPointLimit = 800;
                trail1.trailWidth = 36;
                trail1.trailMaxLength = 100;
                trail1.timesToDraw = 2;
                trail1.trailTime = (float)Main.timeForVisualEffects * 0.05f;
                trail1.trailRot = Projectile.rotation;

                trail1.trailPos = Projectile.Center + Projectile.velocity;
                trail1.TrailLogic();
            }
            else if (trailVersion == 2)
            {
                trail1.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/EnergyTex").Value;
                trail1.trailColor = Color.White * 1f;
                trail1.trailPointLimit = 800;
                trail1.trailWidth = 15;
                trail1.trailMaxLength = 600;
                trail1.timesToDraw = 1;
                trail1.usePinchedWidth = true;
                trail1.trailTime = Projectile.ai[2] * 0.021f;
                trail1.trailRot = Projectile.velocity.ToRotation();
                trail1.trailPos = Projectile.Center;
                trail1.TrailLogic();

                //Trail2 Info Dump
                trail2.trailTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Extra_196_Black").Value;
                trail2.trailColor = Color.Wheat;
                trail2.trailPointLimit = 800;
                trail2.trailWidth = 45;
                trail2.trailMaxLength = 600;
                trail2.timesToDraw = 2;
                trail2.usePinchedWidth = true;

                trail2.gradient = true;
                trail2.gradientTexture = ModContent.Request<Texture2D>("AerovelenceMod/Assets/Gradients/CyverGrad2").Value;
                trail2.shouldScrollColor = true;
                trail2.gradientTime = Projectile.ai[2] * 0.03f;

                trail2.trailTime = Projectile.ai[2] * 0.04f;
                trail2.trailRot = Projectile.velocity.ToRotation();
                trail2.trailPos = Projectile.Center;
                trail2.TrailLogic();
            }

            Projectile.ai[2]++;
            fakeTimeLeft--;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item94 with { Pitch = 0.4f, Volume = 0.35f, PitchVariance = 0.2f }, Projectile.Center);

            int explo = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CyverRoarPulse>(), 0, 0, Main.myPlayer);

            if (Main.projectile[explo].ModProjectile is CyverRoarPulse crp)
            {
                crp.pixel = true;
                crp.forRoar = false;
            }
            
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[2] == 0)
                return false;

            //trail1.TrailDrawing(Main.spriteBatch);
            //trail2.TrailDrawing(Main.spriteBatch);
            //return false;
            
            trail1.TrailDrawing(Main.spriteBatch);
            trail1.trailColor = Color.White;
            trail1.trailWidth = 11;

            trail1.TrailDrawing(Main.spriteBatch);
            trail1.trailColor = new Color(78, 225, 245) * 0.75f;
            trail1.trailWidth = 40;

            Color pinkToUse = new Color(230, 23, 140);

            Texture2D newTex = Mod.Assets.Request<Texture2D>("Assets/Orbs/feather_circle128PMA").Value;
            Texture2D BallTexture = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/EnergyBall").Value;
            Texture2D BallTextureWhite = Mod.Assets.Request<Texture2D>("Content/NPCs/Bosses/Cyvercry/Textures/EnergyBallWhite").Value;


            int frameHeight = BallTexture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, BallTexture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Vector2 bonus = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 0f;
            Vector2 vec2Scale = new Vector2(1f, 0.75f) * Projectile.scale;

            for (int k = 0; k < 0; k++)
            {
                float progress = k / (float)Projectile.oldPos.Length;
                Vector2 scale = new Vector2(1f, 0.85f - (progress * 0.85f));// * (Projectile.scale + (progress * 0.25f));

                float alpha = ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + origin - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10;
                Color color = Color.Lerp(pinkToUse, Color.SkyBlue, Easings.easeInQuint(progress)) with { A = 0 } * alpha;
                Main.spriteBatch.Draw(BallTexture, drawPos, sourceRectangle, color * 0.4f, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            }

            for (int am = 0; am < 4; am++)
            {
                Main.spriteBatch.Draw(BallTextureWhite, Main.rand.NextVector2Circular(2.5f, 2.5f) + Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, sourceRectangle, new Color(230, 40, 140) with { A = 0 } * 0.65f, Projectile.rotation, origin, new Vector2(1f, 0.85f), SpriteEffects.None, 0f);
            }
            //Main.spriteBatch.Draw(BallTextureWhite, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, sourceRectangle, Color.Pink with { A = 0 } * 0.5f, Projectile.rotation, origin, new Vector2(1f, 0.85f), SpriteEffects.None, 0f);

            //Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, sourceRectangle, Color.DeepPink with { A = 0 } * 0.7f, Projectile.rotation, origin, new Vector2(1f, 0.85f) * 0.98f, SpriteEffects.None, 0f);

            Main.spriteBatch.Draw(newTex, Projectile.Center - Main.screenPosition + bonus, null, pinkToUse with { A = 0 } * 0.17f, Projectile.rotation, newTex.Size() / 2, vec2Scale * 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(newTex, Projectile.Center - Main.screenPosition + bonus, null, pinkToUse with { A = 0 } * 0.4f, Projectile.rotation, newTex.Size() / 2, vec2Scale * 0.5f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, sourceRectangle, Color.White * 1f, Projectile.rotation, origin, new Vector2(1f, 0.85f), SpriteEffects.None, 0f);

            return false;

        }
    }
    public class LaserExplosionBall : ModProjectile
    {
        //Used in PinkClone
        public float rotationOffset = 0f;
        public int stretchLaserAccelTime = 200;
        public float stretchLaserAccelStrength = 1.01f;
        public int stretchLaserTimeLeft = 400;

        public int numberOfLasers = 12;
        public int projType = ModContent.ProjectileType<CyverLaser>();
        public float vel = 5;
        public bool burstFX = true;

        public int projTimeLeft = -1;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Ball");
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 42;
            Projectile.timeLeft = 1;
            Projectile.penetrate = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.damage = 54;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            //projectile.netImportant = true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public int CyverIndex = 0;

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.9f / 255f, (255 - Projectile.alpha) * 0.5f / 255f, (255 - Projectile.alpha) * 0.7f / 255f);
            Projectile.rotation = 0;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Projectile.type];
            }
            Projectile.velocity *= 0.9f;

            if (burstFX && Projectile.timeLeft <= 20)
            {
                scale = MathHelper.Lerp(0f, 1f, Easings.easeOutBack(Projectile.timeLeft / 20f)) * 1.03f;
                //scale -= 0.08f; //MathHelper.Lerp(0f, 1f, Easings.easeInQuint(Projectile.timeLeft / 10f));
                Projectile.scale = scale;
            }
        }
        public override void OnKill(int timeLeft)
        {
            var entitySource = Projectile.GetSource_FromAI();

            SoundEngine.PlaySound(SoundID.Item94 with { Pitch = 0.4f, Volume = 0.35f, PitchVariance = 0.2f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item91 with { Pitch = 0.4f, PitchVariance = 0.2f }, Projectile.Center);
            SoundStyle style = new SoundStyle("Terraria/Sounds/Custom/dd2_explosive_trap_explode_1") with { PitchVariance = .16f, Volume = 0.8f, Pitch = 0.7f };
            SoundEngine.PlaySound(style, Projectile.Center);

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {

                for (int i = 0; i < 360; i += 360 / numberOfLasers)
                {
                    //For Ball Dash
                    bool aimToPlayer = projType == ModContent.ProjectileType<EnergyBall>();
                    Player player = Main.player[(int)Projectile.ai[0]];
                    Vector2 toPlayer = (player.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);

                    NPC cyver = Main.npc[CyverIndex];
                    int damage = (cyver.ModNPC as Cyvercry2).GetDamage("BallDash");

                    int proj = 0;
                    if (aimToPlayer) 
                        proj = Projectile.NewProjectile(entitySource, Projectile.Center, toPlayer.RotatedBy(MathHelper.ToRadians(i) + rotationOffset) * vel, projType, damage, 0);
                    else
                        proj = Projectile.NewProjectile(entitySource, Projectile.Center, new Vector2(vel, 0).RotatedBy(MathHelper.ToRadians(i) + rotationOffset), projType, damage, 0);

                    if (Main.projectile[proj].ModProjectile is StretchLaser laser)
                    {
                        Main.projectile[proj].timeLeft = stretchLaserTimeLeft;
                        laser.accelerateTime = stretchLaserAccelTime;
                        laser.accelerateStrength = stretchLaserAccelStrength;                    
                    }

                    if (projTimeLeft > 0)
                        Main.projectile[proj].timeLeft = projTimeLeft;

                }
            }

            base.OnKill(timeLeft);
        }

        float scale = 1f;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/Orbs/feather_circle128PMA");
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.DeepPink with { A = 0 } * 0.7f, Projectile.rotation, glow.Size() / 2, Projectile.scale * 0.6f * scale, 0, 0f);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.HotPink with { A = 0 } * 0.7f, Projectile.rotation, glow.Size() / 2, Projectile.scale * 0.45f * scale, 0, 0f);

            Texture2D BallTexture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/LaserExplosionBall").Value;

            int frameHeight = BallTexture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, BallTexture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;

            Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White * 0.7f, Projectile.rotation, origin, Projectile.scale * scale, 0, 0f);
            Main.spriteBatch.Draw(BallTexture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.HotPink with { A = 0 } * 0.8f, Projectile.rotation, origin, Projectile.scale * scale, 0, 0f);

            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White with { A = 0 } * 0.35f, Projectile.rotation, glow.Size() / 2, Projectile.scale * 0.35f * scale, 0, 0f);


            return false;
        }
    }
}
