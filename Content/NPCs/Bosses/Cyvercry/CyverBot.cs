using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.NPCs.Bosses.Cyvercry
{
    public class CyverBot : ModNPC
    {
        float auraRotation = 0; //rot of the aura
        float auraIntensity = 0f; //how much to draw the aura
        Vector2 auraPosition = Vector2.Zero; //Where the aura is drawn
        public bool Leader = false;

        //Enum and State beacuse I don't want to redo the drawing code in a different file
        public int State = (int)Behavior.PrimeLaser;
        public enum Behavior
        {
            PrimeLaser = 0,
            PrimeLaserLong = 1,
            StarStrike = 2,
            ESABall = 3
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyver Bot");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 540;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 66;
            NPC.height = 40;
            NPC.boss = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.alpha = 0;
            NPC.scale = 1;
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            database.Entries.Remove(bestiaryEntry);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot) => false;
        public override bool? CanBeHitByItem(Player player, Item item) => false;

        public override bool? CanBeHitByProjectile(Projectile projectile) => false;

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/GlowmaskBot");
            Texture2D eyeStar = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/RainbowRod");

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            if (!shouldHide)
                Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + new Vector2(0,4), NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);


            Main.EntitySpriteDraw(eyeStar, NPC.Center - Main.screenPosition + (NPC.rotation.ToRotationVector2() * 22f * NPC.direction) + new Vector2(0, 4), null, Color.DeepPink with { A = 0 } * 1.75f, eyeStarRot, eyeStar.Size() / 2, justShotPower * 0.85f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(eyeStar, NPC.Center - Main.screenPosition + (NPC.rotation.ToRotationVector2() * 22f * NPC.direction) + new Vector2(0, 4), null, Color.HotPink with { A = 0 } * 1.75f, eyeStarRot, eyeStar.Size() / 2, justShotPower * 0.6f, SpriteEffects.None, 0f);


        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 5;
                if (NPC.frame.Y > 3 * frameHeight)
                {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y == 2 * frameHeight) //booster frame
                {
                    int type = DustID.Electric;
                    Vector2 from = NPC.Center + new Vector2(24, 0).RotatedBy(NPC.rotation);
                }
            }
        }

        int timer = 0;
        int advancer = 0;

        Vector2 GoalPos = Vector2.Zero;
        bool UpTrueDownFalse = false;

        Vector2 storedCenter = Vector2.Zero;

        public int ESABallTimer = 0;
        bool TrueChaseX = true;
        String mode = "";
        float quadrant = 0;

        bool shouldHide = false;
        int newProjPause = 0;
        int ballPauseTime = 0;

        public bool isExpert = false;
        public bool isMaster = false;
        public bool isMasterPhase3 = false;

        float justShotPower = 0f;
        float eyeStarRot = 0f;

        public float rotIntensity = 0.02f;
        public float angleToCover = 120f;

        Vector2 originalVec = Vector2.Zero;

        public int CyverIndex = 0;
        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }


            Player myPlayer = Main.player[NPC.target];

            if (State != (int)CyverBot.Behavior.ESABall)
            {
                Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(10 * (NPC.direction == 1 ? -1 : 1), 0).RotatedBy(NPC.rotation), DustID.Electric, Vector2.Zero);
                dust.noGravity = true;
                dust.velocity += NPC.velocity;  
                dust.velocity *= 0.1f;
                dust.scale *= 0.35f;
            }

            #region PrimeLaser
            if (State == (int)Behavior.PrimeLaser || State == (int)Behavior.PrimeLaserLong)
            {
                if (advancer == 0)
                {
                    originalVec = GoalPos;
                    auraPosition = myPlayer.Center;
                    Vector2 goalPoint = GoalPos;

                    NPC.Center = Vector2.Lerp(NPC.Center, (goalPoint + myPlayer.Center), timer / 30f);
                    NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation() + MathHelper.ToRadians(180);

                    if (timer == 20)
                    {

                        advancer++;
                        timer = -1;
                    }

                }
                else if (advancer == 1)
                {
                    NPC.rotation = (GoalPos).ToRotation();
                    NPC.Center = myPlayer.Center + GoalPos;
                    NPC.velocity = Vector2.Zero;

                    if (timer == 15)
                    {

                        advancer++;
                        storedCenter = myPlayer.Center;
                    }

                }
                else if (advancer >= 2)
                {
                    auraPosition = storedCenter;

                    float progress = (advancer - 2) / 70f;
                    float easingVal = MathF.Pow(progress, 2.2f);

                    float amountToRotatePosition = MathHelper.Lerp(0f, MathHelper.PiOver2 + 0.75f, easingVal) * (NPC.ai[1]);
                    GoalPos = originalVec.RotatedBy(amountToRotatePosition);


                    //GoalPos = GoalPos.RotatedBy(MathHelper.ToRadians(advancer * (UpTrueDownFalse ? rotIntensity : -rotIntensity))); //0.02
                    NPC.rotation = (GoalPos).ToRotation();
                    NPC.Center = storedCenter + GoalPos;
                    NPC.velocity = Vector2.Zero;
                    advancer++;

                    if (timer == 40)
                    {
                        if (Leader)
                        {
                            /*
                            int b = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0f);
                            Projectile explo = Main.projectile[b];
                            if (explo.ModProjectile is PinkExplosion pinkExplo)
                            {
                                pinkExplo.exploScale = 12.51f;
                                pinkExplo.numberOfDraws = 1;
                            }
                            */
                        }
                    }
                }

                bool isFarFromCenter = Math.Abs(myPlayer.Distance(storedCenter)) > 1250;

                if (timer % 2 == 0 && timer > 20 && timer < 250) //%7
                {
                    justShotPower = 1f;
                    eyeStarRot = Main.rand.NextFloat(6.28f);

                    if (timer % 4 == 0)
                    {
                        SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = .75f, Volume = 0.075f, MaxInstances = -1 }; //1f
                        SoundEngine.PlaySound(stylec, NPC.Center);
                        
                        //justShotPower = 1.25f;
                        //eyeStarRot = Main.rand.NextFloat(6.28f);
                    }

                    NPC cyver = Main.npc[CyverIndex];
                    int damage = (cyver.ModNPC as Cyvercry2).GetDamage("Bots");


                    Vector2 offset = (NPC.rotation + MathHelper.Pi).ToRotationVector2();    //NPC.direction == 1 ? (NPC.rotation + MathHelper.Pi).ToRotationVector2() : NPC.rotation.ToRotationVector2(); 
                    float speedMultiplier = (isFarFromCenter ? 6.5f : 6.5f); //13 : 8
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset * 20, (NPC.rotation + MathHelper.Pi).ToRotationVector2() * speedMultiplier, 
                        ModContent.ProjectileType<CyverLaser>(), damage, 1);
                    
                    
                    
                    Main.projectile[a].scale = 0.8f;
                    Main.projectile[a].timeLeft = 300;


                    if (Main.projectile[a].ModProjectile is CyverLaser laser)
                    {
                        laser.damageDelay = 40; 
                        //Never collide
                        laser.tileCollideDelay = 400;
                    }

                    //Dust
                    Vector2 dustVel = (NPC.rotation + MathHelper.Pi).ToRotationVector2();
                    Dust p = Dust.NewDustPerfect(NPC.Center + offset * 20, ModContent.DustType<GlowPixelCross>(), dustVel.SafeNormalize(Vector2.UnitX) * 12f,
                        newColor: Color.HotPink, Scale: 0.3f);

                    p.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                        rotPower: 0.05f, timeBeforeSlow: 0, postSlowPower: 0.94f, velToBeginShrink: 5f, fadePower: 0.93f, shouldFadeColor: false);


                    ///ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                    ///Vector2 vel = (NPC.rotation + MathHelper.Pi).ToRotationVector2();

                    ///Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center + offset * 20, ModContent.DustType<GlowCircleQuadStar>(), vel.SafeNormalize(Vector2.UnitX) * 12f,
                        ///Color.HotPink, 0.5f, 0.4f, 0f, dustShader);
                    ///p.noLight = false;
                    ///p.velocity *= 0.4f;

                    lineExtraPower = 1f;
                }

                if (NPC.direction == 1)
                {
                    NPC.rotation = NPC.rotation + MathHelper.Pi;
                }

                int timeBeforeDeath = State == (int)Behavior.PrimeLaserLong ? 300 : 85; //150

                if (Leader)
                {
                    
                    if (timer == 300) //380
                    {
                        SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = 1f, Volume = 1f, MaxInstances = -1 }; //1f
                        SoundEngine.PlaySound(stylec, auraPosition);
                        /*
                        for (int i = 0; i < 30; i++)
                        {
                            //int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, new Vector2(10, 0).RotatedBy(MathHelper.ToRadians(i * (360 / 15))), ModContent.ProjectileType<EnergyBall>(), 4, 2, Main.myPlayer);

                            float speed = i % 2 == 0 ? 3.5f : 5;
                            int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, new Vector2(speed,0).RotatedBy(MathHelper.ToRadians(i * (360/30))), ModContent.ProjectileType<CyverLaser>(), 10, 0, Main.myPlayer);
                        }
                        */

                        //(myPlayer.Center - auraPosition).SafeNormalize(Vector2.UnitX)
                        //int laserUp = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition + new Vector2(-0.5f,0), new Vector2(0,-1), ModContent.ProjectileType<CyverHyperBeam>(), 20, 0f);
                        //int laserDown = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition + new Vector2(0.5f,0), new Vector2(0,1), ModContent.ProjectileType<CyverHyperBeam>(), 20, 0f);
                        //int laserRight = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, new Vector2(1, 0), ModContent.ProjectileType<CyverHyperBeam>(), 20, 0f);
                        //int laserLeft = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, new Vector2(-1, 0), ModContent.ProjectileType<CyverHyperBeam>(), 20, 0f);


                        int b = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0f);
                        Projectile explo = Main.projectile[b];
                        if (explo.ModProjectile is PinkExplosion pinkExplo)
                        {
                            pinkExplo.exploScale = 2.51f;
                        }

                        for (int i = 0; i < 6; i++)
                        {
                            //int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, new Vector2(8, 0).RotatedBy(MathHelper.ToRadians(i * (360 / 6))), ModContent.ProjectileType<EnergyBall>(), 4, 2, Main.myPlayer);

                            //float speed = i % 2 == 0 ? 3.5f : 5;
                            //int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, new Vector2(speed,0).RotatedBy(MathHelper.ToRadians(i * (360/30))), ModContent.ProjectileType<CyverLaser>(), 10, 0, Main.myPlayer);
                        }

                    }

                }

                if (timer == timeBeforeDeath) //180
                {
                    if (State == (int)Behavior.PrimeLaser)
                    {
                        KillFX();
                        NPC.active = false;
                    }

                    if (Leader)
                    {
                        State = (int)CyverBot.Behavior.ESABall;
                        NPC.Center = auraPosition;
                    } else if (State == (int)Behavior.PrimeLaserLong)
                    {
                        KillFX();
                        NPC.active = false;
                    }
                    
                }
                if (timer >= 160)
                {
                    auraRotation += MathHelper.ToRadians(timer * (UpTrueDownFalse ? -0.1f : 0.1f));

                    auraIntensity = MathHelper.Clamp((timer - 160) * 0.01f, 0, 1f);
                }
            }

            #endregion;
             
            #region StarStrike

            #endregion

            #region ESABall

            if (State == (int)CyverBot.Behavior.ESABall)
            {
                NPC.setNPCName(" ", ModContent.NPCType<CyverBot>());

                shouldHide = true;
                if (ESABallTimer == 0)
                {
                    quadrant = getQuadrant(myPlayer);

                    if (TrueChaseX && (quadrant == 1 || quadrant == 4)) mode = "RightToLeft";
                    if (TrueChaseX && (quadrant == 2 || quadrant == 3)) mode = "LeftToRight";

                    if (!TrueChaseX && (quadrant == 2 || quadrant == 1)) mode = "UpToDown";
                    if (!TrueChaseX && (quadrant == 3 || quadrant == 3)) mode = "DownToUp";

                }

                if (mode.Equals("LeftToRight"))
                {
                    if (newProjPause <= 10)
                        NPC.velocity.X += 1.15f;
                    auraRotation += MathHelper.ToRadians(3);

                    if (NPC.Center.X > myPlayer.Center.X)
                        Clear();
                }
                else if (mode.Equals("RightToLeft"))
                {
                    if (newProjPause <= 10)
                        NPC.velocity.X -= 1.15f;
                    auraRotation += MathHelper.ToRadians(-3);

                    if (NPC.Center.X < myPlayer.Center.X)
                        Clear();
                }
                else if (mode.Equals("UpToDown"))
                {
                    if (newProjPause <= 10)
                        NPC.velocity.Y += 0.7f;
                    auraRotation += MathHelper.ToRadians(3);

                    if (NPC.Center.Y > myPlayer.Center.Y)
                        Clear();
                }
                else if (mode.Equals("DownToUp"))
                {
                    if (newProjPause <= 10)
                        NPC.velocity.Y -= 0.7f;
                    auraRotation += MathHelper.ToRadians(-3);

                    if (NPC.Center.Y < myPlayer.Center.Y)
                        Clear();
                }
                auraPosition = NPC.Center;
                NPC.Center = auraPosition;

                if (ballPauseTime > 0)
                    NPC.velocity = Vector2.Zero;

                ESABallTimer++;

                newProjPause--;
                ballPauseTime--;
                auraIntensity = 1f;
            }

            #endregion
            //Dust.NewDustPerfect(auraPosition, DustID.FireworkFountain_Blue);

            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.25f, 0.05f), 0f, 2f);
            lineExtraPower = Math.Clamp(MathHelper.Lerp(lineExtraPower, -0.25f, 0.1f), 0f, 1f);

            timer++;
            NPC.damage = 0;
            
        }
        public void setGoalLocation(Vector2 location)
        {
            GoalPos = location;
        }

        public void setTurn(bool input)
        {
            UpTrueDownFalse = input;
        }

        public override bool CheckActive()
        {
            return false; //!!true
        }

        float lineExtraPower = 0f;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Player myPlayer = Main.player[NPC.target];

            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/GlowmaskBot");
            Texture2D spiralTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Items/Weapons/Flares/star_05");
            Texture2D glorbTex = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/Textures/circle_05");
            Texture2D eyeStar = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Twinkle");

            Texture2D Line = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Assets/TrailImages/Medusa_Gray");
            Vector2 lineScale = new Vector2(3f, 0.2f + (lineExtraPower * 0.1f)) * 1.5f;

            float add = NPC.spriteDirection == 1 ? 0f : MathHelper.Pi;

            if (!shouldHide)
            {
                float lineOpacity = timer < 40 ? 1f : 1f - Math.Clamp(((float)timer - 40f) / 40f, 0, 1);

                //Color col1 = Color.Lerp(Color.DeepPink, Color.White, lineExtraPower) with { A = 0 } * 0.4f * lineOpacity;
                //Color col2 = Color.Lerp(Color.HotPink, Color.White, lineExtraPower) with { A = 0 } * 0.5f * lineOpacity;
                //Color col3 = Color.White with { A = 0 } * 0.6f * lineOpacity;
                Color col1 = Color.DeepPink with { A = 0 } * 0.4f * lineOpacity;
                Color col2 = Color.HotPink with { A = 0 } * 0.5f * lineOpacity;
                Color col3 = Color.White with { A = 0 } * 0.6f * lineOpacity;

                Main.spriteBatch.Draw(Line, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, col1, NPC.rotation + add, new Vector2(0, Line.Height / 2), lineScale * 1.25f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Line, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, col2, NPC.rotation + add, new Vector2(0, Line.Height / 2), lineScale * 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(Line, NPC.Center - Main.screenPosition + new Vector2(0, 4), null, col3, NPC.rotation + add, new Vector2(0, Line.Height / 2), lineScale * 0.75f, SpriteEffects.None, 0f);
            }

            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            ///Effect myEffect = ModContent.Request<Effect>("AerovelenceMod/Effects/GlowMisc", AssetRequestMode.ImmediateLoad).Value;
            ///myEffect.Parameters["uColor"].SetValue(Color.DeepPink.ToVector3() * MathHelper.Clamp((1.5f * auraIntensity), 0, 1.5f)); //2.5f makes it more spear like
            ///myEffect.Parameters["uTime"].SetValue(2);
            ///myEffect.Parameters["uOpacity"].SetValue(0.2f); //0.6
            ///myEffect.Parameters["uSaturation"].SetValue(1.2f);

            ///Main.spriteBatch.End();
            ///Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
            //myEffect.CurrentTechnique.Passes[0].Apply();


            if (Leader)
            {
                for (int i = 0; i < 4; i++)
                {
                    spriteBatch.Draw(spiralTex, auraPosition - Main.screenPosition, null, Color.HotPink * (auraIntensity) * 0.7f, auraRotation + MathHelper.PiOver2 + (i * MathHelper.PiOver4), spiralTex.Size() / 2, 1.25f, SpriteEffects.None, 0f);
                    spriteBatch.Draw(glorbTex, auraPosition - Main.screenPosition, null, Color.HotPink * (auraIntensity) * 0.7f, auraRotation + MathHelper.PiOver2 * i, spiralTex.Size() / 2, 0.5f, SpriteEffects.None, 0f);
                }
            }

            //spriteBatch.Draw(flareTex, auraPosition - Main.screenPosition, null, Color.HotPink * (auraIntensity) * 0.4f, MathHelper.PiOver2 + (float)Math.Sin(timer * 0.2f), spiralTex.Size() / 2, 2f, SpriteEffects.None, 0f);

            if (Leader && timer > 40)
            {
                //spriteBatch.Draw(barrierTex, auraPosition - Main.screenPosition, null, (Color.HotPink) * ((float)Math.Sin(timer  * 0.1f) * 0.75f), auraRotation, barrierTex.Size() / 2, 8f, SpriteEffects.None, 0f);
                //spriteBatch.Draw(barrierTex, auraPosition - Main.screenPosition, null, Color.HotPink, auraRotation, barrierTex.Size() / 2, 8f, SpriteEffects.None, 0f);
                
                //spriteBatch.Draw(barrierTex, auraPosition - Main.screenPosition, null, Color.HotPink * 0.8f, auraRotation, barrierTex.Size() / 2, 7f, SpriteEffects.None, 0f);
                //spriteBatch.Draw(barrierTex2, auraPosition - Main.screenPosition, null, Color.HotPink * 0.8f, auraRotation, barrierTex2.Size() / 2, 2.1f, SpriteEffects.None, 0f);
                //spriteBatch.Draw(barrierTex2, auraPosition - Main.screenPosition, null, Color.HotPink * 0.8f, auraRotation, barrierTex2.Size() / 2, 2.1f, SpriteEffects.None, 0f);
                //spriteBatch.Draw(barrierTex2, auraPosition - Main.screenPosition, null, Color.HotPink * 0.8f, auraRotation, barrierTex2.Size() / 2, 2.1f, SpriteEffects.None, 0f);


            }
            //spriteBatch.Draw(barrierTex, auraPosition - Main.screenPosition, null, Color.HotPink, 0f, barrierTex.Size() / 2, 3f, SpriteEffects.None, 0f);


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            if (shouldHide)
            {
                return false;
            }
            Vector2 drawOrigin = NPC.frame.Size() / 2;
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - screenPos + drawOrigin + new Vector2(0f, NPC.gfxOffY) + new Vector2(0, 4);
                //drawPos = drawPos + (drawSpikeFrame ? new Vector2(-6, -18) : new Vector2(0, 10));
                Color newDrawCol = Color.DeepPink * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);

                if (advancer >= 2)
                    spriteBatch.Draw(texture, drawPos, NPC.frame, newDrawCol with { A = 0 } * 0.5f, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
                else
                    spriteBatch.Draw(texture, drawPos, NPC.frame, newDrawCol * 0.5f, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
            }

            Vector2 backGlowPos = NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY) + new Vector2(0, 4);

            for (int a = 0; a < 4; a++)
            {
                spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, backGlowPos + Main.rand.NextVector2Circular(2.25f, 2.25f), NPC.frame, Color.DeepPink with { A = 0 } * 1.5f, NPC.rotation, drawOrigin, NPC.scale * 1.1f, effects, 0f);
            }
            return true;

        }

        #region ESAMethods
        public float getQuadrant(Player myPlayer)
        {
            bool LesserX = NPC.Center.X < myPlayer.Center.X;
            bool LesserY = NPC.Center.Y < myPlayer.Center.Y;

            if (!LesserX && LesserY) return 1;
            if (LesserX && LesserY) return 2;
            if (LesserX && !LesserY) return 3;
            if (!LesserX && !LesserY) return 4;

            return -1;
        }

        public void Clear()
        {
            if (newProjPause <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item94 with { Pitch = -0.2f, Volume = 0.5f, PitchVariance = 0.25f }, NPC.Center);
                SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = 0.4f, Volume = 0.75f, MaxInstances = -1, PitchVariance = 0.25f }; //1f
                SoundEngine.PlaySound(stylec, NPC.Center);

                int b = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.velocity.SafeNormalize(Vector2.UnitX) * 10f, Vector2.Zero, ModContent.ProjectileType<PinkExplosion>(), 0, 0f);
                Projectile explo = Main.projectile[b];
                if (explo.ModProjectile is PinkExplosion pinkExplo)
                {
                    pinkExplo.exploScale = 1f;
                    pinkExplo.numberOfDraws = 2;
                }
                newProjPause = 20;
                ballPauseTime = 10;

                for (int i = 0; i < 8; i++)
                {
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.velocity.SafeNormalize(Vector2.UnitX) * 10f, new Vector2(1.2f, 0).RotatedBy(MathHelper.ToRadians(i * 45)), ModContent.ProjectileType<StretchLaser>(), 14, 0);
                    Main.projectile[a].timeLeft = 400;
                    if (Main.projectile[a].ModProjectile is StretchLaser laser)
                    {
                        laser.accelerateTime = 150;
                        laser.accelerateStrength = 1.02f; //1.025
                    }
                }
            }
            /*
            if (TrueChaseX && newProjPause <= 0)
            {
                int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, new Vector2(10,0), ModContent.ProjectileType<EnergyBall>(), 20, 0);
                int b = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, new Vector2(-10,0), ModContent.ProjectileType<EnergyBall>(), 20, 0);

                
                newProjPause = 20;
            }
            else if (newProjPause <= 0)
            {
                int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, new Vector2(0, -10), ModContent.ProjectileType<EnergyBall>(), 20, 0);
                int b = Projectile.NewProjectile(NPC.GetSource_FromAI(), auraPosition, new Vector2(0, 10), ModContent.ProjectileType<EnergyBall>(), 20, 0);

                newProjPause = 20;
            }
            */


            TrueChaseX = !TrueChaseX;
            NPC.Center = NPC.velocity.SafeNormalize(Vector2.UnitX) * 10f + NPC.Center;
            NPC.velocity = Vector2.Zero;
            ESABallTimer = -1;
        }
        #endregion

        public void KillFX()
        {
            //SpawnGores
            int g1 = Gore.NewGore(null, NPC.Center, Main.rand.NextVector2Circular(2, 2), ModContent.GoreType<CyverBotGore1>(), Scale: 1f);
            int g2 = Gore.NewGore(null, NPC.Center, Main.rand.NextVector2Circular(2, 2), ModContent.GoreType<CyverBotGore2>(), Scale: 1f);
            int g3 = Gore.NewGore(null, NPC.Center, Main.rand.NextVector2Circular(2, 2), ModContent.GoreType<CyverBotGore3>(), Scale: 1f);


            for (double m = 0; m < 6.28; m += 1)
            {
                Dust dust = Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowPixelCross>(), new Vector2((float)Math.Sin(m) * 1.3f, (float)Math.Cos(m)) * 2.4f);
                dust.color = Color.DeepPink;
                dust.velocity *= Main.rand.NextFloat(0.4f, 1.3f);
                dust.scale = 0.4f;


                dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(
                    rotPower: 0.05f, preSlowPower: 0.99f,  timeBeforeSlow: 8, postSlowPower: 0.92f, velToBeginShrink: 1f, fadePower: 0.87f, shouldFadeColor: false);

            }

            SoundStyle style = new SoundStyle("Terraria/Sounds/NPC_Killed_44") with { Pitch = 0f, PitchVariance = 0, MaxInstances = -1, Volume = 0.1f }; 
            SoundEngine.PlaySound(style, NPC.Center);

        }
    }
    public class CyverBotOrbiter : ModNPC {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cyver Bot");
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }
        public override void SetDefaults()
        {
            NPC.lifeMax = 540;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 66;
            NPC.height = 40;
            NPC.boss = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.alpha = 0;
            NPC.scale = 1;
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = true;
        }

        public float storedRotation;
        public Vector2 GoalPoint = Vector2.Zero;
        public int timer = 0;
        public int State = (int)Behavior.StarStrikeP1;
        public int storedDirection = 0;
        public enum Behavior
        {
            StarStrikeP1 = 0
        }

        #region drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/GlowmaskBot");
            Vector2 drawOrigin = NPC.frame.Size() / 2;
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Vector2 drawPos = NPC.oldPos[k] - screenPos + drawOrigin + new Vector2(0f, NPC.gfxOffY) + new Vector2(0, 4);
                //drawPos = drawPos + (drawSpikeFrame ? new Vector2(-6, -18) : new Vector2(0, 10));
                drawColor = Color.HotPink * ((NPC.oldPos.Length - k) / (float)NPC.oldPos.Length);
                spriteBatch.Draw(texture, drawPos, NPC.frame, drawColor * 0.5f, NPC.rotation, drawOrigin, NPC.scale, effects, 0f);
            }
            return true;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 10)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 5;
                if (NPC.frame.Y > 3 * frameHeight)
                {
                    NPC.frame.Y = 0;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y == 2 * frameHeight) //booster frame
                {
                    
                }
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/NPCs/Bosses/Cyvercry/GlowmaskBot");
            SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, NPC.Center - Main.screenPosition + new Vector2(0, 4), NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }
        #endregion

        #region AI

        public override void AI()
        {
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
            {
                NPC.TargetClosest();
            }
            Player myPlayer = Main.player[NPC.target];
            StarStrike(myPlayer);

            // THE DUST     the dust...
            Dust dust = Dust.NewDustPerfect(NPC.Center + new Vector2(10 * (NPC.direction == 1 ? -1 : 1), 0).RotatedBy(NPC.rotation), DustID.Electric, Vector2.Zero);
            dust.noGravity = true;
            dust.velocity += NPC.velocity;
            dust.velocity *= 0.1f;
            dust.scale *= 0.35f;

        }

        float leaveSpeed = 0f;
        public float rotDir = 1;
        public void StarStrike(Player myPlayer)
        {
            //Move to Point
            if (timer < 80)
            {
                NPC.spriteDirection = NPC.direction;

                Vector2 move = (GoalPoint.RotatedBy(timer * (0.007f * rotDir)) + myPlayer.Center) - NPC.Center;
                float scalespeed = 4; //(timer < 20 ? 3f : 7); //5

                NPC.velocity.X = (NPC.velocity.X + move.X) / 20f * scalespeed;
                NPC.velocity.Y = (NPC.velocity.Y + move.Y) / 20f * scalespeed;

                NPC.rotation = (myPlayer.Center - NPC.Center).ToRotation();

                //Shoot
                if (timer == 55)
                {
                    SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = .74f, Volume = 0.15f, MaxInstances = 5 }; //1f
                    SoundEngine.PlaySound(stylec, NPC.Center);
                    Vector2 offset = (NPC.rotation + MathHelper.Pi).ToRotationVector2();
                    int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), myPlayer.Center + GoalPoint.RotatedBy(timer * (0.007f * rotDir)), (myPlayer.Center - (GoalPoint.RotatedBy(timer * (0.007f * rotDir)) + myPlayer.Center)).SafeNormalize(Vector2.UnitX) * 6, ModContent.ProjectileType<CyverLaser>(), 10, 0, Main.myPlayer);
                    Main.projectile[a].scale = 1f;

                    ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                    Vector2 vel = (NPC.rotation).ToRotationVector2();

                    Dust p = GlowDustHelper.DrawGlowDustPerfect(NPC.Center - offset * 20, ModContent.DustType<GlowCircleQuadStar>(), vel.SafeNormalize(Vector2.UnitX) * 12f,
                        Color.HotPink, 0.5f, 0.4f, 0f, dustShader);
                    p.noLight = false;
                    p.fadeIn = 10;
                    p.velocity *= 0.4f;
                }
                if (NPC.direction == -1)
                {
                    NPC.rotation = NPC.rotation + MathHelper.Pi;
                }
                
            }

            //Leave
            if (timer >= 80)
            {
                if (timer == 80)
                    storedDirection = NPC.direction;

                NPC.spriteDirection = storedDirection; //?

                if (timer < 100)
                    leaveSpeed = MathHelper.Lerp(leaveSpeed, 25, 0.08f);
                else
                    leaveSpeed = MathHelper.Lerp(leaveSpeed, 35, 0.08f);

                NPC.velocity = NPC.rotation.ToRotationVector2() * -leaveSpeed * storedDirection;
            }

            if (timer == 260)
            {
                NPC.active = false;
            }
            timer++;
        }

        public void LaserWall(Player myPlayer)
        {
            //move to point, fire leave
        }

        #endregion

        /*
        SoundStyle stylec = new SoundStyle("Terraria/Sounds/Item_67") with { Pitch = .75f, Volume = 0.15f, MaxInstances = -1 }; //1f
        SoundEngine.PlaySound(stylec, NPC.Center);
        Vector2 offset = (NPC.rotation + MathHelper.Pi).ToRotationVector2();    //NPC.direction == 1 ? (NPC.rotation + MathHelper.Pi).ToRotationVector2() : NPC.rotation.ToRotationVector2(); 
        float speedMultiplier = (isFarFromCenter ? 6.5f : 6.5f); //13 : 8
        int a = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset * 20, (NPC.rotation + MathHelper.Pi).ToRotationVector2() * speedMultiplier, ModContent.ProjectileType<CyverLaser>(), 10, 0, Main.myPlayer);
        Main.projectile[a].scale = 0.8f;
        */
    }

    public class CyverBotGore1 : ModGore
    {
        public override string Texture => "AerovelenceMod/Content/Gores/CyverBotGore1";

        public override bool Update(Gore gore)
        {
            if (gore.frameCounter > 15)
                gore.alpha += 10;

            if (gore.alpha >= 250)
                gore.active = false;
            gore.frameCounter++;

            return base.Update(gore);
        }
    }
    public class CyverBotGore2 : CyverBotGore1 { public override string Texture => "AerovelenceMod/Content/Gores/CyverBotGore2"; }
    public class CyverBotGore3 : CyverBotGore1 { public override string Texture => "AerovelenceMod/Content/Gores/CyverBotGore3"; }
}