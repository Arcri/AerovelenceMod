using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.NPCs.Bosses.Snowrium
{
	public class SnowriumBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("Snowrium");

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}

		public override void SetDefaults()
		{
			item.maxStack = 999;
			item.consumable = true;
			item.width = 24;
			item.height = 24;
			item.rare = ItemRarityID.Purple;
			item.expert = true;
		}

		public override bool CanRightClick()
		{
			return true;
		}

		public override void OpenBossBag(Player player)
		{
			int drop = Main.rand.Next(7);

			player.TryGettingDevArmor();

			switch (drop)
			{
				case 0:
					player.QuickSpawnItem(mod.ItemType("IcySaber"));
					break;
				case 1:
					player.QuickSpawnItem(mod.ItemType("CryoBall"));
					break;
				case 2:
					player.QuickSpawnItem(mod.ItemType("Snowball"));
					break;
				case 3:
					player.QuickSpawnItem(mod.ItemType("CrystalArch"));
					break;
				case 4:
					player.QuickSpawnItem(mod.ItemType("DeepFreeze"));
					break;
				case 5:
					player.QuickSpawnItem(mod.ItemType("FrozenBliss"));
					break;
				case 6:
					player.QuickSpawnItem(mod.ItemType("FragileIceCrystal"));
					break;
				default:
					break;
			}
		}
	}
}