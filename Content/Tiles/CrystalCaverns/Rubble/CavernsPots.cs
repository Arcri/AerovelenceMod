using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble
{
    public class CavernsPots : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileWaterDeath[Type] = false;
			Main.tileOreFinderPriority[Type] = 1000;
			Main.tileSpelunker[Type] = true;
			Main.tileCut[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Pot");
			DustType = 1;
			HitSound = SoundID.Shatter;
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 59;
		}
	}
}
