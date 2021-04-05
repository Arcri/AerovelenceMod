using AerovelenceMod.Content.Dusts;
using AerovelenceMod.Content.Items.Placeables.CrystalCaverns;
using AerovelenceMod.Content.Items.Placeables.Furniture.Glimmering;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

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
            Item.NewItem(i * 16, j * 16, 64, 32, ItemType<ElectricTravelPylonItem>());
		}

		public override bool NewRightClick(int i, int j)
		{
			var player = Main.LocalPlayer.GetModPlayer<AeroPlayer>();
			i -= Main.tile[i, j].frameX / 18 % 2;
			j -= Main.tile[i, j].frameY / 18 % 3;
			if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
			{
				if (player.IsETPBeingLinked)
				{
					if (new Vector2(i * 16, (j - 3) * 16) != player.ETPBeingLinkedPosition)
					{
						player.IsETPBeingLinked = false;
						CombatText.NewText(new Rectangle(i * 16, j * 16, 16, 18), Color.LightSkyBlue, "Link Estabilished");
						AeroWorld.ETPLinks[player.ETPBeingLinkedPosition] = new Vector2(i * 16, (j - 3) * 16);
					}
				}
				else
				{
					player.IsETPBeingLinked = true;
					player.ETPBeingLinkedPosition = new Vector2(i * 16, (j - 3) * 16);
					CombatText.NewText(new Rectangle(i * 16, j * 16, 16, 18), Color.LightSkyBlue, "Link Open");
				}
			}
			else if (AeroWorld.ETPLinks.ContainsKey(new Vector2(i * 16, (j - 3) * 16)))
			{
				player.TravellingByETP = true;
				player.ETPDestination = AeroWorld.ETPLinks[new Vector2(i * 16, (j - 3) * 16)];
			}
			else
			{
				CombatText.NewText(new Rectangle(i * 16, j * 16, 16, 18), Color.LightSkyBlue, "No Link Estabilished");
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
	}
}