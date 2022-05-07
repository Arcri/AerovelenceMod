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
			Item.maxStack = 999;
			Item.consumable = true;
			Item.width = 36;
			Item.height = 32;
			Item.rare = ItemRarityID.Green;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void RightClick(Player player)
		{
			var s = player.GetSource_OpenItem(Type);
			player.QuickSpawnItem(s, ItemID.GoldCoin, 3);

			int ItemDrop = Main.rand.Next(7);
			int chance = Main.rand.Next(5);

			if (!Main.hardMode)
			{
				switch (Main.rand.Next(5))
				{
					case 0:
						player.QuickSpawnItem(s, ItemID.IronOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 1:
						player.QuickSpawnItem(s,ItemID.LeadOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 2:
						player.QuickSpawnItem(s,ItemID.SilverOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 3:
						player.QuickSpawnItem(s,ItemID.TungstenOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 4:
						player.QuickSpawnItem(s,ItemID.GoldOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 5:
						player.QuickSpawnItem(s,ItemID.PlatinumOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
				}
			}
			else
            {
				switch (Main.rand.Next(5))
				{
					case 0:
						player.QuickSpawnItem(s,ItemID.CobaltOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 1:
						player.QuickSpawnItem(s,ItemID.PalladiumOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 2:
						player.QuickSpawnItem(s,ItemID.OrichalcumOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 3:
						player.QuickSpawnItem(s,ItemID.MythrilOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 4:
						player.QuickSpawnItem(s,ItemID.AdamantiteOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
					case 5:
						player.QuickSpawnItem(s,ItemID.TitaniumOre, Main.rand.Next(20, 30));
						if (chance == 0)
						{
							player.QuickSpawnItem(s,ItemID.Topaz, Main.rand.Next(2, 8));
						}
						else if (chance == 1)
						{
							player.QuickSpawnItem(s,ItemID.Sapphire, Main.rand.Next(2, 8));
						}
						else if (chance == 2)
						{
							player.QuickSpawnItem(s,ItemID.Ruby, Main.rand.Next(2, 8));
						}
						else if (chance == 3)
						{
							player.QuickSpawnItem(s,ItemID.Emerald, Main.rand.Next(2, 8));
						}
						else
						{
							player.QuickSpawnItem(s,ItemID.Diamond, Main.rand.Next(2, 8));
						}
						break;
				}
			}
		}
	}
}