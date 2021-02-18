using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles.Flora
{
	public class LuminGrass : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileNoFail[Type] = true;
			Main.tileMergeDirt[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			dustType = 116;
			soundType = SoundID.Grass;
			AddMapEntry(new Color(100, 125, 255));
			TileObjectData.addTile(Type);
		}
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			Color colour = Color.White;

			Texture2D glow = ModContent.GetTexture("AerovelenceMod/Blocks/CrystalCaverns/Tiles/Flora/LuminGrass_Glow");
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			spriteBatch.Draw(glow, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.frameX, tile.frameY, 16, 16), colour);
		}
		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 2;
		}
	}
}