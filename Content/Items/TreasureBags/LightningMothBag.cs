using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.TreasureBags
{
	public class LightningMothBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("LightningMoth");

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
			player.QuickSpawnItem(mod.ItemType("PrismaticSoul"));
			player.QuickSpawnItem(ItemID.GoldCoin, 18);
			player.QuickSpawnItem(ItemID.GreaterHealingPotion, Main.rand.Next(4, 12));

			player.TryGettingDevArmor();

			switch (Main.rand.Next(6))
			{
				case 0:
					player.QuickSpawnItem(mod.ItemType("BladeOfTheSkies"));
					break;
				case 1:
					player.QuickSpawnItem(mod.ItemType("EyeOfTheGreatMoth"));
					break;
				case 2:
					player.QuickSpawnItem(mod.ItemType("MothLeg"));
					break;
				case 3:
					player.QuickSpawnItem(mod.ItemType("Florentine"));
					break;
				case 4:
					player.QuickSpawnItem(mod.ItemType("ElectrapulseCanister"));
					break;
				case 5:
					player.QuickSpawnItem(mod.ItemType("SongOfTheStorm"));
					break;
				case 6:
					player.QuickSpawnItem(mod.ItemType("StaticSurge"));
					break;
			}
		}
	}
}