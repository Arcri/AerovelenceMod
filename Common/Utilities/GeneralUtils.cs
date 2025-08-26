using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using System.Linq;
using Terraria.Audio;
using AerovelenceMod.Content.Dusts.GlowDusts;
using AerovelenceMod.Common.Globals.SkillStrikes;
using Terraria.DataStructures;
using static Terraria.NPC;

namespace AerovelenceMod.Common.Utilities
{
    public static class GeneralUtils
    {

        //Strikes all hostile npcs in a circle
        public static void strikeNPCsInRadius(Vector2 position, float radius, float damage, float knockback)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && Vector2.Distance(position, Main.npc[i].Center) < radius)
                {
                    int Direction = 0;
                    if (position.X - Main.npc[i].Center.X < 0)
                        Direction = 1;
                    else
                        Direction = -1;

                    HitInfo myHit = new HitInfo();
                    myHit.Damage = (int)(damage * Main.rand.NextFloat(0.85f, 1.15f)); //<- Vanilla damage variance values
                    myHit.Knockback = knockback * Main.npc[i].knockBackResist;
                    myHit.HitDirection = Direction;

                    Main.npc[i].StrikeNPC(myHit);
                }
            }

        }

        //Overload that allows you to ignore a certain npc
        //Useful for having an explosion trigger on hitting an enemy but not having that enemy be double-hit
        public static void strikeNPCsInRadius(Vector2 position, float radius, float damage, float knockback, int exceptionID)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && exceptionID != i && Vector2.Distance(position, Main.npc[i].Center) < radius)
                {
                    int Direction = 0;
                    if (position.X - Main.npc[i].Center.X < 0)
                        Direction = 1;
                    else
                        Direction = -1;

                    HitInfo myHit = new HitInfo();
                    myHit.Damage = (int)(damage * Main.rand.NextFloat(0.85f, 1.15f)); //<- Vanilla damage variance values
                    myHit.Knockback = knockback * Main.npc[i].knockBackResist;
                    myHit.HitDirection = Direction;

                    Main.npc[i].StrikeNPC(myHit);
                }
            }

        }
    }
}
