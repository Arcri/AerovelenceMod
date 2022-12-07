using AerovelenceMod.Common.Utilities;
using AerovelenceMod.Content.Dusts.GlowDusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using tModPorter;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Common.Globals.SkillStrikes
{
	public class SkillStrikeNPC : GlobalNPC
	{
        public override bool InstancePerEntity => true;

        public bool strikeCTRemove = true;

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            //Main.NewText("strike");
            return base.StrikeNPC(npc, ref damage, defense, ref knockback, hitDirection, ref crit);
        }
        public override void HitEffect(NPC npc, int hitDirection, double damage)
        {

        }
        public override void OnHitNPC(NPC npc, NPC target, int damage, float knockback, bool crit)
        {
            Main.NewText("on hit NPC");
            base.OnHitNPC(npc, target, damage, knockback, crit);
        }

        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            int recent = -1;
            for (int i = 99; i >= 0; i--)
            {
                CombatText ctToCheck = Main.combatText[i];
                if (ctToCheck.lifeTime == 60 || ctToCheck.lifeTime == 120)
                {
                    if (ctToCheck.alpha == 1f)
                    {
                        if ((ctToCheck.color == CombatText.DamagedHostile || ctToCheck.color == CombatText.DamagedHostileCrit))
                        {
                            recent = i;
                            i = 0;
                        }
                    }
                }
            }
            CombatText anchor = Main.combatText[recent];
            anchor.color = Color.White * 0f;
            int a = Projectile.NewProjectile(null, anchor.position, anchor.velocity, ProjectileType<SkillStrikeProj>(), 0, 0, Main.myPlayer, recent, recent);
            if (Main.projectile[a].ModProjectile is SkillStrikeProj SS)
            {
                SS.damageNumber = anchor.text;
                SS.skillCrit = false;
                SS.superCrit = false;
            }

            base.OnHitByItem(npc, player, item, damage, knockback, crit);
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            //Main.NewText("OnHitByProjectile");

            int recent = -1;
            for (int i = 99; i >= 0; i--)
            {
                CombatText ctToCheck = Main.combatText[i];
                if (ctToCheck.lifeTime == 60 || ctToCheck.lifeTime == 120)
                {
                    if (ctToCheck.alpha == 1f)
                    {
                        if ( (ctToCheck.color == CombatText.DamagedHostile || ctToCheck.color == CombatText.DamagedHostileCrit) )
                        {
                            recent = i;
                            break;
                        }
                    }
                }
            }

            if (projectile.GetGlobalProjectile<SkillStrikeGProj>().SkillStrike)
            {
                if (crit)
                    Main.combatText[recent].color = Color.Purple * 0f;
                else
                    Main.combatText[recent].color = Color.Gold * 0f;

                CombatText anchor = Main.combatText[recent];
                int a = Projectile.NewProjectile(null, anchor.position, anchor.velocity, ProjectileType<SkillStrikeProj>(), 0, 0, Main.myPlayer, recent, recent);
                if (Main.projectile[a].ModProjectile is SkillStrikeProj SS)
                {
                    SS.damageNumber = anchor.text;
                    SS.skillCrit = true;
                    SS.superCrit = crit;
                }
                //Main.NewText("Spawned proj with CT index: " + recent);
            } else
            {
                /*
                CombatText anchor = Main.combatText[recent];
                anchor.color = Color.White * 0f;
                int a = Projectile.NewProjectile(null, anchor.position, anchor.velocity, ProjectileType<SkillStrikeProj>(), 0, 0, Main.myPlayer, recent, recent);
                if (Main.projectile[a].ModProjectile is SkillStrikeProj SS)
                {
                    SS.damageNumber = anchor.text;
                    SS.skillCrit = false;
                    SS.superCrit = false;
                }
                */
            }

            
        }

        
    }

}