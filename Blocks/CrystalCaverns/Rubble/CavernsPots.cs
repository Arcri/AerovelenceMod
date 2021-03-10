using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;

namespace AerovelenceMod.Blocks.CrystalCaverns.Rubble
{
    public class CavernsPots : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileWaterDeath[Type] = false;
			Main.tileValue[Type] = 1000;
			Main.tileSpelunker[Type] = true;
			Main.tileCut[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Pot");
			dustType = 1;
			soundType = SoundID.Shatter;
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 59;
		}
	}
}