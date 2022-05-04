using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.TreasureBags
{
	public class RimegeistBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("Rimegeist");

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}

		public override void SetDefaults()
		{
			item.maxStack = 999;
			item.consumable = true;
			item.width = 42;
			item.height = 32;
			item.rare = ItemRarityID.Purple;
			item.expert = true;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.QuickSpawnItem(mod.ItemType("FragileIceCrystal"));
			player.QuickSpawnItem(ItemID.GoldCoin, 15);
			player.QuickSpawnItem(mod.ItemType("FrostShard"), Main.rand.Next(10) + 10);
			player.QuickSpawnItem(ItemID.HealingPotion, Main.rand.Next(4, 20));

			int drop = Main.rand.Next(7);

			switch (Main.rand.Next(5))
			{
				case 0:
					player.QuickSpawnItem(mod.ItemType("CrystalArch"));
					break;
				case 1:
					player.QuickSpawnItem(mod.ItemType("IcySaber"));
					break;
				case 2:
					player.QuickSpawnItem(mod.ItemType("CryoBall"));
					break;
				case 3:
					player.QuickSpawnItem(mod.ItemType("DeepFreeze"));
					break;
				case 4:
					player.QuickSpawnItem(mod.ItemType("Snowball"));
					break;
			}
		}
	}
}