using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using AerovelenceMod.Effects;
using Terraria.Enums;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Blocks.CrystalCaverns.Rubble
{
    public class LargeCrystal1 : ModTile
    {
		public override void SetDefaults()
		{
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Large Crystal");
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 6;
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
		}
		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(default, BlendState.Additive, default, default, default, Filters.Scene["AerovelenceMod:TestCrystalShine"].GetShader().Shader);
			Filters.Scene["AerovelenceMod:TestCrystalShine"].GetShader().UseProgress((float)Main.time * 0.02f).UseOpacity(0.8f);

			return (true);
		}
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(default, default, default, default, default, default);
		}


		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height)
		{
			offsetY = 2;
		}
		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			if (!tileBelow.active() || tileBelow.halfBrick() || tileBelow.topSlope())
			{
				WorldGen.KillTile(i, j);
			}

			return true;
		}
	}
}