using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace AerovelenceMod.Content.NPCs.Bosses.FeatheredFoe
{
    public partial class FeatheredFoe : ModNPC
    {
        Vector2 basicAttackPoint = Vector2.Zero;
        public void BasicAttack()
        {
            //Move to point
            //Stop
            //Shoot 
            //Repeat

            if (substate == 1)
            {
                if (timer == 0)
                {
                    basicAttackPoint = Main.rand.NextVector2CircularEdge(550, 250);
                }

                //BasicMovementVariant1(player.Center + basicAttackPoint, 0.06f, 22, 2, 0.1f, 60);
                
                //////BasicMovementVariant2(player.Center + basicAttackPoint); bad
                
                BasicMovementVariant3(player.Center + basicAttackPoint, 3f, 270f);

                if (timer == 100)
                {
                    timer = -1;
                    substate++;
                }
            }
            else if (substate == 2)
            {
                //NPC.velocity = Vector2.Zero;

                //BasicMovementVariant1(player.Center + basicAttackPoint);

                BasicMovementVariant3(player.Center + basicAttackPoint, 3f, 540f);


                if (timer > 20 && timer < 65 && timer % 8 == 0)
                {
                    Vector2 vel = Main.rand.NextVector2CircularEdge(8f, 8f) * Main.rand.NextFloat();

                    int feather = Projectile.NewProjectile(null, NPC.Center, vel, ModContent.ProjectileType<OrbitingFeather>(), 2, 0, Main.myPlayer);

                    if (Main.projectile[feather].ModProjectile is OrbitingFeather of)
                    {
                        of.timeToOrbit = 0;
                        of.orbitVector = vel;
                        of.orbitVal = 300;
                        of.rotSpeed = 0;
                    }
                }

                if (timer == 100) //100
                {
                    substate = 1;
                    timer = -1;
                }
            }

            Dust.NewDustPerfect(player.Center + basicAttackPoint, ModContent.DustType<StillDust>(), Velocity: Vector2.Zero, newColor: Color.Black, Scale: 2f);
        }

        public void SwoopFeatherBehind()
        {

        }

        Vector2 fiveSpreadGoalVec = Vector2.Zero;
        Vector2 fiveSpreadStartVec = Vector2.Zero;
        float fiveSpreadRotAmount = 0f;
        Vector2 resultingVec = Vector2.Zero;
        public void FiveSpread()
        {
            //Choose point oposite player
            //
            //

            Vector2 toPlayer = (NPC.Center - player.Center).SafeNormalize(Vector2.UnitX);

            BasicMovementVariant2(new Vector2(0f, -300f));

            /*
            if (substate == 1) 
            {
                if (timer == 0)
                {
                    Vector2 toPlayer = (NPC.Center - player.Center).SafeNormalize(Vector2.UnitX);

                    fiveSpreadStartVec = toPlayer;

                    float randAmount = Main.rand.NextFloat(-0.1f, 0.11f);
                    float rotAmount = ((MathHelper.Pi * 0.85f) + randAmount);// * (Main.rand.NextBool() ? 1f : -1f);

                    fiveSpreadGoalVec = toPlayer.RotatedBy(rotAmount); //(toPlayer * -1f).RotatedByRandom(0.75f);
                }


                float easingProg = Utils.GetLerpValue(0f, 1f, timer / 70f, true);

                float functedEase = Easings.easeInOutHarsh(easingProg);

                
                float dist = 425f;

                float currentVecAngle = fiveSpreadStartVec.ToRotation().AngleLerp(fiveSpreadGoalVec.ToRotation(), functedEase); 
                    //MathHelper.Lerp(fiveSpreadStartVec.ToRotation(), fiveSpreadGoalVec.ToRotation(), Easings.easeInOutHarsh(easingProg));

                Vector2 npcPosFromPlayer = currentVecAngle.ToRotationVector2() * dist;

                NPC.Center = player.Center + npcPosFromPlayer;
                NPC.velocity = Vector2.Zero;

                if (timer == 75)
                {

                    resultingVec = (NPC.Center - player.Center);
                    substate++;
                    timer = -1;
                }
            }
            else if (substate == 2)
            {
                //BasicMovementVariant1(player.Center + resultingVec);

                if (timer == 30)
                {
                    timer = -1;
                    substate = 1;
                }

            }
            */
        }

        public void MartletOrbitFeather()
        {

        }

        public void CircleBurstFeather()
        {

        }

        public void SwirlFeather()
        {

        }

        public void CornerTravelShot()
        {

        }

        //Based off Emode Cryogen
        void BasicMovementVariant1(Vector2 goalPos, float accel = 0.03f, float maxSpeed = 15, float minSpeed = 5, float decel = 0.06f, float slowdown = 30)
        {
            if (NPC.Distance(goalPos) > slowdown)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, (goalPos - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed, accel);
            }
            else
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, (goalPos - NPC.Center).SafeNormalize(Vector2.Zero) * minSpeed, decel);
            }
        }

        void BasicMovementVariant2(Vector2 goalPos)
        {
            Vector2 trueTarget = goalPos;
            float moveSpeed = 8f; //4f

            if (NPC.Distance(trueTarget) > moveSpeed * 2f)
            {
                NPC.velocity += NPC.DirectionTo(trueTarget) * moveSpeed;
            }

            float clampedDistance = Math.Clamp(NPC.Distance(trueTarget), 5f, 800f);

            float velocityMult = MathHelper.Lerp(0.5f, 0.9f, clampedDistance / 800f);
                
                //Utils.GetLerpValue(0.9f, 0.9f, (clampedDistance / 800f), true);
            //Main.NewText((clampedDistance / 800f));

            NPC.velocity *= velocityMult;
            
        }

        void BasicMovementVariant3(Vector2 goalPos, float moveSpeed = 6f, float clampDistance = 240f)
        {
            NPC.velocity *= 0.875f;

            Vector2 goTo = goalPos - NPC.Center;
            float speed = moveSpeed * MathHelper.Clamp(goTo.Length() / clampDistance, 0, 1);
            if (goTo.Length() < speed)
            {
                speed = goTo.Length();
            }
            NPC.velocity += goTo.SafeNormalize(Vector2.Zero) * speed;

        }
    }
}
