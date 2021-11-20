using AerovelenceMod.Content.Items.Weapons.Ranged;
using System.Collections.Generic;
using AerovelenceMod.Common.Globals.Worlds.WorldGeneration;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Microsoft.Xna.Framework;
using Terraria.ModLoader.IO;
using static AerovelenceMod.Common.Utilities.ChestUtilities;
using AerovelenceMod.Content.Items.Placeables.Blocks;
using AerovelenceMod.Content.Items.Others.Misc;
using AerovelenceMod.Content.Tiles.CrystalCaverns.Tiles.Furniture;

namespace AerovelenceMod
{
    public class AeroWorld : ModWorld
	{
		public static Dictionary<Vector2, Vector2> ETPLinks = new Dictionary<Vector2, Vector2>();
		public IList<Vector2> Keys;
		public IList<Vector2> Values;
		public bool Bingus;
		public override void PreUpdate()
		{
			/*if (!Bingus)
			{
				for (int i = 0; i < Keys.Count; i++)
				{
					if (!ETPLinks.ContainsKey(Keys[i]))
					ETPLinks.Add(Keys[i], Values[i]);
				}
				Bingus = false;
			}*/
		}
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int idx = tasks.FindIndex(t => t.Name == "Underworld"); //Terrain
			if (idx == -1)
			{
				idx = 1;
			}

		//	var pass = new WormGenPass();

			//tasks.Insert(idx, pass);    //+1

			//totalWeight += pass.Weight;
		}
        public override void PostWorldGen()
        {
			int[] itemsToPlaceInMarbleChests = { ModContent.ItemType<MarbleMusket>(), ItemID.SilverBullet};
			int itemsToPlaceInMarbleChestsChoice = 0;

			int[] itemsToPlaceInGraniteChests = { ModContent.ItemType<GraniteCannon>(), ItemID.SilverBullet };
			int itemsToPlaceInGraniteChestsChoice = 0;

			int[] commonItems1 = new int[] { ItemID.CopperBar, ItemID.IronBar, ItemID.TinBar, ItemID.LeadBar };
			int[] ammo1 = new int[] { ItemID.WoodenArrow, ItemID.Shuriken };
			int[] potions = new int[] { ItemID.SwiftnessPotion, ItemID.IronskinPotion, ItemID.ShinePotion, ItemID.NightOwlPotion, ItemID.ArcheryPotion, ItemID.HunterPotion };
			int[] recall = new int[] { ItemID.RecallPotion };
			int[] potionscorrupt = new int[] { ItemID.WrathPotion };
			int[] potionscrim = new int[] { ItemID.RagePotion, ItemID.HeartreachPotion };
			int[] other1 = new int[] { ItemID.HerbBag, ItemID.Grenade };
			int[] other2 = new int[] { ItemID.Bottle, ItemID.Torch };
			int[] moddedMaterials = new int[] {ModContent.ItemType<CavernCrystal>(), ModContent.ItemType<MiningSack>() };

			List<ChestInfo> CavernPool = new List<ChestInfo> {
				new ChestInfo(new int[] { ItemID.MagicMirror, ItemID.WandofSparking }),
				new ChestInfo(commonItems1, WorldGen.genRand.Next(3, 10)),
				new ChestInfo(ammo1, WorldGen.genRand.Next(20, 50)),
				new ChestInfo(potions, WorldGen.genRand.Next(2, 4)),
				new ChestInfo(ItemID.RecallPotion, WorldGen.genRand.Next(1, 3)),
				new ChestInfo(other1, WorldGen.genRand.Next(1, 4)),
				new ChestInfo(other2, WorldGen.genRand.Next(1, 4)),
				new ChestInfo(moddedMaterials, WorldGen.genRand.Next(2, 6)),
				new ChestInfo(ItemID.SilverCoin, WorldGen.genRand.Next(12, 30))
			};
			AddToModdedChest(CavernPool, ModContent.TileType<CavernChest>());
			for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
			{
				Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].type == TileID.Containers && Main.tile[chest.x, chest.y].frameX == 51 * 36)
				{
					for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
					{
						if (chest.item[inventoryIndex].type == ItemID.None)
						{
							chest.item[inventoryIndex].SetDefaults(itemsToPlaceInMarbleChests[itemsToPlaceInMarbleChestsChoice]);
							itemsToPlaceInMarbleChestsChoice = (itemsToPlaceInMarbleChestsChoice + 1) % itemsToPlaceInMarbleChests.Length;
							break;
						}
					}
				}
			}
			for (int chestIndex = 0; chestIndex < 1000; chestIndex++)
			{
				Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].type == TileID.Containers && Main.tile[chest.x, chest.y].frameX == 50 * 36)
				{
					for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
					{
						if (chest.item[inventoryIndex].type == ItemID.None)
						{
							chest.item[inventoryIndex].SetDefaults(itemsToPlaceInGraniteChests[itemsToPlaceInGraniteChestsChoice]);
							itemsToPlaceInGraniteChestsChoice = (itemsToPlaceInGraniteChestsChoice + 1) % itemsToPlaceInGraniteChests.Length;
							break;
						}
					}
				}
			}
		}
		public override TagCompound Save()
		{
			return new TagCompound {
		{"ETPLinksKeys", new List<Vector2>(ETPLinks.Keys)},
		{"ETPLinksValues", new List<Vector2>(ETPLinks.Values)}
			};
		}
		public override void Load(TagCompound tag)
		{
			if (tag.ContainsKey("ETPLinksKeys"))
			{
				Keys = tag.GetList<Vector2>("ETPLinksKeys");
			}
			if (tag.ContainsKey("ETPLinksValues"))
			{
				Values = tag.GetList<Vector2>("ETPLinksValues");
			}
		}
	}
}
 