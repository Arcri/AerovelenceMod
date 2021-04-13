using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using System.Linq;
using static Terraria.ModLoader.ModContent;
using System.Collections.Generic;

namespace AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture
{
    public class ElectricTravelPylon : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Electric Travel Pylon");
			AddMapEntry(new Color(034, 153, 186), name);
			dustType = DustID.Electric;
			disableSmartCursor = true;
			adjTiles = new int[] { TileID.Painting2X3 };
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
			i -= Main.tile[i, j].frameX / 18 % 2;
			j -= Main.tile[i, j].frameY / 18 % 3;
			Vector2 position = new Vector2(i * 16, (j - 3) * 16);
			Item.NewItem(i * 16, j * 16, 64, 32, ItemType<ElectricTravelPylonItem>());
			foreach (KeyValuePair<Vector2, Vector2> entry in AeroWorld.ETPLinks)
			{
				if (entry.Key == position || entry.Value == position)
				{
					AeroWorld.ETPLinks.Remove(entry.Key);
				}
			}
		}

		public override bool NewRightClick(int i, int j)
		{
			var player = Main.LocalPlayer.GetModPlayer<AeroPlayer>();
			i -= Main.tile[i, j].frameX / 18 % 2;
			j -= Main.tile[i, j].frameY / 18 % 3;
			Vector2 position = new Vector2(i * 16, (j - 3) * 16);
			if (!player.TravellingByETP)
			{
				if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
				{
					if (player.IsETPBeingLinked)
					{
						if (new Vector2(i * 16, (j - 3) * 16) != player.ETPBeingLinkedPosition)
						{
							player.IsETPBeingLinked = false;
							CombatText.NewText(new Rectangle(i * 16, j * 16, 16, 18), Color.LightSkyBlue, "Link Estabilished");
							AeroWorld.ETPLinks[player.ETPBeingLinkedPosition] = position;
							if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.CapsLock))
							{
								AeroWorld.ETPLinks[position] = player.ETPBeingLinkedPosition;
							}
						}
					}
					else
					{
						player.IsETPBeingLinked = true;
						player.ETPBeingLinkedPosition = position;
						CombatText.NewText(new Rectangle(i * 16, j * 16, 16, 18), Color.LightSkyBlue, "Link Open");
					}
				}
				else if (AeroWorld.ETPLinks.ContainsKey(position))
				{
					player.ETPDestination = new Vector2(AeroWorld.ETPLinks[position].X, AeroWorld.ETPLinks[position].Y + 40f);
					Main.LocalPlayer.position = new Vector2(position.X, position.Y + 40f);
					player.TravellingByETP = true;
					for (int i2 = 0; i2 < 4; i2++)
					{
						Dust Dust1 = Dust.NewDustDirect(new Vector2(position.X + 8f, position.Y + 32f), 20, 20, DustID.Electric, 0f, 0f, 100, default, 1f);
						Dust1.velocity *= 1.6f;
						Dust Dust2 = Dust1;
						Dust2.velocity.Y -= 1f;
						Dust1.position = Vector2.Lerp(Dust1.position, new Vector2(position.X + 8f, position.Y + 32f), 0.5f);
					}
				}
				else
				{
					CombatText.NewText(new Rectangle(i * 16, j * 16, 16, 18), Color.LightSkyBlue, "No Link Estabilished");
				}

			}
			return true;
		}
		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.noThrow = 2;
			player.showItemIcon = true;
			player.showItemIcon2 = ItemType<ElectricTravelPylonItem>();
		}
		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			i -= Main.tile[i, j].frameX / 18 % 2;
			j -= Main.tile[i, j].frameY / 18 % 3;
			Vector2 position = new Vector2(i * 16, (j - 3) * 16);
			if (Main.netMode > NetmodeID.SinglePlayer && Main.player.Any(p => p.active && p.GetModPlayer<AeroPlayer>().ETPDestination == new Vector2(AeroWorld.ETPLinks[position].X, AeroWorld.ETPLinks[position].Y + 40f)))
			{
				return false;
			}
			else
			{
				if (Main.LocalPlayer.GetModPlayer<AeroPlayer>().TravellingByETP)
					return false;
				else
					return true;
			}
		}
	}
}