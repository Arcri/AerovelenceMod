using AerovelenceMod.Content.Items.Placeables.Arsenal;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Tiles.Arsenal
{
	public class LaserTrap : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.DrawsWalls[Type] = true;

			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileFrameImportant[Type] = true;

			AddMapEntry(new Color(21, 179, 192), Language.GetText("MapObject.Trap"));
		}

		public int TrapTimer = 0;

		public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameY / 18);

		public override bool IsTileDangerous(int i, int j, Player player) => true;

		public override bool CreateDust(int i, int j, ref int type)
		{
			int style = Main.tile[i, j].TileFrameY / 18;
			if (style == 0)
				type = 13;
			return true;
		}

		public override void PlaceInWorld(int i, int j, Item item)
		{
			int style = Main.LocalPlayer.HeldItem.placeStyle;
			Tile tile = Main.tile[i, j];
			tile.TileFrameY = (short)(style * 18);
			if (Main.LocalPlayer.direction == 1)
			{
				tile.TileFrameX += 18;
			}
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
			}
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{

			Tile tile = Main.tile[i, j];
			int style = tile.TileFrameY / 18;
			Vector2 spawnPosition;
			int horizontalDirection = (tile.TileFrameX == 0) ? -1 : ((tile.TileFrameX == 18) ? 1 : 0);
			int verticalDirection = (tile.TileFrameX < 36) ? 0 : ((tile.TileFrameX < 72) ? -1 : 1);
			TrapTimer++;
			if (TrapTimer % 50 == 0)
			{
				spawnPosition = new Vector2(i * 16 + 8 + 0 * horizontalDirection, j * 16 + 9 + 0 * verticalDirection);
				//Projectile.NewProjectile(spawnPosition, new Vector2(horizontalDirection, verticalDirection) * 6f, ProjectileID.FrostBlastHostile, 20, 2f, Main.myPlayer);
				TrapTimer = 0;
			}
		}
	}
}