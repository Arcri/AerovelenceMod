using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Blocks.CrystalCaverns.Rubble
{
  public class CavernStalactite : ModTile
  {
        public override void SetDefaults()
        {
			Main.tileSolidTop[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileNoFail[Type] = false;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
			TileObjectData.newTile.AnchorInvalidTiles = new[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
        }

        public virtual void SetDrawPositions(
			int i,
			int j,
			ref int width,
			ref int offsetY,
			ref int height)
			{
				Tile tile = ((Tile[,]) Main.tile)[i, j];
				if (tile.frameY <= 18 || tile.frameY == 72)
					{
						offsetY = -2;
					}
				else
				{
					if ((tile.frameY < 36 || tile.frameY > 54) && tile.frameY != 90)
					return;
					offsetY = 2;
				}
			}
		}
	}