using AerovelenceMod.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Rubble
{
    public class LargeCrystal5 : ModTile
	{
		public override void SetStaticDefaults()
		{
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Large Crystal");
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
		}
		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.351f;
			g = 0.3f;
			b = 0.078f;
		}

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
			//drawColor *= 0.6f;

			if (Main.rand.NextBool(400))
			{
				Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, ModContent.DustType<Dusts.CavernCrystalDust>());
			}
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(default, BlendState.Additive, default, default, default, Filters.Scene["AerovelenceMod:CavernCrystalShine"].GetShader().Shader);
			Filters.Scene["AerovelenceMod:CavernCrystalShine"].GetShader().ApplyTime((float)Main.time * 0.02f).ApplyOpacity(0.8f);

			return (true);
		}
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(default, default, default, default, default, default);
		}
		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
		{
			offsetY = 2;
		}
		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			if (!tileBelow.HasTile || tileBelow.IsHalfBlock || tileBelow.TopSlope)
			{
				WorldGen.KillTile(i, j);
			}

			return true;
		}
	}
}