using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Flora
{
	public class LuminGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileMergeDirt[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			DustType = 116;
			SoundType = SoundID.Grass;
			AddMapEntry(new Color(100, 125, 255));
			TileObjectData.addTile(Type);
		}
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			Color colour = Color.White;

			Texture2D glow = (Texture2D)ModContent.Request<Texture2D>("AerovelenceMod/Content/Tiles/CrystalCaverns/Tiles/Flora/LuminGrass_Glow");
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			Main.spriteBatch.Draw(glow, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), colour);
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 2;
		}
	}
}