using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
                    basicAttackPoint = Main.rand.NextVector2CircularEdge(550, 550);
                }

                BasicMovementVariant1(player.Center + basicAttackPoint, 0.06f, 22, 5, 0.1f, 60);

                if (timer == 100)
                {
                    timer = -1;
                    substate++;
                }
            }
            else if (substate == 2)
            {
                //NPC.velocity = Vector2.Zero;

                BasicMovementVariant1(player.Center + basicAttackPoint);


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

                if (timer == 100)
                {
                    substate = 1;
                    timer = -1;
                }
            }


        }

        public void SwoopFeatherBehind()
        {

        }

        public void FiveSpread()
        {

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

        }
    }
}
