using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Blocks.CrystalCaverns.Walls
{
	public class CavernWallUnsafe : ModWall
	{
		public override void SetDefaults()
		{
			Main.wallHouse[Type] = false;
			AddMapEntry(new Color(54, 87, 129));
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
	}
}