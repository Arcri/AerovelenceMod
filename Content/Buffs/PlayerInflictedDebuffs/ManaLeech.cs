using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using AerovelenceMod.Common.Utilities;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using AerovelenceMod.Content.Dusts.GlowDusts;
using System;
using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Projectiles;

namespace AerovelenceMod.Content.Buffs.PlayerInflictedDebuffs
{
    public class ManaLeech : ModBuff
    {
        public int timer = 0;
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;  // Is it a debuff?
            Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world

        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.type != NPCID.TargetDummy) //Im not letting this happen lol
            {
                npc.GetGlobalNPC<ManaLeechModNPC>().ManaLeechDebuff = true;
                timer++;
            }
        }
    }

    public class ManaLeechModNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool ManaLeechDebuff = false;
        public float ManaLeechTime = 0f;

        public override void ResetEffects(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<ManaLeech>()))
            {
                ManaLeechDebuff = false;
                ManaLeechTime = 0;
                
            } else
            {

            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (ManaLeechDebuff)
            {
                if (ManaLeechTime % 14 == 0)
                {
                    Projectile.NewProjectile(null, npc.Center, Main.rand.NextVector2CircularEdge(7,7), ModContent.ProjectileType<ManaLeechStar>(), 
                        0, 0, Main.myPlayer);
                }
                if (ManaLeechTime % 1 == 0)
                {
                    int dust = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowPixelRise>(), Scale: 0.5f, newColor: Color.DodgerBlue);
                    Main.dust[dust].velocity.Y = Math.Abs(Main.dust[dust].velocity.Y) * -1;
                    Main.dust[dust].velocity.X *= 0.5f;
                    Main.dust[dust].alpha = 2;
                    Main.dust[dust].noLight = true;
                    //Main.dust[dust].rotation = Main.rand.NextFloat(6.28f);


                    if (ManaLeechTime % 8 == 0)
                    {
                        int dust2 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowPixelRise>(), Scale: 0.5f, newColor: Color.DodgerBlue);
                        Main.dust[dust2].velocity.Y = Math.Abs(Main.dust[dust2].velocity.Y) * -1;
                        Main.dust[dust2].velocity.X *= 0.85f;
                        Main.dust[dust2].alpha = 2;
                        Main.dust[dust2].noLight = true;
                        //Main.dust[dust2].rotation = Main.rand.NextFloat(6.28f);
                    }

                }

                if (ManaLeechTime % 4 == 0)
                {
                    int dust3 = Dust.NewDust(npc.position, npc.width, npc.height, ModContent.DustType<GlowPixelCross>(), newColor: Color.DodgerBlue);
                    Main.dust[dust3].scale *= 0.35f;
                }

                if (ManaLeechTime % 17 == 0)
                {
                    int dust3 = Dust.NewDust(npc.position, npc.width, npc.height, DustID.PortalBoltTrail, newColor: Color.DeepSkyBlue);
                    Main.dust[dust3].velocity.Y = Math.Abs(Main.dust[dust3].velocity.Y) * -2;
                }

                ManaLeechTime++;
            }
        }
    }
}