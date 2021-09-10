using AerovelenceMod.Content.Items.Armor.Vanity;
using AerovelenceMod.Content.Items.Placeables.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.TreasureBags
{
	public class CyvercryBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("Cyvercry");

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
			player.TryGettingDevArmor();
			player.QuickSpawnItem(mod.ItemType("EnergyShield"));
			player.QuickSpawnItem(ItemID.GoldCoin, 15);
			player.QuickSpawnItem(ItemID.GreaterHealingPotion, Main.rand.Next(4, 12));


			if (Main.rand.NextBool(7))
			{
				switch (Main.rand.Next(2))
				{
					case 0:
						player.QuickSpawnItem(ModContent.ItemType<CyvercryMask>());
						break;
					case 1:
						player.QuickSpawnItem(ModContent.ItemType<CyvercryTrophy>());
						break;
				}
					
			}

			switch (Main.rand.Next(6))
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
				case 5:
					player.QuickSpawnItem(ModContent.ItemType<Weapons.Thrown.DarkDagger>());
					break;
			}
		}
	}
}