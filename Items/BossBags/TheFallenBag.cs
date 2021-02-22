using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Items.BossBags
{
	public class TheFallenBag : ModItem
	{
		public override int BossBagNPC => mod.NPCType("TheFallen");

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
			player.QuickSpawnItem(mod.ItemType("GlassDeflector"));
			player.QuickSpawnItem(ItemID.GoldCoin, 15);
			player.QuickSpawnItem(mod.ItemType("CrystalShard"), Main.rand.Next(15) + 15);

			int drop = Main.rand.Next(3);

			player.TryGettingDevArmor();

			switch (Main.rand.Next(5))
			{
				case 0:
					player.QuickSpawnItem(mod.ItemType("CrystalKnife"));
					break;
				case 1:
					player.QuickSpawnItem(mod.ItemType("WindboundWave"));
					break;
				case 2:
					player.QuickSpawnItem(mod.ItemType("OzoneShredder"));
					break;
				case 3:
					player.QuickSpawnItem(mod.ItemType("StormRazor"));
					break;
			}
		}
	}
}