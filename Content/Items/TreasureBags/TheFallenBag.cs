using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace AerovelenceMod.Content.Items.TreasureBags
{
	public class TheFallenBag : ModItem
	{
		public override int BossBagNPC => Mod.Find<ModNPC>("TheFallen").Type;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Treasure Bag");
			Tooltip.SetDefault("{$CommonItemTooltip.RightClickToOpen}");
		}

		public override void SetDefaults()
		{
			Item.maxStack = 999;
			Item.consumable = true;
			Item.width = 42;
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
			var entitySource = player.GetSource_OpenItem(Type);
			player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("GlassDeflector").Type);
			player.QuickSpawnItem(entitySource, ItemID.GoldCoin, 15);
			player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("CrystalShard").Type, Main.rand.Next(15) + 15);

			int ItemDrop = Main.rand.Next(3);


			switch (Main.rand.Next(5))
			{
				case 0:
					player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("CrystalKnife").Type);
					break;
				case 1:
					player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("WindboundWave").Type);
					break;
				case 2:
					player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("OzoneShredder").Type);
					break;
				case 3:
					player.QuickSpawnItem(entitySource, Mod.Find<ModItem>("StormRazor").Type);
					break;
			}
		}
	}
}