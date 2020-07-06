using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles.Furniture
{
	public class GlimmeringBookcase : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.Height = 4;
			dustType = ModContent.DustType<Sparkle>();
			TileObjectData.newTile.Width = 3;
			AddMapEntry(new Color(068, 077, 098));
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16
			};
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Glimmering Bookcase");
			AddMapEntry(new Color(179, 146, 107), name);
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
			disableSmartCursor = true;
			adjTiles = new int[] { TileID.Bookcases };
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Terraria.Item.NewItem(i * 16, j * 16, 16, 32, ModContent.ItemType<Items.Placeable.CrystalCaverns.GlimmeringBookcaseItem>());
		}
	}
}