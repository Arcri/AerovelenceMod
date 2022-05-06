using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Flora
{
	public class VividVines : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.CoordinateHeights = new int[]{ 16, 16 };
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = default;
			TileObjectData.addTile(Type);
			DustType = 59;
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Vine");
			AddMapEntry(new Color(104, 156, 70), name);
		}
		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tileAbove = Framing.GetTileSafely(i, j + 1);
			int type = -1;

			if (tileAbove.HasTile && !tileAbove.BottomSlope)
			{
				type = tileAbove.TileType;
			}

			if (type == ModContent.TileType<CavernStone>() || type == Type)
			{
				return true;
			}

			WorldGen.KillTile(i, j);
			return true;
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 59;
		}
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			Color colour = Color.White;

			Texture2D glow = (Texture2D)ModContent.Request<Texture2D>("Content/Tiles/CrystalCaverns/Tiles/Flora/VividVines_Glow");
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			Main.spriteBatch.Draw(glow, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), colour);
		}
	}
}