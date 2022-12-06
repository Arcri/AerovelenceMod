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
	public class SkilStrikeNPC : GlobalNPC
	{

        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {

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
                    SS.crit = crit;
                }
                //Main.NewText("Spawned proj with CT index: " + recent);
            }

            
        }

    }
}