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
using static Terraria.NPC;
using Terraria.IO;

namespace AerovelenceMod.Content.Buffs
{
    //Based heavily off SOTS Intimidating Presence
    public class FearsomeFoe : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            Main.debuff[Type] = false;
        }
    }

    public class FearsomeFoeGNPC : GlobalNPC
    {

        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            if (player.HasBuff(ModContent.BuffType<FearsomeFoe>()))
            {
                spawnRate = (int)(spawnRate * 10); //1/10 of normal spawn rate
                maxSpawns = (int)(maxSpawns * 0.5f); 
            }
        }
    }
}