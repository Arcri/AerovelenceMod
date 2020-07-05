using AerovelenceMod.Dusts;
using AerovelenceMod.Items.Placeable.CrystalCaverns;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles.Furniture
{
    public class GlimmeringBathtub : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Glimmering Bathtub");
			AddMapEntry(new Color(200, 200, 200), name);
			dustType = DustType<Sparkle>();
			disableSmartCursor = true;
			adjTiles = new int[] { TileID.Bathtubs };
		}



		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 64, 32, ItemType<GlimmeringBathtubItem>());
		}
	}
}