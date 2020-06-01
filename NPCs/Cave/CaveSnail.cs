using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Cave
{
	public class CaveSnail : ModNPC
	{
		public override void SetStaticDefaults()
		{
	        DisplayName.SetDefault("Cave Snail");
		}
        public override void SetDefaults()
        {
            npc.aiStyle = 67;
            npc.lifeMax = 4;
            npc.defense = 24;
            npc.knockBackResist = 0f;
            npc.width = 38;
            npc.height = 18;
            npc.value = Item.buyPrice(0, 0, 4, 50);
            npc.lavaImmune = true;
            npc.noGravity = false;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.buffImmune[24] = true;
		}
	}
}