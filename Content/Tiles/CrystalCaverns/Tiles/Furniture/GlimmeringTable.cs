using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture
{
	public class GlimmeringTable : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileTable[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 18 };
			TileObjectData.addTile(Type);
			AdjTiles = new int[] { TileID.Tables2 };
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Glimmering Table");
			DustType = ModContent.DustType<Sparkle>();
			AddMapEntry(new Color(068, 077, 098), name);
			TileID.Sets.DisableSmartCursor[Type] = true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
		{
			offsetY = -2;
		}
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<Items.Placeables.Furniture.Glimmering.GlimmeringTable>());
		}
	}
}