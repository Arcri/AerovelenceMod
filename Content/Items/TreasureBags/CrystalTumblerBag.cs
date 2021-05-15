using AerovelenceMod.Content.Items.Accessories;
using AerovelenceMod.Content.Items.Weapons.Magic;
using AerovelenceMod.Content.Items.Weapons.Melee;
using AerovelenceMod.Content.Items.Weapons.Ranged;
using AerovelenceMod.Content.Items.Weapons.Summoning;
using AerovelenceMod.Content.Items.Weapons.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.TreasureBags
{
	public class CrystalTumblerBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("CrystalTumbler");

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
			player.QuickSpawnItem(ModContent.ItemType<PrismaticSoul>());
			player.QuickSpawnItem(ItemID.GoldCoin, 9);
			player.QuickSpawnItem(ItemID.HealingPotion, Main.rand.Next(4, 12));

			player.TryGettingDevArmor();

			switch (Main.rand.Next(7))
			{
				case 0:
					player.QuickSpawnItem(ModContent.ItemType<CrystallineQuadshot>());
					break;
				case 1:
					player.QuickSpawnItem(ModContent.ItemType<PrismPiercer>());
					break;
				case 2:
					player.QuickSpawnItem(ModContent.ItemType<CarbonCadence>());
					break;
				case 3:
					player.QuickSpawnItem(ModContent.ItemType<PrismThrasher>());
					break;
				case 4:
					player.QuickSpawnItem(ModContent.ItemType<CavernousImpaler>());
					break;
				case 5:
					player.QuickSpawnItem(ModContent.ItemType<CavernMauler>());
					break;
				case 6:
					player.QuickSpawnItem(ModContent.ItemType<DarkCrystalStaff>());
					break;
				case 7:
					player.QuickSpawnItem(ModContent.ItemType<ShiningCrystalCore>());
					break;
			}
		}
	}
}