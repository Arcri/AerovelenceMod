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
	public class LightningMothBag : ModItem
	{
		public override int BossBagNPC => Mod.Find<ModNPC>("LightningMoth").Type;

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
			player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<PrismaticSoul>()), Mod.Find<ModItem>("PrismaticSoul").Type);
			player.QuickSpawnItem(player.GetSource_OpenItem(ItemID.GoldCoin), ItemID.GoldCoin, 18);
			player.QuickSpawnItem(player.GetSource_OpenItem(ItemID.GreaterHealingPotion), ItemID.GreaterHealingPotion, Main.rand.Next(4, 12));

			switch (Main.rand.Next(6))
			{
				case 0:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<BladeOfTheSkies>()), Mod.Find<ModItem>("BladeOfTheSkies").Type);
					break;
				case 1:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<EyeOfTheGreatMoth>()), Mod.Find<ModItem>("EyeOfTheGreatMoth").Type);
					break;
				case 2:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<MothLeg>()), Mod.Find<ModItem>("MothLeg").Type);
					break;
				case 3:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<Florentine>()), Mod.Find<ModItem>("Florentine").Type);
					break;
				case 4:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<ElectrapulseCanister>()), Mod.Find<ModItem>("ElectrapulseCanister").Type);
					break;
				case 5:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<SongOfTheStorm>()), Mod.Find<ModItem>("SongOfTheStorm").Type);
					break;
				case 6:
					player.QuickSpawnItem(player.GetSource_OpenItem(ModContent.ItemType<StaticSurge>()), Mod.Find<ModItem>("StaticSurge").Type);
					break;
			}
		}
	}
}