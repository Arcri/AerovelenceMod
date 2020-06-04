using AerovelenceMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;

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
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.NotReallySolid[Type] = true;
			TileID.Sets.DrawsWalls[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
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
			name.SetDefault("Kelvin Door");
			AddMapEntry(new Color(200, 200, 200), name);
			dustType = DustType<Sparkle>();
			disableSmartCursor = true;
			adjTiles = new int[] { TileID.ClosedDoor };
		}

		public override bool HasSmartInteract()
		{
			return true;
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = 1;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Item.NewItem(i * 16, j * 16, 16, 48, ItemType<KelvinDoor>());
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.showItemIcon = true;
			player.showItemIcon2 = ItemType<KelvinDoor>();
		}

        [System.Obsolete]
        public override void RightClick(int i, int j)
		{
			if (Main.LocalPlayer.HasItem(mod.ItemType("KelvinChestKey")))
			{
				Main.LocalPlayer.ConsumeItem(mod.ItemType("KelvinChestKey"));
				Main.PlaySound(SoundID.Unlock, Main.LocalPlayer.Center);
				Dust.NewDust(new Vector2(i * 16, j * 16), 1, 3, mod.DustType("Sparkle"));
			}
			Tile tile = Main.tile[i, j];
			int topY = j - tile.frameY / 18 % 3;
			short frameAdjustment = (short)(tile.frameX > 0 ? -18 : 18);
			Main.tile[i, topY].frameX += frameAdjustment;
			Main.tile[i, topY + 1].frameX += frameAdjustment;
			Main.tile[i, topY + 2].frameX += frameAdjustment;
			NetMessage.SendTileSquare(-1, i, topY + 1, 3, TileChangeType.None);
            {
                _ = Main.tile[i, j];
                int width = 16;
				int offsetY = 0;
				int height = 16;
				TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height);
				var flameTexture = mod.GetTexture("Blocks/FrostDungeon/Furniture/KelvinDoorClosed"); // We could also reuse Main.FlameTexture[] textures, but using our own texture is nice.
			}
		}
	}
}