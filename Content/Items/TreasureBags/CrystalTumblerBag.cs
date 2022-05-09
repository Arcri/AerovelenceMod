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
		public override int BossBagNPC => Mod.Find<ModNPC>("CrystalTumbler").Type;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}

		public override void SetDefaults()
		{
			Item.maxStack = 999;
			Item.consumable = true;
			Item.width = 36;
			Item.height = 32;
			Item.rare = ItemRarityID.Purple;
			Item.expert = true;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<PrismaticSoul>()), ModContent.ItemType<PrismaticSoul>());
			player.QuickSpawnItem(player.GetSource_OpenItem(ItemID.GoldCoin), ItemID.GoldCoin, 9);
			player.QuickSpawnItem(player.GetSource_OpenItem(ItemID.HealingPotion), ItemID.HealingPotion, Main.rand.Next(4, 12));

			switch (Main.rand.Next(8))
			{
				case 0:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<CrystallineQuadshot>()), ModContent.ItemType<CrystallineQuadshot>());
					break;
				case 1:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<PrismPiercer>()), ModContent.ItemType<PrismPiercer>());
					break;
				case 2:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<CarbonCadence>()), ModContent.ItemType<CarbonCadence>());
					break;
				case 3:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<PrismThrasher>()), ModContent.ItemType<PrismThrasher>());
					break;
				case 4:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<CavernousImpaler>()), ModContent.ItemType<CavernousImpaler>());
					break;
				case 5:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<CavernMauler>()), ModContent.ItemType<CavernMauler>());
					break;
				case 6:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<DarkCrystalStaff>()), ModContent.ItemType<DarkCrystalStaff>());
					break;
				case 7:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<ShiningCrystalCore>()), ModContent.ItemType<ShiningCrystalCore>());
					break;
			}
		}
	}
}