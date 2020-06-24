using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles.Flora
{
	public class CrystalFlora6 : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			TileObjectData.addTile(Type);
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = true;
			Main.tileLighted[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.addTile(Type);
			dustType = 206;
			soundType = SoundID.Grass;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 10;
		}
	}
}