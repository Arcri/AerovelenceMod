using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Terraria.GameContent.Generation;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using AerovelenceMod.Dusts;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace AerovelenceMod.Blocks.CrystalCaverns.Tiles
{
	public class CrystalGrass : ModTile
	{
		public static int _type;

		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			SetModTree(new CrystalTree());
			Main.tileMerge[Type][mod.TileType("CrystalGrass")] = true;
			Main.tileBlendAll[this.Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			//Main.tileLighted[Type] = true;
			AddMapEntry(new Color(099, 155, 255));
			drop = mod.ItemType("CrystalDirt");
			TileID.Sets.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = mod.TileType("CrystalDirt");

		}



		public override void FloorVisuals(Player player)
		{
			Vector2 playerPosition = player.Center + new Vector2(-7, player.height / 3);
			if (player.velocity.X != 0)
			{
				Dust.NewDust(playerPosition, 16, 1, ModContent.DustType<CrystalLeaves>(), 0, 0.15f);
			}
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
     	{
			fail = true;
			if (Type == ModContent.TileType<CrystalGrass>())
			{
				Main.tile[i, j].type = (ushort)ModContent.TileType<CrystalDirt>();
			}
		}


		public static bool PlaceObject(int x, int y, int type, bool mute = false, int style = 0, int alternate = 0, int random = -1, int direction = -1)
		{
			TileObject toBePlaced;
			if (!TileObject.CanPlace(x, y, type, style, direction, out toBePlaced, false))
			{
				return false;
			}
			toBePlaced.random = random;
			if (TileObject.Place(toBePlaced) && !mute)
			{
				WorldGen.SquareTileFrame(x, y, true);
			}
			return false;
		}


		public override void RandomUpdate(int i, int j)
		{
			WorldGen.SpreadGrass(i, j, mod.TileType("CrystalDirt"), mod.TileType("CrystalGrass"), true, Main.tile[i, j].color());
			{
				if (!Framing.GetTileSafely(i, j - 1).active() && Main.rand.Next(1) == 0)
				{
					int style = Main.rand.Next(2);
					if (PlaceObject(i, j - 1, Flora.CrystalFlora._type, false, style))
						NetMessage.SendObjectPlacment(-1, i, j - 1, Flora.CrystalFlora._type, style, 0, -1, -1);
				}

			}

		}

		public override int SaplingGrowthType(ref int style)
		{
			style = 0;
			return mod.TileType("CrystalSapling");
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.028f;
			g = 0.153f;
			b = 0.081f;
		}
	}
}