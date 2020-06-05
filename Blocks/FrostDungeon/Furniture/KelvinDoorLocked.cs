using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace AerovelenceMod.Blocks.FrostDungeon.Furniture
{
	public class KelvinDoorLocked : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = false;
			TileID.Sets.NotReallySolid[Type] = true;
			TileID.Sets.DrawsWalls[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 1);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 2);
			TileObjectData.addAlternate(0);
			TileObjectData.addTile(Type);
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsDoor);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Kelvin Locked Door");
			AddMapEntry(new Color(200, 100, 200), name);
			disableSmartCursor = true;
			minPick = 9999;
		}

		public override bool HasSmartInteract()
		{
			return true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.showItemIcon = true;
			player.showItemIcon2 = mod.ItemType("KelvinChestKey");
		}

        [System.Obsolete]
        public override void RightClick(int i, int j)
		{
			if(Main.LocalPlayer.HasItem(mod.ItemType("KelvinChestKey")))
			{
				Tile tile = Main.tile[i, j];
				int topY = j - tile.frameY / 17 % 3;
   				Main.tile[i, topY].type = 10;
            			Main.tile[i, topY + 1].type = 10;
            			Main.tile[i, topY + 2].type = 10;
				Main.tile[i, topY].frameX = 54;
            			Main.tile[i, topY + 1].frameX = 54;
            			Main.tile[i, topY + 2].frameX = 54;
				Main.LocalPlayer.ConsumeItem(mod.ItemType("KelvinChestKelvinChestKey"));
				Main.PlaySound(SoundID.Unlock, Main.LocalPlayer.Center);
			}
		}
	}
}