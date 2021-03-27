using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.TreasureBags
{
	public class DecurionBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("DecurionHead");

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}

		public override void SetDefaults()
		{
			item.maxStack = 999;
			item.consumable = true;
			item.width = 36;
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
			player.QuickSpawnItem(mod.ItemType("EnergyShield"));
			player.QuickSpawnItem(ItemID.GoldCoin, 9);

			int drop = Main.rand.Next(7);

			player.TryGettingDevArmor();

			switch (Main.rand.Next(4))
			{
				case 0:
					player.QuickSpawnItem(mod.ItemType("DarknessDischarge"));
					break;
				case 1:
					player.QuickSpawnItem(mod.ItemType("Oblivion"));
					break;
				case 2:
					player.QuickSpawnItem(mod.ItemType("CyverCannon"));
					break;
				case 3:
					player.QuickSpawnItem(mod.ItemType("Cyverthrow"));
					break;
				case 4:
					player.QuickSpawnItem(mod.ItemType("AetherVision"));
					break;
			}
		}
	}
}