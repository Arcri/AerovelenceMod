using AerovelenceMod.Common;
using AerovelenceMod.Common.Particles;
using AerovelenceMod.Common.Systems;
using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Weapons.BossDrops.Cyvercry.TrojanForce
{
    public class TrojanForceBotTest : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";

        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.ignoreWater = false;
            Projectile.hostile = false;
            Projectile.friendly = true;

            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 99370;
        }

        public override bool? CanDamage() => isDashing;

        public NPC target;
        public Player Owner => Main.player[Projectile.owner];

        private enum State
        {
            SpawnIn,
            TriangleLaser,
            Dash,
            Orb,
            Idle,
            Glitching
        }

        State currentState = State.TriangleLaser;

        //How blue the colors should be 0f = pink, 1f = blue
        float dashColorProgress = 0f;

        float orbChargePower = 0f;

        float justShotPower = 0f;

        int timer = 0;
        int substateTimer = 0;

        float overallScale = 1f;
        float overallAlpha = 1f;
        public override void AI()
        {
            FindTarget();

            //currentState = State.Orb;
            switch (currentState)
            {
                case State.TriangleLaser:
                    TriangleLaser();
                    break;
                case State.Dash:
                    Dash();
                    break;
                case State.Orb:
                    Orb();
                    break;
                case State.Idle:
                    Idle();
                    break;
                case State.Glitching:
                    Glitching();
                    break;
            }



            int trailCount = 9;
            previousPositions.Add(Projectile.Center);
            previousRotations.Add(Projectile.rotation);

            if (previousPositions.Count > trailCount)
                previousPositions.RemoveAt(0);

            if (previousRotations.Count > trailCount)
                previousRotations.RemoveAt(0);

            if (timer % 5 == 0)
                Projectile.frame = (Projectile.frame + 1) % 4;

            if (timer % 1 == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Color thisCol = Color.Lerp(Color.LightSkyBlue, Color.DeepSkyBlue, 0.75f);

                    Vector2 myvel = Projectile.rotation.ToRotationVector2() * -(1f + i);

                    Vector2 dustSpawnPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * -20f;

                    float particleScale = isDashing ? 0.7f : 0.5f;

                    FireParticle fire = new FireParticle(dustSpawnPos + Main.rand.NextVector2Circular(2f, 2f), myvel, particleScale, thisCol, colorMult: 0.5f, bloomAlpha: 1f,
                        AlphaFade: 0.88f, RotPower: 0.01f);
                    fire.renderLayer = RenderLayer.UnderProjectiles;
                    ShaderParticleHandler.SpawnParticle(fire);
                }
            }

            justShotPower = Math.Clamp(MathHelper.Lerp(justShotPower, -0.5f, 0.07f), 0f, 1f);

            if (isDashing)
                dashColorProgress = Math.Clamp(MathHelper.Lerp(dashColorProgress, 0.5f, 0.1f), 0f, 1f);
            else
                dashColorProgress = Math.Clamp(MathHelper.Lerp(dashColorProgress, -0.5f, 0.1f), 0f, 1f);

            timer++;
        }

        #region States
        public void SpawnIn()
        {
            substateTimer++;
        }

        Vector2 orbitVector = Main.rand.NextVector2CircularEdge(200f, 200f);
        public void TriangleLaser()
        {
            if (!FindTarget())
                return;

            int timeBeforeShot = 25; //25 | 20 --> 20 | 15
            int timeAfterShot = 20;

            if (substateTimer < timeBeforeShot)
            {
                if (substateTimer == 0)
                    orbitVector = Main.rand.NextVector2CircularEdge(200f, 200f);

                //Move to target
                Vector2 goalPos = target.Center + orbitVector;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(goalPos) * (Projectile.Distance(goalPos) / 15), 0.2f); //15 0.2

                Projectile.rotation = (target.Center - Projectile.Center).ToRotation();
            }
            else if (substateTimer < timeBeforeShot + timeAfterShot)
            {
                Vector2 goalPos = target.Center + orbitVector;

                //Shoot
                if (substateTimer == timeBeforeShot)// || substateTimer % 6 == 0)
                {
                    Vector2 toTarget = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);

                    int laser = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, toTarget.RotateRandom(0.1f * 0f) * 6f, ProjectileID.PurpleLaser, 10, 1, Projectile.owner);
                    Main.projectile[laser].scale *= 1f;
                    //Main.projectile[laser].penetrate = 1;

                    //Recoil
                    Projectile.velocity += toTarget * -10f;

                    //Dust
                    Vector2 dustPos = Projectile.Center;
                    Color between = Color.Lerp(Color.DeepPink, Color.HotPink, 0.25f);
                    for (int i = 0; i < 2 + Main.rand.Next(1, 3); i++) //2 //0,3
                    {
                        Dust dp = Dust.NewDustPerfect(dustPos, ModContent.DustType<LineSpark>(),
                            toTarget.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(6f, 22f),
                            newColor: between * 1f, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.45f);

                        dp.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 80,
                            1f, 0.5f);
                    }

                    for (int i = 0; i < 3 + Main.rand.Next(0, 3); i++)
                    {
                        Color col1 = Color.Lerp(Color.DeepPink, Color.HotPink, 0.65f);

                        Vector2 randomStart = Main.rand.NextVector2Circular(4f, 4f) * 1f;
                        Dust dust = Dust.NewDustPerfect(dustPos, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: col1, Scale: Main.rand.NextFloat(0.25f, 0.3f) * 1.15f);
                        dust.noLight = false;
                        dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, preSlowPower: 0.99f, timeBeforeSlow: 0, postSlowPower: 0.89f,
                            velToBeginShrink: 10f, fadePower: 0.93f, shouldFadeColor: false);

                        dust.velocity += toTarget * 6f;
                    }


                    Vector2 vel = toTarget * 1f; //2.5
                    Dust d = Dust.NewDustPerfect(dustPos - toTarget * 2f, ModContent.DustType<CirclePulse>(), vel, newColor: Color.HotPink * 1f);
                    d.scale = 0.05f;
                    CirclePulseBehavior b = new CirclePulseBehavior(0.2f, true, 2, 0.2f, 0.35f);
                    b.drawLayer = RenderLayer.OverPlayers;
                    d.customData = b;


                    SoundStyle style = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_fire") with { Volume = .12f, Pitch = .1f, PitchVariance = .15f, MaxInstances = 1 };
                    SoundEngine.PlaySound(style, Projectile.Center);

                    SoundStyle style2 = new SoundStyle("Terraria/Sounds/Research_1") with { Pitch = .85f, PitchVariance = .2f, Volume = 0.25f };
                    SoundEngine.PlaySound(style2, Projectile.Center);

                    justShotPower = 1f;
                }

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(goalPos) * (Projectile.Distance(goalPos) / 10), 0.15f); //15 0.2
                //Projectile.velocity *= 0.8f;
            }
            else if (substateTimer == timeBeforeShot + timeAfterShot)
            {
                currentState = State.Dash;
                substateTimer = -1;
            }

            substateTimer++;
        }

        bool isDashing = false;
        public void Dash()
        {
            int timeBeforeDash = 50; //50 | 15 | 15
            int timeToBackUp = 15;
            int timeAfterDash = 15;

            //Move closer to target
            if (substateTimer < timeBeforeDash)
            {
                if (substateTimer == 0)
                    orbitVector = orbitVector.SafeNormalize(Vector2.UnitX) * 100f;

                //Back up right before dash
                if (substateTimer == timeBeforeDash - timeToBackUp)
                    orbitVector *= 1.75f;

                Vector2 goalPos = target.Center + orbitVector;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(goalPos) * (Projectile.Distance(goalPos) / 15), 0.2f); //15 0.2

                Projectile.rotation = (target.Center - Projectile.Center).ToRotation();
            }
            //Dash
            else if (substateTimer < timeBeforeDash + timeAfterDash)
            {
                if (substateTimer == timeBeforeDash)
                {
                    Projectile.velocity = Projectile.rotation.ToRotationVector2() * 40f;

                    Dust d2 = Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * 0.75f, ModContent.DustType<CirclePulse>(), Projectile.velocity.SafeNormalize(Vector2.UnitX) * 2f,
                        newColor: Color.DeepSkyBlue * 0.3f);
                    d2.scale = 0.1f;
                    CirclePulseBehavior b2 = new CirclePulseBehavior(0.5f, true, 2, 0.2f, 0.4f);
                    b2.drawLayer = RenderLayer.UnderProjectiles;
                    d2.customData = b2;
                }

                Projectile.velocity *= 0.93f;

                if (Projectile.velocity.Length() > 15f && substateTimer != timeBeforeDash)
                {
                    //Color dustCol = Color.Lerp(Color.DeepPink, Color.HotPink, 0.75f);
                    Color dustCol = Color.Lerp(Color.DeepSkyBlue, Color.SkyBlue, 0.35f);

                    Dust p = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(10f, 10f), ModContent.DustType<WindLine>(),
                        Projectile.velocity.SafeNormalize(Vector2.UnitX) * -2f, newColor: dustCol, Scale: 3f);

                    WindLineBehavior wlb = new WindLineBehavior(VelFadePower: 0.95f, TimeToStartShrink: 0, ShrinkYScalePower: 0.75f, 0.6f, 0.55f, true);
                    wlb.drawWhiteCore = true;
                    p.customData = wlb;
                }

                Projectile.rotation = Projectile.velocity.ToRotation();

                isDashing = true;
            }
            else if (substateTimer == timeBeforeDash + timeAfterDash)
            {
                currentState = Main.rand.NextBool(2) ? State.Orb : State.TriangleLaser;
                orbitVector = Main.rand.NextVector2CircularEdge(200f, 200f);
                isDashing = false;

                substateTimer = -1;
            }
            substateTimer++;
        }

        public void Orb()
        {
            if (!FindTarget())
                return;

            int timeBeforeShot = 70;
            int timeAfterShot = 60;

            if (substateTimer < timeBeforeShot)
            {
                if (substateTimer == 0)
                    orbitVector = Main.rand.NextVector2CircularEdge(280f, 280f);

                //Move to target
                Vector2 goalPos = target.Center + orbitVector;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(goalPos) * (Projectile.Distance(goalPos) / 15), 0.2f); //15 0.2

                Projectile.rotation = (target.Center - Projectile.Center).ToRotation();

                orbChargePower = Utils.GetLerpValue(0, timeBeforeShot, substateTimer, true);

            }
            else if (substateTimer < timeBeforeShot + timeAfterShot)
            {
                Vector2 goalPos = target.Center + orbitVector;

                //Shoot
                if (substateTimer == timeBeforeShot)
                {
                    Vector2 toTarget = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);

                    float offRot = (MathHelper.TwoPi / 3);

                    int laser = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, toTarget.RotatedBy(offRot) * 6f, ModContent.ProjectileType<TrojanForceEnergyBall>(), 10, 1, Projectile.owner);
                    int laser2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, toTarget.RotatedBy(-offRot) * 6f, ModContent.ProjectileType<TrojanForceEnergyBall>(), 10, 1, Projectile.owner);
                    int laser3 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, toTarget * 4f, ModContent.ProjectileType<TrojanForceEnergyBall>(), 10, 1, Projectile.owner);

                    Main.projectile[laser].scale *= 0.65f;
                    Main.projectile[laser2].scale *= 0.65f;
                    Main.projectile[laser3].scale *= 1f;

                    //Recoil
                    Projectile.velocity += toTarget * -14f;
                    Projectile.rotation = toTarget.ToRotation();

                    //Dust
                    Vector2 dustPos = Projectile.Center;
                    Color between = Color.Lerp(Color.DeepPink, Color.HotPink, 0.25f);
                    for (int i = 0; i < 2 + Main.rand.Next(1, 3); i++) //2 //0,3
                    {
                        Dust dp = Dust.NewDustPerfect(dustPos, ModContent.DustType<LineSpark>(),
                            toTarget.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(6f, 22f),
                            newColor: between * 1f, Scale: Main.rand.NextFloat(0.45f, 0.65f) * 0.45f);

                        dp.customData = DustBehaviorUtil.AssignBehavior_LSBase(velFadePower: 0.88f, preShrinkPower: 0.99f, postShrinkPower: 0.8f, timeToStartShrink: 10 + Main.rand.Next(-5, 5), killEarlyTime: 80,
                            1f, 0.5f);
                    }

                    for (int i = 0; i < 3 + Main.rand.Next(0, 3); i++)
                    {
                        Color col1 = Color.Lerp(Color.DeepPink, Color.HotPink, 0.65f);

                        Vector2 randomStart = Main.rand.NextVector2Circular(4f, 4f) * 1f;
                        Dust dust = Dust.NewDustPerfect(dustPos, ModContent.DustType<GlowPixelCross>(), randomStart, newColor: col1, Scale: Main.rand.NextFloat(0.25f, 0.3f) * 1.15f);
                        dust.noLight = false;
                        dust.customData = DustBehaviorUtil.AssignBehavior_GPCBase(rotPower: 0.2f, preSlowPower: 0.99f, timeBeforeSlow: 0, postSlowPower: 0.89f,
                            velToBeginShrink: 10f, fadePower: 0.93f, shouldFadeColor: false);

                        dust.velocity += toTarget * 6f;
                    }

                    CirclePulseBehavior cpb2 = new CirclePulseBehavior(0.35f, true, 1, 0.8f, 0.8f);

                    Dust d1 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<CirclePulse>(), Velocity: Vector2.Zero, newColor: Color.HotPink * 0.5f);
                    d1.scale = 0.1f;
                    d1.customData = cpb2;
                    d1.velocity = new Vector2(-0.01f, 0f);

                    Dust d2 = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<CirclePulse>(), Velocity: Vector2.Zero, newColor: Color.HotPink * 0.5f);
                    d2.scale = 0.1f;
                    d2.customData = cpb2;
                    d2.velocity = new Vector2(0.01f, 0f);

                    SoundStyle style = SoundID.Item75 with { Volume = 0.35f, Pitch = -0.05f, PitchVariance = .05f, MaxInstances = -1 };
                    SoundEngine.PlaySound(style, Projectile.Center);

                    SoundStyle style32 = new SoundStyle("AerovelenceMod/Sounds/Effects/laser_fire") with { Volume = 0.15f, Pitch = 0.25f, MaxInstances = -1, PitchVariance = 0.1f };
                    SoundEngine.PlaySound(style32, Projectile.Center);

                    SoundStyle style23 = SoundID.DD2_SkyDragonsFuryShot with { Pitch = 0.15f, PitchVariance = 0.4f, Volume = 0.3f };
                    SoundEngine.PlaySound(style23, Projectile.Center);

                    SoundStyle stylec = SoundID.Item67 with { Pitch = 0f, Volume = 0.55f, PitchVariance = 0.1f, MaxInstances = -1 }; //1f
                    SoundEngine.PlaySound(stylec, Projectile.Center);

                    justShotPower = 1f;
                }

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(goalPos) * (Projectile.Distance(goalPos) / 10), 0.1f); //15 0.2

                orbChargePower = 0f;
            }
            else if (substateTimer == timeBeforeShot + timeAfterShot)
            {
                currentState = State.Dash;
                substateTimer = -1;
            }

            substateTimer++;
        }

        public void Idle()
        {
            //Every 90 frames, choose a random new location around player and try to move there
            if (substateTimer % 40 == 0)
                orbitVector = Main.rand.NextVector2CircularEdge(70f, 70f);

            Vector2 goalPos = Owner.Center + orbitVector + new Vector2(0f, -40f);

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(goalPos) * (Projectile.Distance(goalPos) / 10), 0.01f); //15 0.2

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.velocity.Length() > 18)
                isDashing = true;
            else
                isDashing = false;


            Main.NewText(Projectile.velocity.Length());
            //Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 30;


            substateTimer++;
        }

        public void Glitching()
        {

            substateTimer++;
        }
        #endregion

        public bool FindTarget(float maxTargetDist = 700f)
        {
            if (Owner.HasMinionAttackTargetNPC) //take the minion target first
            {
                target = Main.npc[Owner.MinionAttackTargetNPC];
                return true;
            }

            foreach (NPC npc in Main.ActiveNPCs) //scan all targets for anything less than maxTargetDist units away
            {
                if (!npc.friendly && npc.CanBeChasedBy(Projectile) && Vector2.Distance(npc.Center, Projectile.Center) < maxTargetDist)
                {
                    target = npc;
                    return true;
                }
            }
            return false;
        }


        public List<float> previousRotations = new List<float>();
        public List<Vector2> previousPositions = new List<Vector2>();
        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D Bot = Mod.Assets.Request<Texture2D>("Content/VFXTest/Aero/TrojanForce/TrojanForceBot").Value;
            Texture2D Glowmask = Mod.Assets.Request<Texture2D>("Content/VFXTest/Aero/TrojanForce/TrojanForceBotGlowmask").Value;
            Texture2D GlowWhite = Mod.Assets.Request<Texture2D>("Content/VFXTest/Aero/TrojanForce/TrojanForceBotGlowWhite").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle sourceRectangle = Bot.Frame(1, 4, frameY: Projectile.frame);
            Vector2 TexOrigin = sourceRectangle.Size() / 2f;
            SpriteEffects SE = Projectile.velocity.X >= 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            //Orb
            Texture2D Glow = CommonTextures.feather_circle128PMA.Value;
            Main.EntitySpriteDraw(Glow, drawPos, null, (isDashing ? Color.DeepSkyBlue : Color.DeepPink) with { A = 0 } * 0.35f, Projectile.rotation, Glow.Size() / 2f, Projectile.scale * 0.75f * overallScale, SpriteEffects.None);


            //Trail
            Color trailCol = isDashing ? Color.DeepSkyBlue : Color.HotPink;
            for (int i = 0; i < previousPositions.Count; i++)
            {
                float progress = (float)i / previousPositions.Count;

                float size = (Projectile.scale * overallScale) - (0.33f - (progress * 0.33f));
                Color col = trailCol with { A = 0 } * progress * 0.25f;


                Vector2 AfterImagePos = previousPositions[i] - Main.screenPosition;

                Main.EntitySpriteDraw(GlowWhite, AfterImagePos, sourceRectangle, col,
                        Projectile.rotation, TexOrigin, size, SE);
            }

            Color between = Color.Lerp(Color.DeepPink, Color.HotPink, 0.5f);
            Color borderCol = isDashing ? Color.DeepSkyBlue : between;
            //Border
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(GlowWhite, drawPos + Main.rand.NextVector2Circular(1.5f, 1.5f), sourceRectangle, borderCol with { A = 0 } * overallAlpha * 0.3f, Projectile.rotation, TexOrigin, Projectile.scale * overallScale, SE, 0f); //0.3
            }

            //Main Tex
            Main.spriteBatch.Draw(Bot, drawPos, sourceRectangle, lightColor * overallAlpha, Projectile.rotation, TexOrigin, Projectile.scale * overallScale, SE, 0f); //0.3

            Main.spriteBatch.Draw(Glowmask, drawPos, sourceRectangle, Color.White * overallAlpha, Projectile.rotation, TexOrigin, Projectile.scale * overallScale, SE, 0f); //0.3

            Main.spriteBatch.Draw(GlowWhite, drawPos, sourceRectangle, (isDashing ? Color.SkyBlue : Color.LightPink) with { A = 0 } * justShotPower, Projectile.rotation, TexOrigin, Projectile.scale * overallScale, SE, 0f); //0.3

            //Star
            Texture2D Star = CommonTextures.CrispStarPMA.Value;

            Vector2 starPos = drawPos + Projectile.rotation.ToRotationVector2() * 14f;
            float starRot = (float)Main.timeForVisualEffects * 0.15f;
            float starScale = Projectile.scale * overallScale * Easings.easeOutQuad(justShotPower) * 1f;

            Main.spriteBatch.Draw(Star, starPos, null, Color.DeepPink with { A = 0 } * 1f, starRot, Star.Size() / 2f, starScale, SE, 0f); //0.3
            Main.spriteBatch.Draw(Star, starPos, null, Color.White with { A = 0 } * 1f, starRot, Star.Size() / 2f, starScale * 0.5f, SE, 0f); //0.3

            //GlitchFX
            Texture2D Line = CommonTextures.Flare.Value;
            for (int i = 0; i < 0; i++)
            {
                Vector2 linePos = drawPos + Main.rand.NextVector2Circular(12f, 12f);

                Main.spriteBatch.Draw(Line, linePos + new Vector2(2f, 0f), null, Color.HotPink with { A = 0 } * 1f, MathHelper.PiOver2, Line.Size() / 2f, new Vector2(1f, 1f) * 0.5f, SE, 0f); //0.3
                Main.spriteBatch.Draw(Line, linePos + new Vector2(-2f, 0f), null, Color.SkyBlue with { A = 0 } * 1f, MathHelper.PiOver2, Line.Size() / 2f, new Vector2(1f, 1f) * 0.5f, SE, 0f); //0.3
                Main.spriteBatch.Draw(Line, linePos, null, Color.Black * 1f, MathHelper.PiOver2, Line.Size() / 2f, new Vector2(1f, 1f) * 0.5f, SE, 0f); //0.3
            }


            //Orb on top
            ModContent.GetInstance<NewPixelationSystem>().QueueRenderAction(RenderLayer.Dusts, () =>
            {
                DrawBasicBall(false);
            });

            return false;
        }

        //I should really just make this a function in utils atp
        public void DrawBasicBall(bool giveUp)
        {
            if (giveUp || orbChargePower == 0)
                return;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 25f;

            //Draw Orb
            Texture2D Orb = CommonTextures.feather_circle128PMA.Value;

            Color[] cols = { Color.White * 1f, Color.HotPink * 0.525f, Color.DeepPink * 0.375f };
            float[] scales = { 0.85f, 1.45f, 2.5f };

            float orbAlpha = 1f;
            float totalScale = Projectile.scale * 0.25f * Easings.easeInOutQuad(orbChargePower); //0.25

            float sineScale1 = 1f + (float)Math.Sin(Main.timeForVisualEffects * 0.12f) * 0.2f;
            float sineScale2 = 1f + (float)Math.Cos(Main.timeForVisualEffects * 0.22f) * 0.11f;

            //Main.EntitySpriteDraw(Orb, drawPos, null, Color.DodgerBlue * orbAlpha * 0.35f, 0f, Orb.Size() / 2f, scales[2] * totalScale, SpriteEffects.None);

            Main.EntitySpriteDraw(Orb, drawPos, null, cols[0] with { A = 0 } * orbAlpha, 0f, Orb.Size() / 2f, scales[0] * totalScale, SpriteEffects.None);
            Main.EntitySpriteDraw(Orb, drawPos, null, cols[1] with { A = 0 } * orbAlpha, 0f, Orb.Size() / 2f, scales[1] * totalScale * sineScale1, SpriteEffects.None);
            Main.EntitySpriteDraw(Orb, drawPos, null, cols[2] with { A = 0 } * orbAlpha, 0f, Orb.Size() / 2f, scales[2] * totalScale * sineScale2, SpriteEffects.None);
        }


    }
}
