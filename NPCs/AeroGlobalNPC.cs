using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.NPCs
{
	public class AeroGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;
		
		public bool SoulFire;

		public override void ResetEffects(NPC npc)
		{
			SoulFire = false;
		}


        public override bool CheckDead(NPC npc)
        {
			if (npc.type == NPCID.EyeofCthulhu && !NPC.downedBoss1)
			{
				for (int k = 0; k < (int)((double)(Main.maxTilesX * Main.maxTilesY * 13) * 2E-05); k++)
				{
					int EEXX = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
					int WHHYY = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 130);
					if (Main.tile[EEXX, WHHYY] != null)
					{
						if (Main.tile[EEXX, WHHYY].active())
						{
							WorldGen.OreRunner(EEXX, WHHYY, (double)WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), (ushort)mod.TileType("PhanticOreBlock"));
						}
					}
				}
				Main.NewText("Phantom stones formed in the caves!", 180, 60, 140);
			}
			return true;
		}

        public override void DrawEffects(NPC npc, ref Color drawColor)
		{
			if (SoulFire)
			{
				if (Main.rand.Next(4) < 3)
				{
					int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, DustType<WispDust>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.8f;
					Main.dust[dust].velocity.Y -= 0.5f;
					if (Main.rand.NextBool(4))
					{
						Main.dust[dust].noGravity = false;
						Main.dust[dust].scale *= 0.5f;
					}
				}
				Lighting.AddLight(npc.position, 0.1f, 0.2f, 0.7f);
			}
		}
	}
}