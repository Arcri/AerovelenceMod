using Microsoft.Xna.Framework;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace AerovelenceMod.Blocks.FrostDungeon.Traps
{
	public class SnowriumArenaTrap : ModTile
	{
		public override void SetDefaults()
		{
			TileID.Sets.DrawsWalls[Type] = true;

			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileFrameImportant[Type] = true;

			AddMapEntry(new Color(21, 179, 192), Language.GetText("MapObject.Trap"));
		}

		public int TrapTimer = 0;

		public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].frameY / 18);

		public override bool Dangersense(int i, int j, Player player) => true;

		public override bool CreateDust(int i, int j, ref int type)
		{
			int style = Main.tile[i, j].frameY / 18;
			if (style == 0)
				type = 13;
			return true;
		}

		public override void PlaceInWorld(int i, int j, Item item)
		{
			int style = Main.LocalPlayer.HeldItem.placeStyle;
			Tile tile = Main.tile[i, j];
			tile.frameY = (short)(style * 18);
			if (Main.LocalPlayer.direction == 1)
			{
				tile.frameX += 18;
			}
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
			}
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			Vector2 tilePosition = new Vector2(i, j) * 16f;
			if (Vector2.Distance(tilePosition, Main.LocalPlayer.Center) < 10f * 16f)
			{
				Tile tile = Main.tile[i, j];
				int style = tile.frameY / 18;
				Vector2 spawnPosition;
				int horizontalDirection = (tile.frameX == 0) ? -1 : ((tile.frameX == 18) ? 1 : 0);
				int verticalDirection = (tile.frameX < 36) ? 0 : ((tile.frameX < 72) ? -1 : 1);
				TrapTimer++;
				if (TrapTimer == 60)
				{
					spawnPosition = new Vector2(i * 16 + 8 + 0 * horizontalDirection, j * 16 + 9 + 0 * verticalDirection);
					Projectile.NewProjectile(spawnPosition, new Vector2(horizontalDirection, verticalDirection) * 6f, ProjectileID.FrostBlastHostile, 20, 2f, Main.myPlayer);
					TrapTimer = 0;
				}
			}
		}
	}
}