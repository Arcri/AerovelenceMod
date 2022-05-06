using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.Ores
{
    public class BurnshockBarPlaced : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileShine[Type] = 1100;
			Main.tileSolid[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;

			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);
			
			
			AddMapEntry(new Color(110, 074, 056), Language.GetText("Burnshock Bar"));
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frame = frameCounter / 54;
			frameCounter += 6;
			if (frame == 2)
			{
				frame = 0;
				frameCounter = 0;
			}
		}

		public override bool Drop(int i, int j)
		{
			Tile t = Main.tile[i, j];
			int style = t.TileFrameX / 18;
			if (style == 0)
			{
				Item.NewItem(i * 16, j * 16, 16, 16, Mod.Find<ModItem>("BurnshockBar").Type);
			}
			return base.Drop(i, j);
		}
	}
}