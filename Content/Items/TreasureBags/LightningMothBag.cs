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
			player.QuickSpawnItem(Mod.Find<ModItem>("PrismaticSoul").Type);
			player.QuickSpawnItem(ItemID.GoldCoin, 18);
			player.QuickSpawnItem(ItemID.GreaterHealingPotion, Main.rand.Next(4, 12));

			player.TryGettingDevArmor();

			switch (Main.rand.Next(6))
			{
				case 0:
					player.QuickSpawnItem(Mod.Find<ModItem>("BladeOfTheSkies").Type);
					break;
				case 1:
					player.QuickSpawnItem(Mod.Find<ModItem>("EyeOfTheGreatMoth").Type);
					break;
				case 2:
					player.QuickSpawnItem(Mod.Find<ModItem>("MothLeg").Type);
					break;
				case 3:
					player.QuickSpawnItem(Mod.Find<ModItem>("Florentine").Type);
					break;
				case 4:
					player.QuickSpawnItem(Mod.Find<ModItem>("ElectrapulseCanister").Type);
					break;
				case 5:
					player.QuickSpawnItem(Mod.Find<ModItem>("SongOfTheStorm").Type);
					break;
				case 6:
					player.QuickSpawnItem(Mod.Find<ModItem>("StaticSurge").Type);
					break;
			}
		}
	}
}