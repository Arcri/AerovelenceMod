using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Terraria.DataStructures;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;

namespace AerovelenceMod.Content.Projectiles.Weapons.Magic
{
    public class CrystalSpray_Proj : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_0";
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
        }
        public override void AI()
        {
    
            if(FindNearestNPC(300f, true, false, true, out int index))
            {
                NPC npc = Main.npc[index];
                Projectile.velocity *= .98f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(npc.Center) * 11.014f, .018f);
                //For some reason, directionTo*length seems to slow down sometimes :shrug:
            }
            else
            {
                Projectile.velocity.Y += .056f;
                Projectile.velocity.X *= .985f;
            }
            if(Main.rand.NextBool(20))
            {
                float randomRot = Main.rand.NextFloat(6.28f);

                ArmorShaderData dustShader = new ArmorShaderData(new Ref<Effect>(Mod.Assets.Request<Effect>("Effects/GlowDustShader", AssetRequestMode.ImmediateLoad).Value), "ArmorBasic");
                int a = GlowDustHelper.DrawGlowDust(Projectile.Center, 1, 1, ModContent.DustType<Dusts.GlowDusts.GlowCircleFlare>(), 0,
                    0, Color.DodgerBlue, Main.rand.NextFloat(.5f, .7f), 0.55f, 0, dustShader);
                Main.dust[a].noLight = true;
                Main.dust[a].velocity = Vector2.Zero;
                Main.dust[a].velocity = Projectile.velocity;

                //a.position += new Vector2(0,1).RotatedBy(Projectile.velocity.ToRotation()) * 32 * Projectile.direction;
                Main.dust[a].rotation = randomRot;
                //a.position += randomRot.ToRotationVector2() * -8;
            }
            else
            {
                Dust d = Dust.NewDustDirect(Projectile.position, 1, 1, Main.rand.Next(new int[] { DustID.DungeonWater, DustID.SpectreStaff }));
                d.velocity = Projectile.velocity;
                d.noGravity = true;
                d.color = Color.Aqua;
            }
        }
        private bool FindNearestNPC(float range, bool scanTiles, bool targetIsFriendly, bool ignoreCritters, out int npcIndex)
        {
            npcIndex = -1;
            bool foundNPC = false;
            double dist = range * range;
            for(int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                //Make sure NPC is valid anyway
                if (npc.active && npc.life > 0)
                {
                    //Target and NPC friendliness are same
                    if (npc.friendly == targetIsFriendly)
                    {
                        //if ignoring critters, make sure lifemax > 10, id is not dummy, and npc does not drop item
                        if ((!(npc.lifeMax < 10 || npc.type == NPCID.TargetDummy || npc.catchItem != 0) && ignoreCritters) || !ignoreCritters)
                        {
                            //cache this
                            float compDist = Projectile.DistanceSQ(npc.Center);
                            //Distance is shorter than current distance, but did not overflow (underflow)
                            if (compDist < dist && compDist > 0)
                            {
                                //ignore tiles, OR scan tiles and can hit anyway
                                if (!scanTiles || (scanTiles && Collision.CanHit(Projectile, new NPCAimedTarget(npc))))
                                {
                                    npcIndex = i;
                                    dist = compDist;
                                    foundNPC = true;
                                }
                            }
                        }
                    }
                }
            }
            //Case: Failed to Find NPC
            if (!foundNPC)
                npcIndex = -1;
            return foundNPC;
        }
    }
}
