using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.Others.Misc
{
	public class MiningSack : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mining Sack");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}
		public override void SetDefaults()
		{
			item.maxStack = 999;
			item.consumable = true;
			item.width = 36;
			item.height = 32;
			item.rare = ItemRarityID.Green;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void RightClick(Player player)
		{
			player.QuickSpawnItem(ItemID.GoldCoin, 3);

			int drop = Main.rand.Next(7);
			int chance = Main.rand.Next(5);

			if (!Main.hardMode)
			{
				switch (Main.rand.Next(5))
				{
					case 0:
						player.QuickSpawnItem(ItemID.IronOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 1:
						player.QuickSpawnItem(ItemID.LeadOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 2:
						player.QuickSpawnItem(ItemID.SilverOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 3:
						player.QuickSpawnItem(ItemID.TungstenOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 4:
						player.QuickSpawnItem(ItemID.GoldOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 5:
						player.QuickSpawnItem(ItemID.PlatinumOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
				}
			}
			else
            {
				switch (Main.rand.Next(5))
				{
					case 0:
						player.QuickSpawnItem(ItemID.CobaltOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 1:
						player.QuickSpawnItem(ItemID.PalladiumOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 2:
						player.QuickSpawnItem(ItemID.OrichalcumOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 3:
						player.QuickSpawnItem(ItemID.MythrilOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 4:
						player.QuickSpawnItem(ItemID.AdamantiteOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 5:
						player.QuickSpawnItem(ItemID.TitaniumOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
				}
			}
		}
	}
}